# VTS HDOutput Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rename the plugin to `VTS_HDOutput` and add selectable Spout, NDI, and Both output modes with 1080p/4K landscape/portrait presets, 30/60 fps, and a top-centered VTS toolbar.

**Architecture:** Keep pure output choices in `Core` so tests can cover preset, fps, transport, and toolbar layout decisions. Runtime code keeps one off-screen camera render per due frame, routes the sender-facing texture through `SpoutOutput`, `NdiOutput`, or both, and draws a fixed top-centered IMGUI toolbar. The NDI path uses VTS-bundled `Klak.Ndi.Runtime.dll` and does not copy NDI native DLLs.

**Tech Stack:** C# .NET Framework 4.7.2 BepInEx plugin, Unity 6000 runtime assemblies, VTS-bundled KlakSpout and KlakNDI, xUnit tests, PowerShell deployment.

---

## File Structure

Create or modify these paths:

```text
VTS_HDOutput.sln
src\VTS_HDOutput\VTS_HDOutput.csproj
src\VTS_HDOutput\Plugin.cs
src\VTS_HDOutput\Core\CompositionMath.cs
src\VTS_HDOutput\Core\OutputPreset.cs
src\VTS_HDOutput\Core\OutputTransport.cs
src\VTS_HDOutput\Core\OutputFrameRate.cs
src\VTS_HDOutput\Core\RenderCadence.cs
src\VTS_HDOutput\Core\ToolbarLayout.cs
src\VTS_HDOutput\Interop\KlakSpoutNative.cs
src\VTS_HDOutput\Interop\KlakSpoutSender.cs
src\VTS_HDOutput\Runtime\CameraResolver.cs
src\VTS_HDOutput\Runtime\HdCameraRenderer.cs
src\VTS_HDOutput\Runtime\HdRenderTarget.cs
src\VTS_HDOutput\Runtime\NdiOutput.cs
src\VTS_HDOutput\Runtime\OutputRouter.cs
src\VTS_HDOutput\Runtime\SpoutFrameBuffer.cs
src\VTS_HDOutput\Runtime\SpoutOutput.cs
src\VTS_HDOutput\Runtime\TopToolbarOverlay.cs
tests\VTS_HDOutput.Tests\VTS_HDOutput.Tests.csproj
tests\VTS_HDOutput.Tests\CompositionMathTests.cs
tests\VTS_HDOutput.Tests\OutputFrameRateTests.cs
tests\VTS_HDOutput.Tests\OutputPresetTests.cs
tests\VTS_HDOutput.Tests\OutputTransportTests.cs
tests\VTS_HDOutput.Tests\RenderCadenceTests.cs
tests\VTS_HDOutput.Tests\ToolbarLayoutTests.cs
scripts\deploy-plugin.ps1
README.md
```

Responsibilities:

- `Core`: pure enums and calculations.
- `Runtime`: Unity and BepInEx runtime behavior.
- `NdiOutput`: owns a `Klak.Ndi.NdiSender` component on a hidden GameObject.
- `OutputRouter`: releases and updates Spout/NDI senders based on `OutputTransport`.
- `TopToolbarOverlay`: fixed top-centered IMGUI toolbar.

### Task 1: Rename Project Skeleton

**Files:**
- Move: `src\VTS_HDOutput` -> `src\VTS_HDOutput`
- Move: `tests\VTS_HDOutput.Tests` -> `tests\VTS_HDOutput.Tests`
- Modify: `VTS_HDOutput.sln` -> `VTS_HDOutput.sln`
- Modify: project and test namespaces from `VTS_HDOutput` to `VTS_HDOutput`

- [ ] **Step 1: Move directories and solution**

Run:

```powershell
Move-Item -LiteralPath .\src\VTS_HDOutput -Destination .\src\VTS_HDOutput
Move-Item -LiteralPath .\tests\VTS_HDOutput.Tests -Destination .\tests\VTS_HDOutput.Tests
Move-Item -LiteralPath .\VTS_HDOutput.sln -Destination .\VTS_HDOutput.sln
Move-Item -LiteralPath .\src\VTS_HDOutput\VTS_HDOutput.csproj -Destination .\src\VTS_HDOutput\VTS_HDOutput.csproj
Move-Item -LiteralPath .\tests\VTS_HDOutput.Tests\VTS_HDOutput.Tests.csproj -Destination .\tests\VTS_HDOutput.Tests\VTS_HDOutput.Tests.csproj
```

- [ ] **Step 2: Replace project identifiers**

Replace `VTS_HDOutput` with `VTS_HDOutput` in source, test, README, scripts, and solution files, then restore deliberate legacy cleanup mentions where needed.

- [ ] **Step 3: Build to reveal remaining rename breaks**

Run:

```powershell
dotnet build .\src\VTS_HDOutput\VTS_HDOutput.csproj -c Release --no-restore
```

Expected: any failures are only path or namespace references still needing rename.

### Task 2: Add Core Output Choices With Tests

**Files:**
- Create: `src\VTS_HDOutput\Core\OutputTransport.cs`
- Create: `src\VTS_HDOutput\Core\OutputFrameRate.cs`
- Create: `src\VTS_HDOutput\Core\ToolbarLayout.cs`
- Modify: `src\VTS_HDOutput\Core\RenderCadence.cs`
- Create/modify tests under `tests\VTS_HDOutput.Tests`

- [ ] **Step 1: Write failing tests for transport, fps, cadence, and toolbar layout**

Tests should assert:

```csharp
OutputTransport.Spout sends Spout only.
OutputTransport.NDI sends NDI only.
OutputTransport.Both sends both.
OutputFrameRate.Fps30 maps to 30.
OutputFrameRate.Fps60 maps to 60.
RenderCadence accepts OutputFrameRate instead of arbitrary int.
ToolbarLayout.Centered(screenWidth, toolbarWidth, topMargin) returns centered x and fixed y.
```

- [ ] **Step 2: Run tests and verify failure**

Run:

```powershell
dotnet test .\tests\VTS_HDOutput.Tests\VTS_HDOutput.Tests.csproj --no-restore
```

Expected: compile fails because the new core files do not exist yet.

- [ ] **Step 3: Implement minimal core files**

Add enums and helpers:

```csharp
public enum OutputTransport { Spout, NDI, Both }
public enum OutputFrameRate { Fps30, Fps60 }
```

Expose helpers:

```csharp
OutputFrameRateInfo.ToInt(OutputFrameRate.Fps30) == 30
OutputFrameRateInfo.ToInt(OutputFrameRate.Fps60) == 60
OutputTransportInfo.UsesSpout(...)
OutputTransportInfo.UsesNdi(...)
ToolbarLayout.Centered(...)
```

- [ ] **Step 4: Run tests and verify pass**

Run:

```powershell
dotnet test .\tests\VTS_HDOutput.Tests\VTS_HDOutput.Tests.csproj --no-restore
```

Expected: all pure logic tests pass.

### Task 3: Update Build and Deployment

**Files:**
- Modify: `src\VTS_HDOutput\VTS_HDOutput.csproj`
- Modify: `tests\VTS_HDOutput.Tests\VTS_HDOutput.Tests.csproj`
- Modify: `scripts\deploy-plugin.ps1`

- [ ] **Step 1: Reference VTS-bundled KlakNDI**

Add a non-copying reference:

```xml
<Reference Include="Klak.Ndi.Runtime">
  <HintPath>$(VtsManaged)\Klak.Ndi.Runtime.dll</HintPath>
  <Private>false</Private>
</Reference>
```

- [ ] **Step 2: Update deploy script**

Deploy to:

```text
BepInEx\plugins\VTS_HDOutput
```

Copy only `VTS_HDOutput.dll`. Remove old deployed plugin folder:

```text
BepInEx\plugins\VTS_HDOutput
```

Do not copy `KlakSpout.dll`, `KlakNDI.dll`, or `Processing.NDI.Lib.x64.dll`.

- [ ] **Step 3: Build**

Run:

```powershell
dotnet build .\src\VTS_HDOutput\VTS_HDOutput.csproj -c Release --no-restore
```

Expected: build succeeds.

### Task 4: Add NDI Output and Router

**Files:**
- Create: `src\VTS_HDOutput\Runtime\NdiOutput.cs`
- Create: `src\VTS_HDOutput\Runtime\OutputRouter.cs`
- Modify: `src\VTS_HDOutput\Runtime\SpoutOutput.cs`
- Modify: `src\VTS_HDOutput\Plugin.cs`

- [ ] **Step 1: Implement `NdiOutput`**

Use a hidden GameObject with `Klak.Ndi.NdiSender`.

Behavior:

```text
Ensure(name, texture, keepAlpha):
  create GameObject and NdiSender if needed
  set ndiName, captureMethod Texture, sourceTexture, keepAlpha
  set status "Sending NDI <name> <width>x<height>"

Release():
  destroy hidden GameObject
```

- [ ] **Step 2: Implement `OutputRouter`**

Behavior:

```text
Ensure(transport, senderName, texture, keepAlpha):
  if transport uses Spout, ensure SpoutOutput; otherwise release SpoutOutput
  if transport uses NDI, ensure NdiOutput; otherwise release NdiOutput
  status combines both active statuses

Update():
  update SpoutOutput
```

NDI uses Klak's component update/coroutine path, so router does not manually call send per frame.

- [ ] **Step 3: Wire Plugin**

Replace direct `SpoutOutput` field with `OutputRouter`. Bind config:

```text
Transport = Spout
FrameRate = Fps30
KeepAlpha = true
SenderName = VTS_HDOutput
```

Use `OutputFrameRateInfo.ToInt(frameRateConfig.Value)` for cadence.

- [ ] **Step 4: Build**

Run:

```powershell
dotnet build .\src\VTS_HDOutput\VTS_HDOutput.csproj -c Release --no-restore
```

Expected: build succeeds.

### Task 5: Replace Overlay With Top Toolbar

**Files:**
- Delete/replace: `src\VTS_HDOutput\Runtime\DebugOverlay.cs`
- Create: `src\VTS_HDOutput\Runtime\TopToolbarOverlay.cs`
- Modify: `src\VTS_HDOutput\Plugin.cs`

- [ ] **Step 1: Implement toolbar**

Toolbar draws:

```text
VTS HDOutput | On/Off | Spout NDI Both | 1080H 4KH 1080V 4KV | 30 60 | status
```

Use `ToolbarLayout.Centered(Screen.width, width, topMargin)` for the window rect.

- [ ] **Step 2: Wire callbacks**

Callbacks update config values and call `Config.Save()`.

- [ ] **Step 3: Build**

Run:

```powershell
dotnet build .\src\VTS_HDOutput\VTS_HDOutput.csproj -c Release --no-restore
```

Expected: build succeeds.

### Task 6: Documentation, Verification, Commit, Push

**Files:**
- Modify: `README.md`
- Modify: `docs\superpowers\specs\2026-06-23-vts-hdoutput-design.md`

- [ ] **Step 1: Update README**

Document:

```text
VTS_HDOutput
Spout / NDI / Both
1080p / 4K landscape and portrait
30 / 60 fps
top-centered toolbar
deployment cleanup of VTS_HDOutput
```

- [ ] **Step 2: Run verification**

Run:

```powershell
dotnet test .\tests\VTS_HDOutput.Tests\VTS_HDOutput.Tests.csproj --no-restore
dotnet build .\src\VTS_HDOutput\VTS_HDOutput.csproj -c Release --no-restore
rg -n "ReadPixels|EncodeToPNG|CaptureScreenshot|Texture2D" .\src
```

Expected:

```text
tests pass
build succeeds with 0 warnings and 0 errors
rg returns no matches
```

- [ ] **Step 3: Commit and push**

Run:

```powershell
git status --short
git add .
git commit -m "Implement VTS HDOutput transport modes"
git push
```
