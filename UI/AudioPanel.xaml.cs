using AstralView.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AstralView.UI;

public sealed partial class AudioPanel : UserControl
{
    public AudioSource SelectedAudioSource { get; private set; } = AudioSource.Output;

    public event EventHandler? SettingsChanged;

    public bool AudioEnabled => AudioToggle?.IsOn == true;

    public AudioPanel()
    {
        this.InitializeComponent();
    }

    public void SetAudioEnabled(bool enabled)
    {
        if (AudioToggle == null) return;
        AudioToggle.IsOn = enabled;
    }

    private void AudioToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (AudioSourceCombo == null || AudioOptions == null) return;

        bool isEnabled = AudioToggle.IsOn;
        
        AudioSourceCombo.IsEnabled = isEnabled;
        AudioOptions.Opacity = isEnabled ? 1.0 : 0.5;

        if (!isEnabled)
            SelectedAudioSource = AudioSource.None;
        else
            UpdateAudioSource();

        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AudioSourceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateAudioSource();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateAudioSource()
    {
        if (AudioSourceCombo == null) return;

        SelectedAudioSource = AudioSourceCombo.SelectedIndex == 1
            ? AudioSource.Mic
            : AudioSource.Output;
    }
}
