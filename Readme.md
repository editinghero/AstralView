# AstralView

Modern Windows 11 GUI for scrcpy

AstralView is a lightweight Windows desktop application that provides a graphical interface for controlling Android devices using **scrcpy**.
It acts as a wrapper around the scrcpy command line interface, allowing users to configure device mirroring, camera streaming, audio routing, and performance settings through a modern Windows interface.

The application is designed with **Windows 11 Fluent Design and Mica backdrop**, providing a clean native experience while keeping the architecture simple and efficient.

Repository author: https://github.com/editinghero/

---

# Features

## Device Management

* Detect connected Android devices using ADB
* Display device list and device identifiers
* Select device for scrcpy sessions
* Support for multiple devices

## Camera Control

* Use Android device cameras through scrcpy
* Select camera source
* Choose front or back camera
* Configure camera resolution and camera ID

## Audio Routing

* Forward device audio to the computer
* Choose audio source
* Enable or disable audio forwarding

## Wireless Connection

* Connect Android devices over WiFi using ADB
* TCP/IP connection support
* Simple IP-based connection interface

## Video Configuration

* Resolution selection
* Bitrate configuration
* Codec selection
* FPS configuration

## Window Controls

* Fullscreen mode
* Always-on-top option
* Custom window title

## Presets

Preset configurations for different use cases:

* Gaming mode (low latency)
* Recording mode
* Streaming mode
* Low bandwidth mode

## Recording

* Record device screen using scrcpy recording options
* Save recordings as MP4 files

---

# Architecture

AstralView uses a simple layered architecture that focuses on clarity and maintainability.

```
UI Layer
   ↓
Command Builder
   ↓
Process Runner
   ↓
scrcpy.exe + adb.exe
```

## UI Layer

Handles user interface interactions using WinUI 3.

## Command Builder

Generates scrcpy command arguments based on user settings.

## Process Runner

Launches scrcpy processes and manages execution.

## External Tools

AstralView uses the official scrcpy binaries:

* scrcpy.exe
* adb.exe
* scrcpy-server.jar

---

# Technology Stack

Language
C#

Framework
.NET 8

UI Framework
WinUI 3 (Windows App SDK)

Design
Windows 11 Fluent Design System with Mica backdrop

Build System
GitHub Actions

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

# Building the Project

AstralView is designed to be built automatically using GitHub Actions.

Local builds are optional.

To build locally:

```
dotnet restore
dotnet build --configuration Release
```

Output executable will be generated in:

```
bin/Release/
```

---

# GitHub Actions

Automated builds run using GitHub Actions.

Workflow file location:

```
.github/workflows/build.yml
```

Example configuration:

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

# Requirements

Windows 10 or Windows 11

Android device with USB debugging enabled

ADB installed or included with the application

scrcpy binaries bundled with the application

---

# How It Works

AstralView does not implement video streaming itself.

Instead it dynamically generates scrcpy commands such as:

```
scrcpy -s DEVICE_ID --video-source=camera --audio-source=output --max-size=1024
```

The application launches scrcpy using the .NET process API.

This ensures maximum performance since scrcpy handles all streaming operations.

---

# UI Layout

The interface contains the following sections:

```
Device Panel
Video Settings
Camera Controls
Audio Settings
Wireless Connection
Presets
Start Button
```

Example layout:

```
Device Selection
Video Configuration
Camera Controls
Audio Settings
Wireless Connection
Preset Selection
Start Scrcpy
```

---

# Goals

* Provide a modern Windows interface for scrcpy
* Keep the architecture simple
* Maintain high performance
* Enable easy builds through GitHub Actions
* Support advanced scrcpy features through a graphical interface

---

# License

This project follows the same usage philosophy as scrcpy and relies on the official scrcpy binaries.

Refer to the scrcpy project for licensing details.

https://github.com/Genymobile/scrcpy

---

# Author

GitHub
https://github.com/editinghero/
