using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
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

namespace Mutagen.Bethesda.Tests.GUI.Views;

public class GroupTestViewBase : ReactiveUserControl<GroupTestVM> { }

/// <summary>
/// Interaction logic for GroupTestView.xaml
/// </summary>
public partial class GroupTestView : GroupTestViewBase
{
    public GroupTestView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyFallback(x => x.ViewModel!.Name)
                .BindTo(this, x => x.Name.Text)
                .DisposeWith(disposable);
            this.WhenAnyFallback(x => x.ViewModel!.PassthroughDisplay)
                .BindTo(this, x => x.PassthroughsControl.ItemsSource)
                .DisposeWith(disposable);
        });
    }
}