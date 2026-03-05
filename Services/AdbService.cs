using AstralView.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace AstralView.Services
{
    /// <summary>
    /// Wraps ADB commands to list and manage Android devices.
    /// </summary>
    public class AdbService
    {
        private readonly string _adbPath;

        public AdbService(string adbPath = "adb")
        {
            _adbPath = adbPath;
        }

        /// <summary>
        /// Returns a list of connected Android devices.
        /// </summary>
        public async Task<List<Device>> GetDevicesAsync()
        {
            var output = await RunAdbAsync("devices -l");
            var devices = new List<Device>();

            foreach (var line in output.Split('\n'))
            {
                if (line.StartsWith("List") || string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                var device = new Device { Serial = parts[0], State = parts[1] };

                // Parse "model:" token
                foreach (var part in parts)
                    if (part.StartsWith("model:"))
                        device.Model = part.Replace("model:", "").Replace("_", " ");

                devices.Add(device);
            }

            return devices;
        }

        /// <summary>
        /// Enables TCP/IP mode for wireless debugging on port 5555.
        /// </summary>
        public async Task<string> EnableTcpIpAsync(string serial)
        {
            return await RunAdbAsync($"-s {serial} tcpip 5555");
        }

        /// <summary>
        /// Connects to a device over WiFi.
        /// </summary>
        public async Task<string> ConnectAsync(string ipAddress)
        {
            return await RunAdbAsync($"connect {ipAddress}:5555");
        }

        /// <summary>
        /// Disconnects a wireless device.
        /// </summary>
        public async Task<string> DisconnectAsync(string ipAddress)
        {
            return await RunAdbAsync($"disconnect {ipAddress}:5555");
        }

        private async Task<string> RunAdbAsync(string arguments)
        {
            var psi = new ProcessStartInfo(_adbPath, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            var sb = new StringBuilder();
            sb.Append(await process.StandardOutput.ReadToEndAsync());
            sb.Append(await process.StandardError.ReadToEndAsync());
            await process.WaitForExitAsync();
            return sb.ToString();
        }
    }
}
