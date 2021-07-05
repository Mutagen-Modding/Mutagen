using System.Diagnostics;
using StructureMap;

namespace Mutagen.Bethesda.WPF.TestDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var container = Container.For<Register>();
            Debug.WriteLine(container.WhatDoIHave());
            this.DataContext = container.GetInstance<MainVM>();
        }
    }
}
