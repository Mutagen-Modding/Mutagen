using ReactiveUI;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
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
using Noggog.WPF;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.Tests.GUI.Views;

public class DataFolderViewBase : ReactiveUserControl<DataFolderVM> { }

/// <summary>
/// Interaction logic for DataFolderView.xaml
/// </summary>
public partial class DataFolderView : DataFolderViewBase
{
    public DataFolderView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyFallback(x => x.ViewModel!.GameRelease)
                .Select(x => x.ToString())
                .BindTo(this, x => x.Name.Text)
                .DisposeWith(disposable);
            this.Bind(this.ViewModel, vm => vm.DataFolder, v => v.DataFolderPicker.PickerVM)
                .DisposeWith(disposable);
        });
    }
}