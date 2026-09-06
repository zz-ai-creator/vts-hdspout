# VTS_HDOutput

## 中文

### 功能

`VTS_HDOutput` 是一个用于 VTube Studio 的 Windows 插件，用于将 VTS 人物画面输出为高清 Spout 信号，方便在 OBS 等软件中采集。

- 输出不受 VTS 窗口大小、显示器分辨率、Windows 缩放或 OBS 画布大小影响。
- 支持 Spout 输出，发送器名称为 `VTS_HDOutput`。
- 支持横屏和竖屏比例：`16:9`、`9:16`、`4:3`、`3:4`。
- 支持分辨率：`1080P`、`2K`、`4K`。
- 支持帧率：`30 FPS`、`60 FPS`。
- 支持透明背景，方便在 OBS 中叠加使用。
- VTS 内左上角提供英文功能面板，可选择输出开关、比例、分辨率和帧率。

### 安装方式

1. 下载并解压 `VTS_HDOutput` 发布包。
2. 如果 VTube Studio 安装在默认 Steam 路径，双击运行：

```text
install-vts-hdoutput.bat
```

默认路径：

```text
C:\Program Files (x86)\Steam\steamapps\common\VTube Studio
```

3. 如果 VTube Studio 安装在其他位置，在命令行中传入 VTS 目录：

```bat
install-vts-hdoutput.bat "D:\SteamLibrary\steamapps\common\VTube Studio"
```

4. 安装完成后启动 VTube Studio，在 OBS 中添加 Spout2 Source，并选择 `VTS_HDOutput`。

### 卸载方式

如果 VTube Studio 安装在默认 Steam 路径，双击运行：

```text
uninstall-vts-hdoutput.bat
```

如果 VTube Studio 安装在其他位置，在命令行中传入 VTS 目录：

```bat
uninstall-vts-hdoutput.bat "D:\SteamLibrary\steamapps\common\VTube Studio"
```

卸载器会删除 `VTS_HDOutput` 插件文件和本插件配置文件，并保留 BepInEx，避免影响其他插件。

**最后，将接收Spout的软件设定为指定分辨率，如在Shoost中使用4K，可以将分辨率设置为:3840x2160**

## English

### Features

`VTS_HDOutput` is a Windows plugin for VTube Studio. It publishes the VTS character image as a high-resolution Spout signal for OBS and other receivers.

- Output is independent of the VTS window size, monitor resolution, Windows scaling, and OBS canvas size.
- Supports Spout output with the sender name `VTS_HDOutput`.
- Supports landscape and portrait aspect ratios: `16:9`, `9:16`, `4:3`, and `3:4`.
- Supports output resolutions: `1080P`, `2K`, and `4K`.
- Supports frame rates: `30 FPS` and `60 FPS`.
- Supports transparent background output for OBS compositing.
- Provides an English toolbar in the top-left corner of VTS for output state, aspect ratio, resolution, and frame rate.

### Install

1. Download and extract the `VTS_HDOutput` release package.
2. If VTube Studio is installed in the default Steam path, double-click:

```text
install-vts-hdoutput.bat
```

Default path:

```text
C:\Program Files (x86)\Steam\steamapps\common\VTube Studio
```

3. If VTube Studio is installed elsewhere, pass the VTS directory from the command line:

```bat
install-vts-hdoutput.bat "D:\SteamLibrary\steamapps\common\VTube Studio"
```

4. After installation, start VTube Studio, add a Spout2 Source in OBS, and select `VTS_HDOutput`.

### Uninstall

If VTube Studio is installed in the default Steam path, double-click:

```text
uninstall-vts-hdoutput.bat
```

If VTube Studio is installed elsewhere, pass the VTS directory from the command line:

```bat
uninstall-vts-hdoutput.bat "D:\SteamLibrary\steamapps\common\VTube Studio"
```

The uninstaller removes the `VTS_HDOutput` plugin files and this plugin's config file. BepInEx is left installed so other plugins are not affected.

 **Finally, configure the software receiving the Spout feed to the desired resolution; for example, if using 4K in Shoost, you can set the resolution to 3840x2160.** 