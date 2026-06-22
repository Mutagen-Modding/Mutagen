using System.Windows;
using ReactiveUI.Builder;

namespace Mutagen.Bethesda.Tests.GUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .Build();
    }
}