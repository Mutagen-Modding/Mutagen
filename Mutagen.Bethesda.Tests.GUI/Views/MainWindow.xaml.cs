using System.IO;
using Autofac;
using Loqui;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Noggog;
using Noggog.UI;
using Noggog.WPF;

namespace Mutagen.Bethesda.Tests.GUI.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Task.Run(LoquiRegistration.SpinUp).FireAndForget();
        var vm = this.WireMainVM(
            $"Settings.json",
            BuildContainer().Resolve<MainVM>(),
            load: (path, m) => JsonConvert.PopulateObject(File.ReadAllText(path), m),
            save: (path, m) => File.WriteAllText(path, JsonConvert.SerializeObject(m, Formatting.Indented)),
            initialize: m => m.FreshInitialize());
        if (Environment.GetCommandLineArgs().Contains("Start"))
        {
            vm.RunAllCommand.Execute();
        }
    }

    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<TestRunnerModule>();
        builder.RegisterInstance<IPathPickerDialogProvider>(new WpfPathPickerDialogProvider());
        return builder.Build();
    }
}