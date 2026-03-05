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

    public event EventHandler? SettingsChanged;

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

        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void FacingCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Facing = FacingCombo.SelectedIndex == 1 ? CameraFacing.Front : CameraFacing.Back;

        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void CameraSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CameraSizeCombo.SelectedItem is ComboBoxItem item)
            CameraSize = item.Content?.ToString() ?? "1920x1080";

        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void CameraIdBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        CameraId = CameraIdBox.Text;

        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnScreenOffCheck_Checked(object sender, RoutedEventArgs e)
    {
        TurnScreenOff = true;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnScreenOffCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        TurnScreenOff = false;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
}
