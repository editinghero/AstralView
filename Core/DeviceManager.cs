using AstralView.Models;
using AstralView.Services;
using System.Collections.ObjectModel;

namespace AstralView.Core
{
    public class DeviceManager
    {
        private readonly AdbService _adb;
        public ObservableCollection<Device> Devices { get; } = new();

        public DeviceManager(AdbService adb)
        {
            _adb = adb;
        }

        public async Task RefreshAsync()
        {
            var list = await _adb.GetDevicesAsync();
            Devices.Clear();
            foreach (var d in list) Devices.Add(d);
        }
    }
}
