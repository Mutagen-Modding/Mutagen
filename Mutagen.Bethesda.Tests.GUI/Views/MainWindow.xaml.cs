using Loqui;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Noggog;
using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(LoquiRegistration.SpinUp).FireAndForget();
            this.WireMainVM<MainVM>($"Settings.json");
        }
    }
}
