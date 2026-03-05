using AstralView.Services;
using System.Threading.Tasks;

namespace AstralView.Core
{
    /// <summary>
    /// Manages ADB wireless connection workflows.
    /// </summary>
    public class WirelessManager
    {
        private readonly AdbService _adb;

        public WirelessManager(AdbService adb)
        {
            _adb = adb;
        }

        /// <summary>
        /// Enables TCP/IP mode on the specified device and connects over WiFi.
        /// </summary>
        public async Task<string> ConnectAsync(string serial, string ipAddress)
        {
            await _adb.EnableTcpIpAsync(serial);
            await Task.Delay(500); // Allow mode switch
            return await _adb.ConnectAsync(ipAddress);
        }

        /// <summary>
        /// Disconnects from the specified WiFi device.
        /// </summary>
        public async Task<string> DisconnectAsync(string ipAddress)
        {
            return await _adb.DisconnectAsync(ipAddress);
        }
    }
}
