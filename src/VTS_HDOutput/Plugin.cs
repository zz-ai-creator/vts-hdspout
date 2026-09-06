using System;
using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using VTS_HDOutput.Core;
using VTS_HDOutput.Runtime;

namespace VTS_HDOutput
{
    [BepInPlugin(Guid, PluginName, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Guid = "me.codex.plugin.vts.hdoutput";
        public const string PluginName = "VTS_HDOutput";
        public const string Version = "0.1.0";

        private ConfigEntry<bool> enabledConfig;
        private ConfigEntry<OutputTransport> transportConfig;
        private ConfigEntry<string> senderNameConfig;
        private ConfigEntry<OutputAspectRatio> aspectRatioConfig;
        private ConfigEntry<OutputResolution> resolutionConfig;
        private ConfigEntry<OutputFrameRate> frameRateConfig;
        private ConfigEntry<FitMode> fitModeConfig;
        private ConfigEntry<float> scaleConfig;
        private ConfigEntry<float> offsetXConfig;
        private ConfigEntry<float> offsetYConfig;
        private ConfigEntry<bool> flipVerticalConfig;
        private ConfigEntry<bool> showOverlayConfig;
        private ConfigEntry<bool> overlayMinimizedConfig;

        private readonly CameraResolver cameraResolver = new CameraResolver();
        private readonly HdRenderTarget renderTarget = new HdRenderTarget();
        private readonly SpoutFrameBuffer spoutFrameBuffer = new SpoutFrameBuffer();
        private readonly HdCameraRenderer cameraRenderer = new HdCameraRenderer();
        private readonly OutputRouter outputRouter = new OutputRouter();
        private readonly TopToolbarOverlay overlay = new TopToolbarOverlay();

        private string status = "Starting";
        private string lastLoggedStatus;
        private double nextRenderAt;

        private void Awake()
        {
            enabledConfig = Config.Bind("General", "Enabled", true, "Enable HD output.");
            transportConfig = Config.Bind("General", "Transport", OutputTransport.Spout, "Output transport mode.");
            senderNameConfig = Config.Bind("General", "SenderName", "VTS_HDOutput", "Sender name shown in receivers.");
            aspectRatioConfig = Config.Bind("General", "AspectRatio", OutputAspectRatio.Ratio16x9, "Output aspect ratio.");
            resolutionConfig = Config.Bind("General", "Resolution", OutputResolution.P1080, "Output resolution tier.");
            frameRateConfig = Config.Bind("General", "FrameRate", OutputFrameRate.Fps30, "Output frame rate.");
            fitModeConfig = Config.Bind("Composition", "FitMode", FitMode.Fit, "Framing mode.");
            scaleConfig = Config.Bind("Composition", "Scale", 1f, "Additional framing scale.");
            offsetXConfig = Config.Bind("Composition", "OffsetX", 0f, "Horizontal framing offset.");
            offsetYConfig = Config.Bind("Composition", "OffsetY", 0f, "Vertical framing offset.");
            flipVerticalConfig = Config.Bind("General", "FlipVertical", true, "Flip the Spout texture vertically for receivers that read Unity render textures upside down.");
            showOverlayConfig = Config.Bind("General", "ShowOverlay", true, "Show output toolbar.");
            overlayMinimizedConfig = Config.Bind("General", "OverlayMinimized", false, "Keep the output toolbar minimized.");

            Logger.LogInfo("VTS_HDOutput loaded.");
            StartCoroutine(RenderLoop());
        }

        private IEnumerator RenderLoop()
        {
            var wait = new WaitForEndOfFrame();

            while (true)
            {
                yield return wait;

                if (!enabledConfig.Value)
                {
                    outputRouter.Release();
                    nextRenderAt = 0d;
                    status = "Disabled";
                    LogStatusIfChanged();
                    continue;
                }

                var now = Time.unscaledTime;
                var targetFps = OutputFrameRateInfo.ToInt(frameRateConfig.Value);
                if (!RenderCadence.ShouldRender(now, nextRenderAt, targetFps))
                    continue;

                nextRenderAt = RenderCadence.NextRenderAt(now, frameRateConfig.Value);

                try
                {
                    var camera = cameraResolver.Resolve();
                    if (camera == null)
                    {
                        status = "Waiting for Live2D Camera";
                        LogStatusIfChanged();
                        continue;
                    }

                    var preset = OutputPresetInfo.Describe(aspectRatioConfig.Value, resolutionConfig.Value);
                    renderTarget.Ensure(preset.Width, preset.Height);

                    var plan = CompositionMath.BuildPlan(
                        preset.Width,
                        preset.Height,
                        fitModeConfig.Value,
                        scaleConfig.Value,
                        offsetXConfig.Value,
                        offsetYConfig.Value);

                    cameraRenderer.Render(camera, renderTarget.Texture, plan);
                    var spoutTexture = spoutFrameBuffer.Prepare(renderTarget.Texture, flipVerticalConfig.Value);
                    outputRouter.Ensure(transportConfig.Value, senderNameConfig.Value, spoutTexture);
                    outputRouter.Update();
                    status = BuildStatus(preset, targetFps);
                    LogStatusIfChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    outputRouter.Release();
                    status = ex.GetType().Name + ": " + ex.Message;
                    LogStatusIfChanged();
                }
            }
        }

        private string BuildStatus(OutputPresetDescriptor preset, int targetFps)
        {
            return $"{transportConfig.Value} {preset.Width}x{preset.Height} {targetFps}fps | {outputRouter.Status}";
        }

        private void LogStatusIfChanged()
        {
            if (status == lastLoggedStatus) return;

            lastLoggedStatus = status;
            Logger.LogInfo(status);
        }

        private void OnGUI()
        {
            overlay.Draw(
                showOverlayConfig.Value,
                overlayMinimizedConfig.Value,
                enabledConfig.Value,
                aspectRatioConfig.Value,
                resolutionConfig.Value,
                frameRateConfig.Value,
                status,
                minimized =>
                {
                    overlayMinimizedConfig.Value = minimized;
                    Config.Save();
                },
                enabled =>
                {
                    enabledConfig.Value = enabled;
                    Config.Save();
                },
                aspectRatio =>
                {
                    aspectRatioConfig.Value = aspectRatio;
                    Config.Save();
                },
                resolution =>
                {
                    resolutionConfig.Value = resolution;
                    Config.Save();
                },
                frameRate =>
                {
                    frameRateConfig.Value = frameRate;
                    Config.Save();
                });
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            outputRouter.Release();
            spoutFrameBuffer.Release();
            renderTarget.Release();
        }
    }
}
