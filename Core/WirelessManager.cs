using AstralView.Services;

namespace AstralView.Core
{
    public class WirelessManager
    {
        private readonly AdbService _adb;

        public WirelessManager(AdbService adb)
        {
            _adb = adb;
        }

        public async Task<string> ConnectAsync(string serial, string ipAddress, int port = 5555)
        {
            if (!string.IsNullOrEmpty(serial))
            {
                await _adb.EnableTcpIpAsync(serial, port);
                await Task.Delay(500); 
            }
            return await _adb.ConnectAsync(ipAddress);
        }

        public async Task<string> DisconnectAsync(string ipAddress)
        {
            return await _adb.DisconnectAsync(ipAddress);
        }
    }
}
