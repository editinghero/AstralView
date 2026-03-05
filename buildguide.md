# AstralView

### Modern Windows 11 GUI for scrcpy

---

# Project Overview

**AstralView** is a modern Windows 11 GUI application for controlling Android devices using **scrcpy**.

The application is designed as a **lightweight wrapper around the scrcpy CLI**, not a reimplementation.

The GUI dynamically generates scrcpy commands and launches them.

Example command:

```
scrcpy --video-source=camera --audio-source=output --max-size=1024
```

The project should prioritize:

* Clean architecture
* Minimal code complexity
* High performance
* Modern Windows 11 design
* GitHub Actions automated builds

---

# Technology Stack

## Language

C#

## Framework

.NET 8

## UI Framework

WinUI 3 (Windows App SDK)

## UI Design Requirements

Use **Windows 11 Fluent Design** with:

* Mica backdrop
* Fluent UI components
* Modern Windows layout
* Native Windows controls

The UI must resemble **modern Windows 11 apps**.

---

# Build Requirements

The project must support **automatic builds using GitHub Actions**.

The developer should **not need a powerful local machine**.

The project must build using:

```
dotnet build
```

Output:

```
AstralView.exe
```

---

# GitHub Actions Workflow

Create:

```
.github/workflows/build.yml
```

Example workflow:

```yaml
name: Build AstralView

on:
  push:
  pull_request:

jobs:
  build:

    runs-on: windows-latest

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x

      - run: dotnet restore

      - run: dotnet build --configuration Release

      - uses: actions/upload-artifact@v4
        with:
          name: build
          path: bin/Release
```

---

# Core Architecture

The application must use a **simple layered architecture**.

```
UI Layer
   ↓
Command Builder
   ↓
Process Runner
   ↓
scrcpy.exe + adb.exe
```

Responsibilities:

### UI Layer

Handles user interaction and settings.

### Command Builder

Creates scrcpy CLI commands from user settings.

### Process Runner

Launches scrcpy processes.

### External Tools

Uses:

```
scrcpy.exe
adb.exe
```

---

# Project Structure

```
AstralView
│
├─ App
│   ├─ App.xaml
│   └─ App.xaml.cs
│
├─ UI
│   ├─ MainWindow.xaml
│   ├─ DevicePanel.xaml
│   ├─ CameraPanel.xaml
│   ├─ AudioPanel.xaml
│   ├─ VideoPanel.xaml
│
├─ Core
│   ├─ ScrcpyCommandBuilder.cs
│   ├─ ScrcpyRunner.cs
│   ├─ DeviceManager.cs
│   ├─ WirelessManager.cs
│
├─ Models
│   ├─ Device.cs
│   ├─ ScrcpySettings.cs
│
├─ Services
│   ├─ AdbService.cs
│   ├─ PresetService.cs
│
├─ Tools
│   ├─ scrcpy.exe
│   ├─ adb.exe
│   ├─ scrcpy-server.jar
│
└─ .github
    └─ workflows
        └─ build.yml
```

---

# Core Features

The application must implement the following features with **minimal complexity and clean code**.

---

# 📷 Camera Control UI

Expose scrcpy camera streaming features.

Supported parameters:

```
--video-source=camera
--camera-id
--camera-facing
--camera-size
```

UI Controls:

* Camera enable toggle
* Front / Back camera selector
* Camera resolution selector
* Camera ID selector

Example command:

```
scrcpy --video-source=camera --camera-facing=front --camera-size=1920x1080
```

---

# 🎤 Audio Routing

Expose scrcpy audio forwarding options.

Supported parameters:

```
--audio-source=output
--audio-source=mic
--no-audio
```

UI Controls:

* Audio enable toggle
* Audio source selector

---

# 📱 Multi Device Control

Detect connected devices using ADB.

Command:

```
adb devices
```

UI should display:

* Device list
* Device serial
* Device model

User can select a device.

Example scrcpy command:

```
scrcpy -s DEVICE_SERIAL
```

---

# 📡 Wireless Connect

Allow connection to devices over WiFi.

Commands:

```
adb tcpip 5555
adb connect IP_ADDRESS
```

UI Elements:

* IP address input
* Connect button
* Disconnect button

---

# 🎮 Gaming Low Latency Mode

Provide a preset optimized for gaming.

Example settings:

```
--max-size=1024
--bit-rate=16M
--max-fps=120
--video-buffer=0
```

Expose as a **Gaming Mode toggle**.

---

# 🎬 Recording Presets

Support screen recording.

Parameters:

```
--record=file.mp4
--record-format=mp4
```

Presets:

* Recording Mode
* Streaming Mode
* Gaming Mode
* Low Bandwidth Mode

Users should be able to select presets.

---

# Video Settings

Expose video configuration options.

Supported parameters:

```
--max-size
--bit-rate
--codec
--max-fps
```

UI Controls:

* Resolution dropdown
* Bitrate slider
* Codec selector
* FPS selector

---

# Window Controls

Expose window configuration.

Supported parameters:

```
--fullscreen
--always-on-top
--window-title
```

---

# Command Builder

Implement a class:

```
ScrcpyCommandBuilder
```

Example output:

```
scrcpy -s DEVICEID --video-source=camera --audio-source=output --max-size=1024
```

The command builder should dynamically assemble parameters.

---

# Process Execution

Use the .NET process API.

Example:

```csharp
Process.Start("scrcpy.exe", arguments);
```

This launches scrcpy using generated arguments.

---

# UI Layout

The main interface should contain the following sections:

```
Device Panel
Video Settings
Camera Controls
Audio Settings
Wireless Connect
Presets
Start Button
```

Example layout:

```
┌─────────────────────────────┐
│ Device: Pixel 7 ▼           │
├──────── Video ──────────────┤
│ Resolution                  │
│ Bitrate                     │
│ Codec                       │
├──────── Camera ─────────────┤
│ Camera Enable               │
│ Front / Back                │
│ Camera Size                 │
├──────── Audio ──────────────┤
│ Audio Enable                │
│ Source                      │
├──────── Wireless ───────────┤
│ IP Address                  │
│ Connect                     │
├──────── Presets ────────────┤
│ Gaming                      │
│ Recording                   │
│ Streaming                   │
├─────────────────────────────┤
│        Start Scrcpy         │
└─────────────────────────────┘
```

---

# Code Quality Requirements

The codebase must:

* Be concise
* Avoid unnecessary abstraction
* Use minimal dependencies
* Be easy to maintain
* Follow clean architecture principles

---

# Performance Goals

The GUI must remain lightweight.

All heavy operations must be handled by **scrcpy itself**.

The GUI should only:

* Generate CLI arguments
* Launch processes
* Manage devices

---

# Final Result

The repository must provide:

* A working WinUI 3 application
* GitHub Actions build pipeline
* scrcpy integration
* Modern Windows 11 UI with Mica
* Lightweight architecture

The result should be a **modern scrcpy GUI similar to Android Studio Device Manager**.
