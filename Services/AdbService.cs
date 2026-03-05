using System.Diagnostics;
using System.Text.RegularExpressions;
using AstralView.Models;

namespace AstralView.Services
{
    public class AdbService
    {
        private readonly string _adbPath;

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

        public async Task EnableTcpIpAsync(string serial)
        {
            await RunCommandAsync($"-s {serial} tcpip 5555");
        }

        public async Task<string> ConnectAsync(string ip)
        {
            return await RunCommandAsync($"connect {ip}:5555");
        }

        public async Task<string> DisconnectAsync(string ip)
        {
            return await RunCommandAsync($"disconnect {ip}:5555");
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
