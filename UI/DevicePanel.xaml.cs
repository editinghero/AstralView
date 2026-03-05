using AstralView.Core;
using AstralView.Models;
using AstralView.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AstralView.UI;

public sealed partial class DevicePanel : UserControl
{
    private AdbService? _adb;
    private DeviceManager? _deviceManager;

    public Device? SelectedDevice { get; private set; }

    public DevicePanel()
    {
        this.InitializeComponent();
    }

    public void Initialize(AdbService adb, DeviceManager dm)
    {
        _adb = adb;
        _deviceManager = dm;
        DeviceComboBox.ItemsSource = _deviceManager.Devices;
        _ = LoadDevicesAsync();
    }

    private async Task LoadDevicesAsync()
    {
        if (_deviceManager == null) return;
        
        RefreshButton.IsEnabled = false;
        DeviceStatusText.Text = "Scanning...";

        await _deviceManager.RefreshAsync();

        if (_deviceManager.Devices.Count > 0)
        {
            DeviceComboBox.SelectedIndex = 0;
            DeviceStatusText.Text = $"{_deviceManager.Devices.Count} device(s)";
        }
        else
        {
            DeviceStatusText.Text = "No devices";
        }

        RefreshButton.IsEnabled = true;
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        _ = LoadDevicesAsync();
    }

    private void DeviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedDevice = DeviceComboBox.SelectedItem as Device;
    }
}
