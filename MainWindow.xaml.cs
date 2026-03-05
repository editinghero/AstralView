using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

namespace AstralView;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        Title = "AstralView";
    }
}
