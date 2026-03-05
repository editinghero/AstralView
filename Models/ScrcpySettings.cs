namespace AstralView.Models
{
    public enum CameraFacing { Back, Front }
    public enum AudioSource { Output, Mic, None }
    public enum VideoCodec { H264, H265, AV1 }

    /// <summary>
    /// Holds all scrcpy configuration settings bound to the UI.
    /// </summary>
    public class ScrcpySettings
    {
        // Device
        public string? DeviceSerial { get; set; }

        // Video
        public bool UseCamera { get; set; } = false;
        public int MaxSize { get; set; } = 0;          // 0 = no limit
        public int BitRateMbps { get; set; } = 8;
        public int MaxFps { get; set; } = 60;
        public VideoCodec Codec { get; set; } = VideoCodec.H264;

        // Camera
        public CameraFacing CameraFacing { get; set; } = CameraFacing.Back;
        public string CameraSize { get; set; } = string.Empty;
        public string CameraId { get; set; } = string.Empty;
        public bool TurnScreenOff { get; set; } = false;

        // Audio
        public AudioSource AudioSource { get; set; } = AudioSource.Output;

        // Window
        public bool Fullscreen { get; set; } = false;
        public bool AlwaysOnTop { get; set; } = false;
        public string WindowTitle { get; set; } = "AstralView";

        // Recording
        public bool Record { get; set; } = false;
        public string RecordFile { get; set; } = "record.mp4";
    }
}
