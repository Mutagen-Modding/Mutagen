using Autofac;

namespace Mutagen.Bethesda.WPF.TestDisplay;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        var builder = new ContainerBuilder();
        builder.RegisterModule<MainModule>();
        this.DataContext = builder.Build().Resolve<MainVM>();
    }
}