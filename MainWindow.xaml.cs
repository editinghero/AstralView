using AstralView.Core;
using AstralView.Models;
using AstralView.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace AstralView;

public sealed partial class MainWindow : Window
{
    private sealed record ShortcutItem(string Action, string Shortcut);

    private readonly TimeSpan _restartDebounce = TimeSpan.FromMilliseconds(500);
    private CancellationTokenSource? _restartCts;
    private bool _restartInProgress;

    private bool _isUpdatingCommandPreview;
    private bool _customCommandActive;

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
        }
        
        Title = "AstralView";

        TrySetWindowIcon();
        TrySetTitleBarToMatchBackground();

        _adb = new AdbService(ScrcpyPaths.AdbPath);
        _runner = new ScrcpyRunner(ScrcpyPaths.ScrcpyPath);
        _wirelessManager = new WirelessManager(_adb);
        _deviceManager = new DeviceManager(_adb);

        _runner.Exited += (_, _) => DispatcherQueue.TryEnqueue(() =>
        {
            _restartCts?.Cancel();
            StatusBarText.Text = "scrcpy closed";
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        });

        DevicePanelControl.Initialize(_adb, _deviceManager);

        ShortcutsListView.ItemsSource = GetDefaultShortcuts();

        WireCommandPreviewEvents();
        
        CameraPanelControl.CameraToggleSwitch.Toggled += (s, e) =>
        {
            if (CameraPanelControl.CameraEnabled)
            {
                AudioPanelControl.SetAudioEnabled(false);
            }

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

        RecordToggle.Toggled += (_, _) => OnSettingsChanged();
        RecordFileBox.TextChanged += (_, _) => OnSettingsChanged();

        CommandPreviewText.TextChanged += (_, _) => OnCommandPreviewEdited();

        IpAddressBox.TextChanged += (_, _) => { };
        PortBox.TextChanged += (_, _) => { };

        AudioPanelControl.SettingsChanged += (_, _) => OnSettingsChanged();
        VideoPanelControl.SettingsChanged += (_, _) => OnSettingsChanged();
        CameraPanelControl.SettingsChanged += (_, _) => OnSettingsChanged();
    }

    private void OnSettingsChanged()
    {
        if (string.IsNullOrWhiteSpace(CommandPreviewText.Text))
        {
            _customCommandActive = false;
        }

        UpdateCommandPreview();

        if (!_runner.IsRunning) return;
        _ = RestartScrcpyDebouncedAsync();
    }

    private void OnCommandPreviewEdited()
    {
        if (_isUpdatingCommandPreview) return;

        if (string.IsNullOrWhiteSpace(CommandPreviewText.Text))
        {
            _customCommandActive = false;
            return;
        }

        _customCommandActive = true;

        if (!_runner.IsRunning) return;
        _ = RestartScrcpyDebouncedAsync();
    }

    private async Task RestartScrcpyDebouncedAsync()
    {
        if (_restartInProgress) return;

        _restartCts?.Cancel();
        _restartCts?.Dispose();
        _restartCts = new CancellationTokenSource();
        var token = _restartCts.Token;

        try
        {
            await Task.Delay(_restartDebounce, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        await RestartScrcpyAsync();
    }

    private async Task RestartScrcpyAsync()
    {
        if (_restartInProgress) return;
        _restartInProgress = true;

        try
        {
            var args = CommandPreviewText.Text.Replace("scrcpy ", "").Trim();
            _runner.ForceStop();
            await Task.Delay(300);
            _runner.Start(args);
            StatusBarText.Text = "scrcpy restarted";
        }
        catch (Exception ex)
        {
            StatusBarText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            _restartInProgress = false;
        }
    }

    private static ShortcutItem[] GetDefaultShortcuts() => new[]
    {
        new ShortcutItem("Switch fullscreen mode", "Left Alt+f"),
        new ShortcutItem("Rotate display left", "Left Alt+←"),
        new ShortcutItem("Rotate display right", "Left Alt+→"),
        new ShortcutItem("Flip display horizontally", "Left Alt+Shift+← | Left Alt+Shift+→"),
        new ShortcutItem("Flip display vertically", "Left Alt+Shift+↑ | Left Alt+Shift+↓"),
        new ShortcutItem("Pause or re-pause display", "Left Alt+z"),
        new ShortcutItem("Unpause display", "Left Alt+Shift+z"),
        new ShortcutItem("Reset video capture/encoding", "Left Alt+Shift+r"),
        new ShortcutItem("Resize window to 1:1 (pixel-perfect)", "Left Alt+g"),
        new ShortcutItem("Resize window to remove black borders", "Left Alt+w | Double-left-click"),
        new ShortcutItem("Click on HOME", "Left Alt+h | Middle-click"),
        new ShortcutItem("Click on BACK", "Left Alt+b | Left Alt+Backspace | Right-click"),
        new ShortcutItem("Click on APP_SWITCH", "Left Alt+s | 4th-click"),
        new ShortcutItem("Click on MENU (unlock screen)", "Left Alt+m"),
        new ShortcutItem("Click on VOLUME_UP", "Left Alt+↑"),
        new ShortcutItem("Click on VOLUME_DOWN", "Left Alt+↓"),
        new ShortcutItem("Click on POWER", "Left Alt+p"),
        new ShortcutItem("Power on", "Right-click"),
        new ShortcutItem("Turn device screen off (keep mirroring)", "Left Alt+o"),
        new ShortcutItem("Turn device screen on", "Left Alt+Shift+o"),
        new ShortcutItem("Rotate device screen", "Left Alt+r"),
        new ShortcutItem("Expand notification panel", "Left Alt+n | 5th-click"),
        new ShortcutItem("Expand settings panel", "Left Alt+n+n | Double-5th-click"),
        new ShortcutItem("Collapse panels", "Left Alt+Shift+n"),
        new ShortcutItem("Copy to clipboard", "Left Alt+c"),
        new ShortcutItem("Cut to clipboard", "Left Alt+x"),
        new ShortcutItem("Synchronize clipboards and paste", "Left Alt+v"),
        new ShortcutItem("Inject computer clipboard text", "Left Alt+Shift+v"),
        new ShortcutItem("Open keyboard settings (HID keyboard only)", "Left Alt+k"),
        new ShortcutItem("Enable/disable FPS counter (on stdout)", "Left Alt+i"),
        new ShortcutItem("Pinch-to-zoom/rotate", "Ctrl + click-and-move"),
        new ShortcutItem("Tilt vertically (slide with 2 fingers)", "Shift + click-and-move"),
        new ShortcutItem("Tilt horizontally (slide with 2 fingers)", "Ctrl+Shift + click-and-move"),
        new ShortcutItem("Drag & drop APK file", "Install APK from computer"),
        new ShortcutItem("Drag & drop non-APK file", "Push file to device"),
    };

    private void TrySetWindowIcon()
    {
        try
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var iconPath = Path.Combine(AppContext.BaseDirectory, "icon", "favicon.ico");
            if (File.Exists(iconPath))
            {
                appWindow.SetIcon(iconPath);
            }
        }
        catch
        {
        }
    }

    private void TrySetTitleBarToMatchBackground()
    {
        try
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var titleBar = appWindow.TitleBar;

            titleBar.ExtendsContentIntoTitleBar = true;
            titleBar.BackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
            titleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
        }
        catch
        {
        }
    }

    private void UpdateCommandPreview()
    {
        if (_customCommandActive) return;

        var settings = BuildSettings();
        var builder = new ScrcpyCommandBuilder(settings);
        var args = builder.Build();

        try
        {
            _isUpdatingCommandPreview = true;
            CommandPreviewText.Text = $"scrcpy {args}";
        }
        finally
        {
            _isUpdatingCommandPreview = false;
        }
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
            AudioSource = AudioPanelControl.AudioEnabled ? AudioPanelControl.SelectedAudioSource : AudioSource.None,
            Fullscreen = FullscreenCheck.IsChecked == true,
            AlwaysOnTop = AlwaysOnTopCheck.IsChecked == true,
            Record = RecordToggle.IsOn,
            RecordFile = RecordFileBox.Text.Length > 0 ? RecordFileBox.Text : "record.mp4"
        };
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var args = CommandPreviewText.Text.Replace("scrcpy ", "").Trim();

        try
        {
            _runner.ForceStop();
            await Task.Delay(200);
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
        StatusBarText.Text = "Stopping...";
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
