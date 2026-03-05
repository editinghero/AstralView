using AstralView.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AstralView.UI;

public sealed partial class CameraPanel : UserControl
{
    public bool CameraEnabled { get; private set; } = false;
    public CameraFacing Facing { get; private set; } = CameraFacing.Back;
    public string CameraSize { get; private set; } = "1920x1080";
    public string CameraId { get; private set; } = string.Empty;
    public bool TurnScreenOff { get; private set; } = false;
    
    public ToggleSwitch CameraToggleSwitch => CameraToggle;

    public CameraPanel()
    {
        this.InitializeComponent();
    }

    private void CameraToggle_Toggled(object sender, RoutedEventArgs e)
    {
        CameraEnabled = CameraToggle.IsOn;
        
        FacingCombo.IsEnabled = CameraEnabled;
        CameraSizeCombo.IsEnabled = CameraEnabled;
        CameraIdBox.IsEnabled = CameraEnabled;
        TurnScreenOffCheck.IsEnabled = CameraEnabled;
        CameraOptions.Opacity = CameraEnabled ? 1.0 : 0.5;
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
