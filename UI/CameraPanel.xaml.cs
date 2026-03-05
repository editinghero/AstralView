using AstralView.Models;
using Microsoft.UI.Xaml.Controls;

namespace AstralView.UI;

public sealed partial class CameraPanel : Page
{
    public bool CameraEnabled { get; private set; } = false;
    public CameraFacing Facing { get; private set; } = CameraFacing.Back;
    public string CameraSize { get; private set; } = "1920x1080";
    public string CameraId { get; private set; } = string.Empty;

    public CameraPanel()
    {
        this.InitializeComponent();
    }

    private void CameraToggle_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CameraEnabled = CameraToggle.IsOn;
        CameraOptions.IsEnabled = CameraEnabled;
    }

    private void FacingCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Facing = FacingCombo.SelectedIndex == 1 ? CameraFacing.Front : CameraFacing.Back;
    }

    private void CameraSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CameraSizeCombo.SelectedItem is ComboBoxItem item)
            CameraSize = item.Content?.ToString() ?? "1920x1080";
    }

    private void CameraIdBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        CameraId = CameraIdBox.Text;
    }
}
