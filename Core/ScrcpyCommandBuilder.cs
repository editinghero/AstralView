using System.Text;
using AstralView.Models;

namespace AstralView.Core
{
    public class ScrcpyCommandBuilder
    {
        private readonly ScrcpySettings _settings;

        public ScrcpyCommandBuilder(ScrcpySettings settings)
        {
            _settings = settings;
        }

        public string Build()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(_settings.DeviceSerial))
                sb.Append($"--serial {_settings.DeviceSerial} ");

            if (_settings.MaxSize > 0)
                sb.Append($"--max-size {_settings.MaxSize} ");

            sb.Append($"--video-bit-rate {_settings.BitRateMbps}M ");
            sb.Append($"--max-fps {_settings.MaxFps} ");

            if (_settings.UseCamera)
            {
                sb.Append("--camera-id ");
                sb.Append(string.IsNullOrEmpty(_settings.CameraId) ? "0 " : $"{_settings.CameraId} ");
                if (!string.IsNullOrEmpty(_settings.CameraSize))
                    sb.Append($"--camera-size {_settings.CameraSize} ");
            }

            if (_settings.AudioSource == AudioSource.None) sb.Append("--no-audio ");
            else if (_settings.AudioSource == AudioSource.Mic) sb.Append("--audio-source=mic ");

            if (_settings.Fullscreen) sb.Append("--fullscreen ");
            if (_settings.AlwaysOnTop) sb.Append("--always-on-top ");

            if (_settings.Record)
                sb.Append($"--record {_settings.RecordFile} ");

            return sb.ToString().Trim();
        }
    }
}
