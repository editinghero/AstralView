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

            if (_settings.UseCamera)
            {
                sb.Append("--video-source=camera ");
                
                if (!string.IsNullOrEmpty(_settings.CameraId))
                    sb.Append($"--camera-id={_settings.CameraId} ");
                else
                    sb.Append($"--camera-facing={_settings.CameraFacing.ToString().ToLower()} ");
                
                if (!string.IsNullOrEmpty(_settings.CameraSize))
                    sb.Append($"--camera-size={_settings.CameraSize} ");
                    
                if (_settings.TurnScreenOff)
                    sb.Append("--turn-screen-off ");
            }
            else
            {
                if (_settings.MaxSize > 0)
                    sb.Append($"--max-size={_settings.MaxSize} ");
            }

            sb.Append($"--video-bit-rate={_settings.BitRateMbps}M ");
            sb.Append($"--max-fps={_settings.MaxFps} ");
            
            var codec = _settings.Codec.ToString().ToLower();
            if (codec == "h264") codec = "h264";
            else if (codec == "h265") codec = "h265";
            else if (codec == "av1") codec = "av1";
            sb.Append($"--video-codec={codec} ");

            if (_settings.AudioSource == AudioSource.None) 
                sb.Append("--no-audio ");
            else if (_settings.AudioSource == AudioSource.Mic) 
                sb.Append("--audio-source=mic ");

            if (_settings.Fullscreen) sb.Append("--fullscreen ");
            if (_settings.AlwaysOnTop) sb.Append("--always-on-top ");

            if (_settings.Record)
                sb.Append($"--record=\"{_settings.RecordFile}\" ");

            return sb.ToString().Trim();
        }
    }
}
