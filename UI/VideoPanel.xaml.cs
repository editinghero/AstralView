using AstralView.Models;
using Microsoft.UI.Xaml.Controls;

namespace AstralView.UI;

public sealed partial class VideoPanel : UserControl
{
    public int MaxSize { get; private set; } = 0;
    public int BitRate { get; private set; } = 8;
    public int MaxFps { get; private set; } = 60;
    public VideoCodec Codec { get; private set; } = VideoCodec.H264;

    public VideoPanel()
    {
        this.InitializeComponent();
    }

    public void SetBitrate(int mbps)
    {
        BitrateSlider.Value = mbps;
    }

    private void ResolutionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ResolutionCombo.SelectedItem is ComboBoxItem item)
        {
            if (int.TryParse(item.Tag?.ToString(), out var result))
                MaxSize = result;
        }
    }

    private void BitrateSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        BitRate = (int)e.NewValue;
        if (BitrateLabel != null) BitrateLabel.Text = $"{BitRate} Mbps";
    }

    private void FpsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FpsCombo.SelectedItem is ComboBoxItem item)
        {
            if (int.TryParse(item.Tag?.ToString(), out var result))
                MaxFps = result;
        }
    }

    private void CodecCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CodecCombo.SelectedItem is ComboBoxItem item)
        {
            if (Enum.TryParse<VideoCodec>(item.Tag?.ToString(), out var result))
                Codec = result;
        }
    }
}
