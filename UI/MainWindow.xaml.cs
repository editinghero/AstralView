using AstralView.Core;
using AstralView.Models;
using AstralView.Services;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AstralView;

public sealed partial class MainWindow : Window
{
    private readonly ScrcpyRunner _runner;
    private readonly WirelessManager _wirelessManager;

    public MainWindow()
    {
        this.InitializeComponent();

        SystemBackdrop = new MicaBackdrop();
        Title = "AstralView";
        // ExtendsContentIntoTitleBar = true; // Disabled for compatibility with generic builds


        var adb = new AdbService(ScrcpyPaths.AdbPath);
        _runner = new ScrcpyRunner(ScrcpyPaths.ScrcpyPath);
        _wirelessManager = new WirelessManager(adb);

        // Keep command preview updated whenever any combobox or slider changes
        UpdateCommandPreview();
    }

    // ─── Build Settings from All Panels ─────────────────────────────────────

    private ScrcpySettings BuildSettings()
    {
        var settings = new ScrcpySettings
        {
            DeviceSerial  = DevicePanelControl.SelectedDevice?.Serial,
            UseCamera     = CameraPanelControl.CameraEnabled,
            CameraFacing  = CameraPanelControl.Facing,
            CameraSize    = CameraPanelControl.CameraSize,
            CameraId      = CameraPanelControl.CameraId,
            MaxSize       = VideoPanelControl.MaxSize,
            BitRateMbps   = VideoPanelControl.BitRate,
            MaxFps        = VideoPanelControl.MaxFps,
            Codec         = VideoPanelControl.Codec,
            AudioSource   = AudioPanelControl.SelectedAudioSource,
            Fullscreen    = FullscreenCheck.IsChecked == true,
            AlwaysOnTop   = AlwaysOnTopCheck.IsChecked == true,
            Record        = RecordToggle.IsOn,
            RecordFile    = RecordFileBox.Text.Length > 0 ? RecordFileBox.Text : "record.mp4"
        };
        return settings;
    }

    private void UpdateCommandPreview()
    {
        var settings = BuildSettings();
        var builder = new ScrcpyCommandBuilder(settings);
        CommandPreviewText.Text = $"scrcpy {builder.Build()}";
    }

    // ─── Start / Stop ────────────────────────────────────────────────────────

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (_runner.IsRunning)
        {
            StatusBarText.Text = "Already running.";
            return;
        }

        var settings = BuildSettings();
        var builder  = new ScrcpyCommandBuilder(settings);
        var args     = builder.Build();

        try
        {
            _runner.Start(args);
            StatusBarText.Text     = "scrcpy started";
            StartButton.IsEnabled  = false;
            StopButton.IsEnabled   = true;
        }
        catch (Exception ex)
        {
            StatusBarText.Text = $"Failed to start: {ex.Message}";
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        _runner.Stop();
        StatusBarText.Text    = "Stopped";
        StartButton.IsEnabled = true;
        StopButton.IsEnabled  = false;
    }

    // ─── Wireless ────────────────────────────────────────────────────────────

    private async void WirelessConnect_Click(object sender, RoutedEventArgs e)
    {
        var ip     = IpAddressBox.Text.Trim();
        var serial = DevicePanelControl.SelectedDevice?.Serial ?? string.Empty;

        if (string.IsNullOrEmpty(ip))
        {
            WirelessStatusText.Text = "Please enter an IP address.";
            return;
        }

        WirelessStatusText.Text = "Connecting...";
        var result = await _wirelessManager.ConnectAsync(serial, ip);
        WirelessStatusText.Text = result.Trim();
    }

    private async void WirelessDisconnect_Click(object sender, RoutedEventArgs e)
    {
        var ip = IpAddressBox.Text.Trim();
        if (string.IsNullOrEmpty(ip)) return;
        var result = await _wirelessManager.DisconnectAsync(ip);
        WirelessStatusText.Text = result.Trim();
    }

    // ─── Presets ─────────────────────────────────────────────────────────────

    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        var tag = btn.Tag?.ToString();

        ScrcpySettings preset = tag switch
        {
            "Gaming"       => PresetService.GamingMode(),
            "Recording"    => PresetService.RecordingMode(),
            "Streaming"    => PresetService.StreamingMode(),
            "LowBandwidth" => PresetService.LowBandwidthMode(),
            _              => new ScrcpySettings()
        };

        ApplyPreset(preset);
        UpdateCommandPreview();
        StatusBarText.Text = $"{tag} preset applied";
    }

    private void ApplyPreset(ScrcpySettings preset)
    {
        // Video
        VideoPanelControl.SetBitrate(preset.BitRateMbps);


        // Audio
        RecordToggle.IsOn = preset.Record;
        if (!string.IsNullOrEmpty(preset.RecordFile))
            RecordFileBox.Text = preset.RecordFile;
    }
}
