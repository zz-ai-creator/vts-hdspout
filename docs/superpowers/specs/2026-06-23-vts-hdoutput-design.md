# VTS HDOutput Design

## Purpose

Build the next version of the VTube Studio output plugin as `VTS_HDOutput`. It should render the VTS Live2D camera into a fixed-resolution off-screen texture, then publish that texture through Spout, NDI, or both.

The output must remain independent of the VTS window size, monitor resolution, Windows scaling, and OBS canvas size.

## Rename Scope

The project moves from `VTS_HDSpout` to `VTS_HDOutput`.

Runtime names:

| Item | New Value |
| --- | --- |
| Plugin name | `VTS_HDOutput` |
| Assembly | `VTS_HDOutput.dll` |
| Default sender name | `VTS_HDOutput` |
| BepInEx plugin folder | `BepInEx\plugins\VTS_HDOutput` |
| BepInEx GUID | `me.codex.plugin.vts.hdoutput` |
| Config file | `me.codex.plugin.vts.hdoutput.cfg` |

The deploy script should remove the old `BepInEx\plugins\VTS_HDSpout` folder after copying the new plugin so VTS does not load both plugins. It should leave old config files in place; the new plugin starts with a clean config namespace.

## Supported Presets

The plugin supports four fixed output presets:

| Preset | Resolution | Orientation |
| --- | ---: | --- |
| `Landscape1080p` | `1920x1080` | Landscape |
| `Landscape4K` | `3840x2160` | Landscape |
| `Portrait1080p` | `1080x1920` | Portrait |
| `Portrait4K` | `2160x3840` | Portrait |

No arbitrary custom resolution UI is in scope for this version.

## Frame Rates

The plugin supports two explicit output frame rate modes:

| Mode | Meaning |
| --- | --- |
| `Fps30` | Send at 30 fps. Default. |
| `Fps60` | Send at 60 fps. Higher load. |

The existing unlimited/every-frame mode is removed from the user-facing UI for this version. The core cadence implementation may still clamp values internally for safety, but the overlay and config should expose only 30 and 60.

## Transport Modes

The plugin supports:

| Mode | Meaning |
| --- | --- |
| `Spout` | Send only a Spout sender. Default for local OBS capture. |
| `NDI` | Send only an NDI sender. Intended for network or cross-machine workflows. |
| `Both` | Send Spout and NDI from the same rendered texture. High load at 4K. |

Spout remains the recommended local OBS path because it shares textures locally. NDI is supported because VTS already bundles `Klak.Ndi.Runtime.dll`, `KlakNDI.dll`, and `Processing.NDI.Lib.x64.dll`.

## NDI Integration

The NDI sender should use VTS-bundled KlakNDI rather than deploying another NDI SDK build.

Implementation target:

```text
Klak.Ndi.NdiSender
captureMethod = Texture
sourceTexture = plugin sender-facing RenderTexture
keepAlpha = true
ndiName = configured sender name
```

The NDI path must not use game view capture or VTS window capture. It should receive the same off-screen texture used by Spout after the vertical orientation step. If NDI output orientation differs from Spout during runtime validation, add a separate NDI flip setting rather than changing the shared render target behavior.

## Runtime Architecture

```text
VTS_HDOutputPlugin
  - OutputConfig
    Binds enabled state, transport, preset, fps, sender names, alpha, flip, and overlay settings.
  - CameraResolver
    Finds and caches the VTS Live2D camera.
  - HdRenderTarget
    Owns the fixed-size ARGB32 render target.
  - HdCameraRenderer
    Temporarily renders the Live2D camera into the plugin render target and restores VTS camera state with ResetAspect().
  - SpoutFrameBuffer
    Creates the sender-facing texture and applies vertical flip when configured.
  - RenderCadence
    Gates rendering to 30 or 60 fps.
  - OutputRouter
    Starts, updates, and stops SpoutOutput and NdiOutput according to Transport.
  - SpoutOutput
    Sends through the existing KlakSpout native wrapper.
  - NdiOutput
    Sends through Klak.Ndi.NdiSender using Texture capture mode.
  - TopToolbarOverlay
    Draws the VTS top-centered control window.
```

## Data Flow

```text
VTS Live2D camera
  -> plugin-owned RenderTexture at selected preset size
  -> optional GPU vertical-flip sender texture
  -> OutputRouter
       -> SpoutOutput when Transport is Spout or Both
       -> NdiOutput when Transport is NDI or Both
  -> OBS / NDI receivers
```

Only one camera render should happen per due output frame, even when `Transport = Both`.

## Top-Centered VTS Toolbar

The in-VTS feature window changes from a draggable debug panel to a top-centered toolbar.

Layout:

```text
[ VTS HDOutput ] [ On/Off ] [ Spout | NDI | Both ] [ 1080H | 4KH | 1080V | 4KV ] [ 30 | 60 ] [ status ]
```

Behavior:

- The toolbar is shown when `ShowOverlay = true`.
- It is horizontally centered using `Screen.width`.
- It is anchored near the top with a small margin.
- It is not draggable in this version.
- Button clicks update BepInEx config immediately.
- Status text shows active transport, resolution, fps, and last output status.

The toolbar should have fixed dimensions so VTS window resize does not cause text overlap or layout jumps.

## Configuration

The first HDOutput version exposes:

| Key | Default | Meaning |
| --- | --- | --- |
| `Enabled` | `true` | Enables all configured outputs. |
| `Transport` | `Spout` | `Spout`, `NDI`, or `Both`. |
| `SenderName` | `VTS_HDOutput` | Base sender name for Spout and NDI. |
| `Preset` | `Landscape1080p` | Output size and orientation. |
| `FrameRate` | `Fps30` | Output fps mode. |
| `FlipVertical` | `true` | Flips sender texture vertically for Spout/OBS orientation. |
| `KeepAlpha` | `true` | Preserves alpha for NDI where supported. |
| `FitMode` | `Fit` | `Fit` or `Fill` camera framing. |
| `Scale` | `1.0` | Additional framing scale. |
| `OffsetX` | `0.0` | Horizontal framing offset. |
| `OffsetY` | `0.0` | Vertical framing offset. |
| `ShowOverlay` | `true` | Shows the top-centered toolbar. |

If `Transport = Both`, Spout and NDI should use the same sender base name. If a runtime collision appears, NDI may append ` NDI` and Spout may keep the exact base name; this should be logged in status.

## Performance Policy

Default mode is `Spout + Landscape1080p + Fps30`.

Expected load tiers:

| Mode | Expected Load |
| --- | --- |
| 1080p Spout 30 | Normal |
| 1080p Spout 60 | Moderate |
| 4K Spout 30 | Moderate to high |
| 4K Spout 60 | High |
| 4K NDI 30 | High |
| 4K NDI 60 | Very high |
| 4K Both 60 | Stress mode |

The plugin should not block users from high-load modes, but status text and documentation should make clear that 4K NDI and Both modes are expensive.

## Compatibility

Required local VTS paths:

```text
D:\steamapps\steamapps\common\VTube Studio\VTube Studio_Data\Plugins\x86_64\KlakSpout.dll
D:\steamapps\steamapps\common\VTube Studio\VTube Studio_Data\Plugins\x86_64\KlakNDI.dll
D:\steamapps\steamapps\common\VTube Studio\VTube Studio_Data\Plugins\x86_64\Processing.NDI.Lib.x64.dll
D:\steamapps\steamapps\common\VTube Studio\VTube Studio_Data\Managed\Klak.Ndi.Runtime.dll
```

The project should reference `Klak.Ndi.Runtime.dll` from the VTS managed folder with `Private=false`. It must not copy NDI native DLLs into the BepInEx plugin folder.

## Error Handling

- If Spout sender creation fails, status should show the Spout error and NDI should continue when `Transport = Both`.
- If NDI sender creation fails, status should show the NDI error and Spout should continue when `Transport = Both`.
- If camera discovery fails, all outputs should pause and status should show `Waiting for Live2D Camera`.
- If a preset switch reallocates the render texture, both outputs should rebind to the new texture.
- Disabling the plugin should release both senders.

## Acceptance Criteria

- Built assembly is `VTS_HDOutput.dll`.
- BepInEx loads `VTS_HDOutput` without loading the old `VTS_HDSpout` plugin.
- Toolbar appears top-centered in the VTS window.
- Toolbar can switch transport, preset, fps, and enabled state.
- Spout output works at all four presets and both fps modes.
- NDI output creates an NDI sender at all four presets and both fps modes.
- `Transport = Both` sends both outputs from one render pass.
- VTS window resize does not stretch the avatar.
- Source code contains no real-time `ReadPixels`, `EncodeToPNG`, or screenshot loop.
- Unit tests cover preset dimensions, fps mapping, transport mode routing decisions, and toolbar layout math.

## Out of Scope

- Arbitrary custom resolutions.
- Audio over NDI.
- NDI HX configuration.
- OBS automation.
- Replacing VTS built-in output features.
- macOS support.
