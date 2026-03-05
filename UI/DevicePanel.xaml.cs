using AstralView.Core;
using AstralView.Models;
using AstralView.Services;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AstralView.UI;

public sealed partial class DevicePanel : Page
{
    private readonly AdbService _adb;
    private readonly DeviceManager _deviceManager;

    public Device? SelectedDevice { get; private set; }

    public DevicePanel()
    {
        this.InitializeComponent();
        _adb = new AdbService(ScrcpyPaths.AdbPath);
        _deviceManager = new DeviceManager(_adb);
        _ = LoadDevicesAsync();
    }

    private async Task LoadDevicesAsync()
    {
        RefreshButton.IsEnabled = false;
        DeviceStatusText.Text = "Scanning...";

        await _deviceManager.RefreshAsync();

        DeviceComboBox.ItemsSource = null;
        DeviceComboBox.ItemsSource = _deviceManager.Devices;

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

    private async void RefreshButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await LoadDevicesAsync();
    }

    private void DeviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedDevice = DeviceComboBox.SelectedItem as Device;
    }
}
