using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.Tests.GUI.Views;

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
            this.Bind(this.ViewModel, x => x.TestParallel, x => x.TestParallel.IsChecked)
                .DisposeWith(disposable);
            this.Bind(ViewModel, vm => vm.TrimmingEnabled, x=> x.TrimmedGroupsEnable.IsChecked)
                .DisposeWith(disposable);
                
            this.OneWayBind(ViewModel, x => x.TrimmingEnabled, x => x.SkippedTypes.IsEnabled)
                .DisposeWith(disposable);

            this.Bind(this.ViewModel, x => x.CacheDecompression, x => x.CacheDecompression.IsChecked)
                .DisposeWith(disposable);
            this.Bind(this.ViewModel, x => x.CacheAlignment, x => x.CacheAlignment.IsChecked)
                .DisposeWith(disposable);
            this.Bind(this.ViewModel, x => x.CacheMerging, x => x.CacheMerging.IsChecked)
                .DisposeWith(disposable);
            this.Bind(this.ViewModel, x => x.CacheProcessing, x => x.CacheProcessing.IsChecked)
                .DisposeWith(disposable);
            this.Bind(this.ViewModel, x => x.CacheTrimming, x => x.CacheTrimming.IsChecked)
                .DisposeWith(disposable);
                
            this.OneWayBind(ViewModel, x => x.SkippedRecordTypes, x => x.SkippedTypes.ItemsSource)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.InterestingRecordTypes, x => x.IncludedTypes.ItemsSource)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.AddSkipCommand, x => x.AddSkipButton.Command)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.AddIncludeCommand, x => x.AddIncludeButton.Command)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.SkipInput, x => x.SkipInput.Text)
                .DisposeWith(disposable);

            this.WhenAnyFallback(x => x.ViewModel!.DataFoldersDisplay)
                .BindTo(this, x => x.DataFolders.ItemsSource)
                .DisposeWith(disposable);
        });
    }
}