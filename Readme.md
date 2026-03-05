# AstralView

AstralView is a Windows desktop app that lets you control and mirror an Android device using scrcpy.

## How to use

### Requirements

- Windows 10 or Windows 11
- An Android device with USB debugging enabled

### Start mirroring over USB

1. Connect your Android device to the PC via USB.
2. Open AstralView.
3. In the **Device** section, select your device.
4. Adjust options in **Video**, **Camera**, **Audio**, and **Recording**.
5. Click **Start scrcpy**.

### Wireless (ADB over Wi-Fi)

1. Connect your device over USB at least once (so ADB can enable TCP/IP mode).
2. In AstralView, enter your device IP address.
3. Enter a port only if you use a custom TCP/IP port.
4. Click **Connect**.
5. Refresh the device list and start scrcpy.

### Camera mode

- Turn on **Camera** to mirror a camera feed instead of the display.
- Audio is off by default for camera mode. You can enable it in the **Audio** section.

### Recording

- Turn on **Recording**.
- Choose an output file path.
- Start scrcpy.

### Device actions

You can use the **Device Actions** buttons while scrcpy is running:

- Turn Screen Off / On
- Rotate Left / Rotate Right
- Show Touches
- Stay Awake

## Notes

- If you close the scrcpy window directly, AstralView detects it and updates the Start/Stop buttons.
- If scrcpy is running and you change settings, AstralView restarts scrcpy automatically when needed.

## Development

### Build

This project targets .NET 8 and WinUI 3.

```
dotnet restore
dotnet build -c Release
```

### Repository

https://github.com/editinghero/AstralView

### License

This project relies on scrcpy. Refer to the scrcpy project for licensing details:

https://github.com/Genymobile/scrcpy
