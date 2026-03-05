namespace AstralView.Models
{
    public class Device
    {
        public string Serial { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        public override string ToString() =>
            string.IsNullOrEmpty(Model) ? Serial : $"{Model} ({Serial})";
    }
}
