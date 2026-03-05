using AstralView.Core;
using AstralView.Models;
using AstralView.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace AstralView;

public sealed partial class MainWindow : Window
{
    private sealed record ShortcutItem(string Action, string Shortcut);

    private readonly ScrcpyRunner _runner;
    private readonly WirelessManager _wirelessManager;
    private readonly AdbService _adb;
    private readonly DeviceManager _deviceManager;

    public MainWindow()
    {
        this.InitializeComponent();

        try
        {
            SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        }
        catch
        {
            // MicaBackdrop requires Windows 11, fallback to default
        }
        
        Title = "AstralView";

        _adb = new AdbService(ScrcpyPaths.AdbPath);
        _runner = new ScrcpyRunner(ScrcpyPaths.ScrcpyPath);
        _wirelessManager = new WirelessManager(_adb);
        _deviceManager = new DeviceManager(_adb);

        DevicePanelControl.Initialize(_adb, _deviceManager);

        ShortcutsListView.ItemsSource = GetDefaultShortcuts();

        WireCommandPreviewEvents();
        
        // Listen to camera toggle to disable video
        CameraPanelControl.CameraToggleSwitch.Toggled += (s, e) =>
        {
            VideoPanelControl.VideoOptions.IsEnabled = !CameraPanelControl.CameraEnabled;
            VideoPanelControl.VideoOptions.Opacity = CameraPanelControl.CameraEnabled ? 0.5 : 1.0;
            UpdateCommandPreview();
        };
        
        UpdateCommandPreview();
    }

    private void WireCommandPreviewEvents()
    {
        FullscreenCheck.Checked += (_, _) => UpdateCommandPreview();
        FullscreenCheck.Unchecked += (_, _) => UpdateCommandPreview();
        AlwaysOnTopCheck.Checked += (_, _) => UpdateCommandPreview();
        AlwaysOnTopCheck.Unchecked += (_, _) => UpdateCommandPreview();

        RecordToggle.Toggled += (_, _) => UpdateCommandPreview();
        RecordFileBox.TextChanged += (_, _) => UpdateCommandPreview();

        IpAddressBox.TextChanged += (_, _) => { };
        PortBox.TextChanged += (_, _) => { };

        AudioPanelControl.Loaded += (_, _) =>
        {
            AudioPanelControl.FindName("AudioToggle");
            UpdateCommandPreview();
        };

        AudioPanelControl.AddHandler(ToggleSwitch.ToggledEvent, new RoutedEventHandler((_, _) => UpdateCommandPreview()), true);
        AudioPanelControl.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((_, _) => UpdateCommandPreview()), true);

        VideoPanelControl.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((_, _) => UpdateCommandPreview()), true);
        VideoPanelControl.AddHandler(Slider.ValueChangedEvent, new Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventHandler((_, _) => UpdateCommandPreview()), true);

        CameraPanelControl.AddHandler(ToggleSwitch.ToggledEvent, new RoutedEventHandler((_, _) => UpdateCommandPreview()), true);
        CameraPanelControl.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((_, _) => UpdateCommandPreview()), true);
        CameraPanelControl.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler((_, _) => UpdateCommandPreview()), true);
        CameraPanelControl.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler((_, _) => UpdateCommandPreview()), true);
        CameraPanelControl.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler((_, _) => UpdateCommandPreview()), true);
    }

    private static ShortcutItem[] GetDefaultShortcuts() => new[]
    {
        new ShortcutItem("Switch fullscreen mode", "MOD+f"),
        new ShortcutItem("Rotate display left", "MOD+←"),
        new ShortcutItem("Rotate display right", "MOD+→"),
        new ShortcutItem("Flip display horizontally", "MOD+Shift+← | MOD+Shift+→"),
        new ShortcutItem("Flip display vertically", "MOD+Shift+↑ | MOD+Shift+↓"),
        new ShortcutItem("Pause or re-pause display", "MOD+z"),
        new ShortcutItem("Unpause display", "MOD+Shift+z"),
        new ShortcutItem("Reset video capture/encoding", "MOD+Shift+r"),
        new ShortcutItem("Resize window to 1:1 (pixel-perfect)", "MOD+g"),
        new ShortcutItem("Resize window to remove black borders", "MOD+w | Double-left-click"),
        new ShortcutItem("Click on HOME", "MOD+h | Middle-click"),
        new ShortcutItem("Click on BACK", "MOD+b | MOD+Backspace | Right-click"),
        new ShortcutItem("Click on APP_SWITCH", "MOD+s | 4th-click"),
        new ShortcutItem("Click on MENU (unlock screen)", "MOD+m"),
        new ShortcutItem("Click on VOLUME_UP", "MOD+↑"),
        new ShortcutItem("Click on VOLUME_DOWN", "MOD+↓"),
        new ShortcutItem("Click on POWER", "MOD+p"),
        new ShortcutItem("Power on", "Right-click"),
        new ShortcutItem("Turn device screen off (keep mirroring)", "MOD+o"),
        new ShortcutItem("Turn device screen on", "MOD+Shift+o"),
        new ShortcutItem("Rotate device screen", "MOD+r"),
        new ShortcutItem("Expand notification panel", "MOD+n | 5th-click"),
        new ShortcutItem("Expand settings panel", "MOD+n+n | Double-5th-click"),
        new ShortcutItem("Collapse panels", "MOD+Shift+n"),
        new ShortcutItem("Copy to clipboard", "MOD+c"),
        new ShortcutItem("Cut to clipboard", "MOD+x"),
        new ShortcutItem("Synchronize clipboards and paste", "MOD+v"),
        new ShortcutItem("Inject computer clipboard text", "MOD+Shift+v"),
        new ShortcutItem("Open keyboard settings (HID keyboard only)", "MOD+k"),
        new ShortcutItem("Enable/disable FPS counter (on stdout)", "MOD+i"),
        new ShortcutItem("Pinch-to-zoom/rotate", "Ctrl + click-and-move"),
        new ShortcutItem("Tilt vertically (slide with 2 fingers)", "Shift + click-and-move"),
        new ShortcutItem("Tilt horizontally (slide with 2 fingers)", "Ctrl+Shift + click-and-move"),
        new ShortcutItem("Drag & drop APK file", "Install APK from computer"),
        new ShortcutItem("Drag & drop non-APK file", "Push file to device"),
    };

    private void UpdateCommandPreview()
    {
        var settings = BuildSettings();
        var builder = new ScrcpyCommandBuilder(settings);
        var args = builder.Build();
        CommandPreviewText.Text = $"scrcpy {args}";
    }

    private ScrcpySettings BuildSettings()
    {
        return new ScrcpySettings
        {
            DeviceSerial = DevicePanelControl.SelectedDevice?.Serial,
            UseCamera = CameraPanelControl.CameraEnabled,
            CameraFacing = CameraPanelControl.Facing,
            CameraSize = CameraPanelControl.CameraSize,
            CameraId = CameraPanelControl.CameraId,
            TurnScreenOff = CameraPanelControl.TurnScreenOff,
            MaxSize = CameraPanelControl.CameraEnabled ? 0 : VideoPanelControl.MaxSize,
            BitRateMbps = VideoPanelControl.BitRate,
            MaxFps = VideoPanelControl.MaxFps,
            Codec = VideoPanelControl.Codec,
            AudioSource = AudioPanelControl.SelectedAudioSource,
            Fullscreen = FullscreenCheck.IsChecked == true,
            AlwaysOnTop = AlwaysOnTopCheck.IsChecked == true,
            Record = RecordToggle.IsOn,
            RecordFile = RecordFileBox.Text.Length > 0 ? RecordFileBox.Text : "record.mp4"
        };
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var args = CommandPreviewText.Text.Replace("scrcpy ", "").Trim();

        try
        {
            _runner.Start(args);
            StatusBarText.Text = "scrcpy started";
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            StatusBarText.Text = $"Error: {ex.Message}";
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        _runner.ForceStop();
        StatusBarText.Text = "Stopped";
        StartButton.IsEnabled = true;
        StopButton.IsEnabled = false;
    }

    private async void WirelessConnect_Click(object sender, RoutedEventArgs e)
    {
        var ip = IpAddressBox.Text.Trim();
        var port = PortBox.Text.Trim();
        var serial = DevicePanelControl.SelectedDevice?.Serial ?? string.Empty;
        
        if (string.IsNullOrEmpty(ip)) return;

        int parsedPort = 5555;
        if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var p) && p > 0) parsedPort = p;
        var address = string.IsNullOrEmpty(port) ? ip : $"{ip}:{parsedPort}";
        
        WirelessStatusText.Text = "Connecting...";
        var result = await _wirelessManager.ConnectAsync(serial, address, parsedPort);
        WirelessStatusText.Text = result.Trim();
        
        await Task.Delay(1000);
        DevicePanelControl.RefreshDevices();
    }

    private async void WirelessDisconnect_Click(object sender, RoutedEventArgs e)
    {
        var ip = IpAddressBox.Text.Trim();
        var port = PortBox.Text.Trim();
        
        if (string.IsNullOrEmpty(ip)) return;

        int parsedPort = 5555;
        if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var p) && p > 0) parsedPort = p;
        var address = string.IsNullOrEmpty(port) ? ip : $"{ip}:{parsedPort}";
        var result = await _wirelessManager.DisconnectAsync(address);
        WirelessStatusText.Text = result.Trim();
        
        await Task.Delay(1000);
        DevicePanelControl.RefreshDevices();
    }

    private async void BrowseFile_Click(object sender, RoutedEventArgs e)
    {
        var savePicker = new FileSavePicker();
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);
        
        savePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
        savePicker.FileTypeChoices.Add("MP4 Video", new[] { ".mp4" });
        savePicker.FileTypeChoices.Add("Matroska Video", new[] { ".mkv" });
        savePicker.SuggestedFileName = "recording";

        var file = await savePicker.PickSaveFileAsync();
        if (file != null)
        {
            RecordFileBox.Text = file.Path;
        }
    }

    private async void TurnScreenOff_Click(object sender, RoutedEventArgs e)
    {
        var serial = DevicePanelControl.SelectedDevice?.Serial;
        if (string.IsNullOrEmpty(serial)) return;
        
        await _adb.ExecuteCommandAsync($"-s {serial} shell input keyevent 26");
        StatusBarText.Text = "Screen turned off";
    }

    private async void TurnScreenOn_Click(object sender, RoutedEventArgs e)
    {
        var serial = DevicePanelControl.SelectedDevice?.Serial;
        if (string.IsNullOrEmpty(serial)) return;
        
        await _adb.ExecuteCommandAsync($"-s {serial} shell input keyevent 26");
        StatusBarText.Text = "Screen turned on";
    }

    private async void ShowTouches_Click(object sender, RoutedEventArgs e)
    {
        var serial = DevicePanelControl.SelectedDevice?.Serial;
        if (string.IsNullOrEmpty(serial)) return;
        
        await _adb.ExecuteCommandAsync($"-s {serial} shell settings put system show_touches 1");
        StatusBarText.Text = "Show touches enabled";
    }

    private async void StayAwake_Click(object sender, RoutedEventArgs e)
    {
        var serial = DevicePanelControl.SelectedDevice?.Serial;
        if (string.IsNullOrEmpty(serial)) return;
        
        await _adb.ExecuteCommandAsync($"-s {serial} shell settings put global stay_on_while_plugged_in 7");
        StatusBarText.Text = "Stay awake enabled";
    }
}
