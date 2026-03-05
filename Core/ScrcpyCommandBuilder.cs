using AstralView.Models;
using System.Text;

namespace AstralView.Core
{
    /// <summary>
    /// Builds scrcpy CLI arguments from a ScrcpySettings instance.
    /// </summary>
    public class ScrcpyCommandBuilder
    {
        private readonly ScrcpySettings _settings;

        public ScrcpyCommandBuilder(ScrcpySettings settings)
        {
            _settings = settings;
        }

        public string Build()
        {
            var args = new StringBuilder();

            // Device selection
            if (!string.IsNullOrEmpty(_settings.DeviceSerial))
                args.Append($"-s {_settings.DeviceSerial} ");

            // Video source
            if (_settings.UseCamera)
            {
                args.Append("--video-source=camera ");

                if (_settings.CameraFacing == CameraFacing.Front)
                    args.Append("--camera-facing=front ");
                else
                    args.Append("--camera-facing=back ");

                if (!string.IsNullOrEmpty(_settings.CameraSize))
                    args.Append($"--camera-size={_settings.CameraSize} ");

                if (!string.IsNullOrEmpty(_settings.CameraId))
                    args.Append($"--camera-id={_settings.CameraId} ");
            }

            // Resolution
            if (_settings.MaxSize > 0)
                args.Append($"--max-size={_settings.MaxSize} ");

            // Bitrate
            args.Append($"--video-bit-rate={_settings.BitRateMbps}M ");

            // FPS
            if (_settings.MaxFps > 0)
                args.Append($"--max-fps={_settings.MaxFps} ");

            // Codec
            args.Append($"--video-codec={_settings.Codec.ToString().ToLower()} ");

            // Audio
            switch (_settings.AudioSource)
            {
                case AudioSource.Output:
                    args.Append("--audio-source=output ");
                    break;
                case AudioSource.Mic:
                    args.Append("--audio-source=mic ");
                    break;
                case AudioSource.None:
                    args.Append("--no-audio ");
                    break;
            }

            // Window
            if (_settings.Fullscreen)
                args.Append("--fullscreen ");

            if (_settings.AlwaysOnTop)
                args.Append("--always-on-top ");

            if (!string.IsNullOrEmpty(_settings.WindowTitle))
                args.Append($"--window-title=\"{_settings.WindowTitle}\" ");

            // Recording
            if (_settings.Record && !string.IsNullOrEmpty(_settings.RecordFile))
                args.Append($"--record={_settings.RecordFile} ");

            return args.ToString().Trim();
        }
    }
}
