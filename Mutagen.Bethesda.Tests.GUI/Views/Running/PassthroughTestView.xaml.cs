using Noggog;
using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    public class PassthroughTestViewBase : ReactiveUserControl<PassthroughTestVM> { }

    /// <summary>
    /// Interaction logic for PassthroughTestView.xaml
    /// </summary>
    public partial class PassthroughTestView : PassthroughTestViewBase
    {
        public PassthroughTestView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAny(x => x.ViewModel.Name)
                    .BindToStrict(this, x => x.Name.Text)
                    .DisposeWith(disposable);
                this.TopBorder.Events().MouseUp
                    .Unit()
                    .InvokeCommandStrict(this, x => x.ViewModel.SelectCommand)
                    .DisposeWith(disposable);
                this.WhenAny(x => x.ViewModel.TimeSpent)
                    .Select(x =>
                    {
                        if (x == null) return null;
                        return $"{x.Value.TotalMinutes:n2}m";
                    })
                    .BindToStrict(this, x => x.TimeSpent.Text)
                    .DisposeWith(disposable);
            });
        }
    }
}
