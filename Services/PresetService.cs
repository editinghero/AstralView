using AstralView.Models;

namespace AstralView.Services
{
    /// <summary>
    /// Provides preset ScrcpySettings configurations.
    /// </summary>
    public static class PresetService
    {
        public static ScrcpySettings GamingMode() => new()
        {
            MaxSize = 1024,
            BitRateMbps = 16,
            MaxFps = 120,
            AudioSource = AudioSource.Output
        };

        public static ScrcpySettings RecordingMode() => new()
        {
            MaxSize = 0,
            BitRateMbps = 20,
            MaxFps = 60,
            Record = true,
            RecordFile = "recording.mp4",
            AudioSource = AudioSource.Output
        };

        public static ScrcpySettings StreamingMode() => new()
        {
            MaxSize = 1280,
            BitRateMbps = 8,
            MaxFps = 30,
            AudioSource = AudioSource.Output
        };

        public static ScrcpySettings LowBandwidthMode() => new()
        {
            MaxSize = 720,
            BitRateMbps = 2,
            MaxFps = 30,
            AudioSource = AudioSource.Output
        };

        public static ScrcpySettings CameraMode() => new()
        {
            UseCamera = true,
            CameraFacing = CameraFacing.Back,
            CameraSize = "1920x1080",
            AudioSource = AudioSource.Mic
        };
    }
}
