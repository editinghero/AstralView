using AstralView.Models;
using AstralView.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AstralView.Core
{
    /// <summary>
    /// Manages the list of connected ADB devices.
    /// </summary>
    public class DeviceManager
    {
        private readonly AdbService _adb;

        public DeviceManager(AdbService adb)
        {
            _adb = adb;
        }

        public List<Device> Devices { get; private set; } = new();

        /// <summary>
        /// Refreshes the list of connected devices from ADB.
        /// </summary>
        public async Task RefreshAsync()
        {
            Devices = await _adb.GetDevicesAsync();
        }
    }
}
