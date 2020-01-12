using Loqui;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Noggog;
using Noggog.WPF;
using System;
using System.IO;
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
            this.WireMainVM<MainVM>(System.IO.Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), $"Mutagen Examples/Settings.json"));
        }
    }
}
