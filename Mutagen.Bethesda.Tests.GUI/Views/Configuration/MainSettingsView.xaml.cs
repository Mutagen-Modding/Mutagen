using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    public class MainSettingsViewBase : ReactiveUserControl<MainVM> { }

    /// <summary>
    /// Interaction logic for MainSettingsView.xaml
    /// </summary>
    public partial class MainSettingsView : MainSettingsViewBase
    {
        public MainSettingsView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.Bind(this.ViewModel, x => x.TestNormal, x => x.TestNormal.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.TestOverlay, x => x.TestOverlay.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.TestImport, x => x.TestImport.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.TestCopyIn, x => x.TestCopyIn.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.TestEquals, x => x.TestEquals.IsChecked)
                    .DisposeWith(disposable);

                this.Bind(this.ViewModel, x => x.CacheDecompression, x => x.CacheDecompression.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.CacheAlignment, x => x.CacheAlignment.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.CacheProcessing, x => x.CacheProcessing.IsChecked)
                    .DisposeWith(disposable);

                this.WhenAnyFallback(x => x.ViewModel!.DataFoldersDisplay)
                    .BindTo(this, x => x.DataFolders.ItemsSource)
                    .DisposeWith(disposable);
            });
        }
    }
}
