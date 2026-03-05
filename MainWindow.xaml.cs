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
        
        // Listen to camera toggle to disable video
        CameraPanelControl.CameraToggleSwitch.Toggled += (s, e) =>
        {
            VideoPanelControl.VideoOptions.IsEnabled = !CameraPanelControl.CameraEnabled;
            VideoPanelControl.VideoOptions.Opacity = CameraPanelControl.CameraEnabled ? 0.5 : 1.0;
            UpdateCommandPreview();
        };
        
        UpdateCommandPreview();
    }

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

        var address = string.IsNullOrEmpty(port) ? ip : $"{ip}:{port}";
        
        WirelessStatusText.Text = "Connecting...";
        var result = await _wirelessManager.ConnectAsync(serial, address);
        WirelessStatusText.Text = result.Trim();
        
        await Task.Delay(1000);
        DevicePanelControl.RefreshDevices();
    }

    private async void WirelessDisconnect_Click(object sender, RoutedEventArgs e)
    {
        var ip = IpAddressBox.Text.Trim();
        var port = PortBox.Text.Trim();
        
        if (string.IsNullOrEmpty(ip)) return;
        
        var address = string.IsNullOrEmpty(port) ? ip : $"{ip}:{port}";
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
