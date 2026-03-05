using AstralView.Models;

namespace AstralView.Services
{
    public static class PresetService
    {
        public static ScrcpySettings GamingMode() => new()
        {
            MaxSize = 1080,
            BitRateMbps = 16,
            MaxFps = 120,
            Codec = VideoCodec.H265
        };

        public static ScrcpySettings RecordingMode() => new()
        {
            MaxSize = 0,
            BitRateMbps = 20,
            MaxFps = 60,
            Record = true,
            RecordFile = "high_quality_record.mp4"
        };

        public static ScrcpySettings StreamingMode() => new()
        {
            MaxSize = 1080,
            BitRateMbps = 8,
            MaxFps = 60
        };

        public static ScrcpySettings LowBandwidthMode() => new()
        {
            MaxSize = 720,
            BitRateMbps = 2,
            MaxFps = 30
        };
    }
}
