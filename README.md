# Toolkit

A collection of tools, scripts, and small applications that I build for myself to solve everyday problems and make some tasks easier. The repository contains everything from simple utility scripts to larger C# applications.

Some tools are used regularly, while others exist for those occasional situations where having a dedicated tool saves time.

## Tools

1. [SubConverter](#subconverter)
1. [FFMPEG Compresion](#ffmpeg-compression)

### [SubConverter](./SubConverter/)

Console line application that allows conversion between some popular subtitle formats.

#### Supported formats:
- `srt`
- `sub` (MicroDVD)

#### Usage

```bash
SubConverter [options]
```

#### Options

| Option | Alias | Required | Description | Default |
| --- | --- | --- | --- | --- |
| `--input` | `-i` | Yes | Path to the input subtitle file. | - |
| `--output` | `-o` | Yes | Path to the output subtitle file. | - |
| `--input-format` | `-if` | No | Input subtitle format. If omitted, it is inferred from the file extension. [Supported formats](#supported-formats) | Auto-detect |
| `--output-format` | `-of` | No | Output subtitle format. [Supported formats](#supported-formats) | `srt` |
| `--framerate` | `-fr` | No | Framerate used by frame-dependent formats, such as MicroDVD. Accepts a floating-point number. | `24` |
| `--offset` | - | No | Time offset applied to subtitles, in timespan format. | `00:00:00.000` |

### Example

```bash
SubConverter -i input.sub -o output.srt -of srt -fr 23.976 --offset 00:00:02.500
```

## Scripts

### [FFMPEG Compression](./Scripts/ffmpeg_compression.ps1)

A simple PowerShell script for compressing videos using **FFmpeg** with **NVIDIA GPU hardware encoding** (`hevc_nvenc`).

#### Usage

```powershell
.\ffmpeg_compression.ps1 input.mp4 output.mp4
```

The script encodes video using HEVC with VBR rate control.
