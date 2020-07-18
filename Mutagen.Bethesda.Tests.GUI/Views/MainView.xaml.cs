using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    public class MainViewBase : ReactiveUserControl<MainVM> { }

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MainViewBase
    {
        public MainView()
        {
            InitializeComponent();
        }
    }
}
