using System;
using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.WPF.Plugins.Order;

public class LoadOrderListingViewBase : NoggogUserControl<IModListingGetter> { }

public partial class LoadOrderListingView : LoadOrderListingViewBase
{
    public LoadOrderListingView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyValue(x => x.ViewModel!.ModKey)
                .Select(m => (object) m)
                .BindTo(this, x => x.CheckBox.Content)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel)
                .Select(x => x is IModListing)
                .BindTo(this, x => x.CheckBox.IsEnabled)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.Enabled)
                .DistinctUntilChanged()
                .Select(x => (bool?) x)
                .BindTo(this, x => x.CheckBox.IsChecked)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel)
                .Select(x =>
                {
                    if (x is IModListing setter)
                    {
                        return this.WhenAnyValue(x => x.CheckBox.IsChecked)
                            .DistinctUntilChanged()
                            .Skip(1)
                            .Do(enabled => setter.Enabled = enabled ?? false);
                    }
                    else
                    {
                        return Observable.Empty<bool?>();
                    }
                })
                .Switch()
                .Subscribe()
                .DisposeWith(disposable);
        });
    }
}