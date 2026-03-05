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
        AudioOptions.IsEnabled = AudioToggle.IsOn;
        if (!AudioToggle.IsOn)
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
        SelectedAudioSource = AudioSourceCombo.SelectedIndex == 1
            ? AudioSource.Mic
            : AudioSource.Output;
    }
}
