using System.IO;
using System.Windows.Input;
using Autofac;
using Avalonia.Controls;
using Loqui;
using Newtonsoft.Json;
using Noggog;
using Noggog.UI;

namespace Mutagen.Bethesda.Tests.GUI.Views;

public partial class MainWindow : Window
{
    private const string SettingsPath = "Settings.json";

    private readonly MainVM _vm;

    public MainWindow()
    {
        InitializeComponent();

        Task.Run(LoquiRegistration.SpinUp).FireAndForget();

        _vm = LoadOrCreate(BuildContainer().Resolve<MainVM>());
        DataContext = _vm;

        Closed += (_, _) =>
        {
            try
            {
                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(_vm, Formatting.Indented));
            }
            catch
            {
                // Best-effort persistence on shutdown.
            }
            _vm.Dispose();
        };

        if (Environment.GetCommandLineArgs().Contains("Start")
            && _vm.RunAllCommand is ICommand runAll
            && runAll.CanExecute(null))
        {
            runAll.Execute(null);
        }
    }

    private IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<TestRunnerModule>();
        builder.RegisterInstance<IPathPickerDialogProvider>(new AvaloniaPathPickerDialogProvider(() => this));
        return builder.Build();
    }

    private static MainVM LoadOrCreate(MainVM vm)
    {
        if (File.Exists(SettingsPath))
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(SettingsPath), vm);
                return vm;
            }
            catch
            {
                // Fall through to a fresh VM if the settings file is unreadable.
            }
        }

        vm.FreshInitialize();
        return vm;
    }
}
