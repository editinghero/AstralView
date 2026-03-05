namespace AstralView.Models
{
    public enum CameraFacing { Back, Front }
    public enum AudioSource { Output, Mic, None }
    public enum VideoCodec { H264, H265, AV1 }
    public class ScrcpySettings
    {
        public string? DeviceSerial { get; set; }

        public bool UseCamera { get; set; } = false;

        public int MaxSize { get; set; } = 0;
        public int BitRateMbps { get; set; } = 8;
        public int MaxFps { get; set; } = 60;
        public VideoCodec Codec { get; set; } = VideoCodec.H264;

        public CameraFacing CameraFacing { get; set; } = CameraFacing.Back;
        public string CameraSize { get; set; } = string.Empty;
        public string CameraId { get; set; } = string.Empty;
        public bool TurnScreenOff { get; set; } = false;

        public AudioSource AudioSource { get; set; } = AudioSource.Output;

        public bool Fullscreen { get; set; } = false;
        public bool AlwaysOnTop { get; set; } = false;
        public string WindowTitle { get; set; } = "AstralView";

        public bool Record { get; set; } = false;
        public string RecordFile { get; set; } = "record.mp4";
    }
}
