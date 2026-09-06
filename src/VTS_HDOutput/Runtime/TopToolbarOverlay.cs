using System;
using UnityEngine;
using VTS_HDOutput.Core;

namespace VTS_HDOutput.Runtime
{
    internal sealed class TopToolbarOverlay
    {
        private const int WindowId = 971204;
        private const int MinimizedWindowId = 971205;
        private const int Width = 500;
        private const int Height = 172;
        private const int MinimizedWidth = 150;
        private const int MinimizedHeight = 38;
        private const int LeftMargin = 10;
        private const int TopMargin = 10;

        public void Draw(
            bool visible,
            bool minimized,
            bool enabled,
            OutputAspectRatio aspectRatio,
            OutputResolution resolution,
            OutputFrameRate frameRate,
            string status,
            Action<bool> setMinimized,
            Action<bool> setEnabled,
            Action<OutputAspectRatio> setAspectRatio,
            Action<OutputResolution> setResolution,
            Action<OutputFrameRate> setFrameRate)
        {
            if (!visible) return;

            if (minimized)
            {
                var minimizedLayout = ToolbarLayout.TopLeft(MinimizedWidth, MinimizedHeight, LeftMargin, TopMargin);
                var minimizedRect = new Rect(
                    minimizedLayout.X,
                    minimizedLayout.Y,
                    minimizedLayout.Width,
                    minimizedLayout.Height);

                GUILayout.Window(MinimizedWindowId, minimizedRect, _ =>
                {
                    if (GUILayout.Button("VTS HDOutput", GUILayout.Height(22)))
                        setMinimized(false);
                }, string.Empty);

                return;
            }

            var layout = ToolbarLayout.TopLeft(Width, Height, LeftMargin, TopMargin);
            var rect = new Rect(layout.X, layout.Y, layout.Width, layout.Height);

            GUILayout.Window(WindowId, rect, _ =>
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal(GUILayout.Height(24));
                GUILayout.Label("VTS HDOutput", GUILayout.Width(130));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Min", GUILayout.Width(42)))
                    setMinimized(true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Height(24));
                GUILayout.Label("Output", GUILayout.Width(76));
                GUILayout.Label("[Spout]", GUILayout.Width(58));
                GUILayout.Space(12);
                if (GUILayout.Button(enabled ? "[ON]" : "[OFF]", GUILayout.Width(58)))
                    setEnabled(!enabled);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Height(24));
                GUILayout.Label("Aspect", GUILayout.Width(76));
                DrawAspectRatioButton("9:16", OutputAspectRatio.Ratio9x16, aspectRatio, setAspectRatio);
                DrawAspectRatioButton("16:9", OutputAspectRatio.Ratio16x9, aspectRatio, setAspectRatio);
                DrawAspectRatioButton("3:4", OutputAspectRatio.Ratio3x4, aspectRatio, setAspectRatio);
                DrawAspectRatioButton("4:3", OutputAspectRatio.Ratio4x3, aspectRatio, setAspectRatio);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Height(24));
                GUILayout.Label("Resolution", GUILayout.Width(76));
                DrawResolutionButton("1080P", OutputResolution.P1080, resolution, setResolution);
                DrawResolutionButton("2K", OutputResolution.K2, resolution, setResolution);
                DrawResolutionButton("4K", OutputResolution.K4, resolution, setResolution);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.Height(24));
                GUILayout.Label("FPS", GUILayout.Width(76));
                DrawFrameRateButton("30", OutputFrameRate.Fps30, frameRate, setFrameRate);
                DrawFrameRateButton("60", OutputFrameRate.Fps60, frameRate, setFrameRate);
                GUILayout.EndHorizontal();

                GUILayout.Label(status ?? "No status", GUILayout.Width(468));
                GUILayout.EndVertical();
            }, string.Empty);
        }

        private static void DrawAspectRatioButton(string label, OutputAspectRatio value, OutputAspectRatio current, Action<OutputAspectRatio> setAspectRatio)
        {
            if (GUILayout.Button(current == value ? "[" + label + "]" : label, GUILayout.Width(58)))
                setAspectRatio(value);
        }

        private static void DrawResolutionButton(string label, OutputResolution value, OutputResolution current, Action<OutputResolution> setResolution)
        {
            if (GUILayout.Button(current == value ? "[" + label + "]" : label, GUILayout.Width(64)))
                setResolution(value);
        }

        private static void DrawFrameRateButton(string label, OutputFrameRate value, OutputFrameRate current, Action<OutputFrameRate> setFrameRate)
        {
            if (GUILayout.Button(current == value ? "[" + label + "]" : label, GUILayout.Width(44)))
                setFrameRate(value);
        }
    }
}
