using System.Diagnostics;
using System.Text.RegularExpressions;
using AstralView.Models;

namespace AstralView.Services
{
    public class AdbService
    {
        private readonly string _adbPath;

        private static string NormalizeAddress(string address)
        {
            address = address.Trim();
            if (string.IsNullOrEmpty(address)) return address;

            return address.Contains(':') ? address : $"{address}:5555";
        }

        public AdbService(string adbPath)
        {
            _adbPath = adbPath;
        }

        public async Task<List<Device>> GetDevicesAsync()
        {
            var output = await RunCommandAsync("devices -l");
            var devices = new List<Device>();

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1))
            {
                var match = Regex.Match(line, @"^([^\s]+)\s+([^\s]+).+model:([^\s]+)");
                if (match.Success)
                {
                    devices.Add(new Device
                    {
                        Serial = match.Groups[1].Value,
                        State = match.Groups[2].Value,
                        Model = match.Groups[3].Value
                    });
                }
            }
            return devices;
        }

        public async Task EnableTcpIpAsync(string serial, int port = 5555)
        {
            if (port <= 0) port = 5555;
            await RunCommandAsync($"-s {serial} tcpip {port}");
        }

        public async Task<string> ConnectAsync(string address)
        {
            address = NormalizeAddress(address);
            return await RunCommandAsync($"connect {address}");
        }

        public async Task<string> DisconnectAsync(string address)
        {
            address = NormalizeAddress(address);
            return await RunCommandAsync($"disconnect {address}");
        }

        public async Task<string> ExecuteCommandAsync(string args)
        {
            return await RunCommandAsync(args);
        }

        private async Task<string> RunCommandAsync(string args)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _adbPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null) return string.Empty;
                return await process.StandardOutput.ReadToEndAsync();
            }
            catch { return "ADB error"; }
        }
    }
}
