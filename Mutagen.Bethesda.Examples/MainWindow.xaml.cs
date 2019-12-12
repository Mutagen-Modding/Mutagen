using Loqui;
using MahApps.Metro.Controls;
using Noggog;
using Noggog.WPF;
using System;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() => LoquiRegistration.SpinUp()).FireAndForget();
            this.WireMainVM(
                new MainVM(this),
                System.IO.Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), $"Mutagen Examples/Settings.xml"),
                load: (s, vm) => vm.CopyInFromXml(s),
                save: (s, vm) => vm.WriteToXml(s, "MutagenExamples"));
        }
    }
}
