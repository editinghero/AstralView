using AstralView.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AstralView.UI;

public sealed partial class AudioPanel : UserControl
{
    public AudioSource SelectedAudioSource { get; private set; } = AudioSource.Output;

    public AudioPanel()
    {
        this.InitializeComponent();
    }

    private void AudioToggle_Toggled(object sender, RoutedEventArgs e)
    {
        // Add safety check for InitializeComponent race conditions
        if (AudioSourceCombo == null || AudioOptions == null) return;

        bool isEnabled = AudioToggle.IsOn;
        
        AudioSourceCombo.IsEnabled = isEnabled;
        AudioOptions.Opacity = isEnabled ? 1.0 : 0.5;

        if (!isEnabled)
            SelectedAudioSource = AudioSource.None;
        else
            UpdateAudioSource();
    }

    private void AudioSourceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateAudioSource();
    }

    private void UpdateAudioSource()
    {
        if (AudioSourceCombo == null) return;

        SelectedAudioSource = AudioSourceCombo.SelectedIndex == 1
            ? AudioSource.Mic
            : AudioSource.Output;
    }
}
