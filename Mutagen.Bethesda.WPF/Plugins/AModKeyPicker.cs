using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace Mutagen.Bethesda.WPF.Plugins
{
    [TemplatePart(Name = "PART_ModKeyBox", Type = typeof(ModKeyBox))]
    public class AModKeyPicker : NoggogControl
    {
        public ModKey ModKey
        {
            get => (ModKey)GetValue(ModKeyProperty);
            set => SetValue(ModKeyProperty, value);
        }
        public static readonly DependencyProperty ModKeyProperty = DependencyProperty.Register(nameof(ModKey), typeof(ModKey), typeof(AModKeyPicker),
             new FrameworkPropertyMetadata(default(ModKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(AModKeyPicker),
             new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool InSearchMode
        {
            get => (bool)GetValue(InSearchModeProperty);
            set => SetValue(InSearchModePropertyKey, value);
        }
        public static readonly DependencyPropertyKey InSearchModePropertyKey = DependencyProperty.RegisterReadOnly(nameof(InSearchMode), typeof(bool), typeof(AModKeyPicker),
             new FrameworkPropertyMetadata(default(bool)));
        public static readonly DependencyProperty InSearchModeProperty = InSearchModePropertyKey.DependencyProperty;

        public bool Processing
        {
            get => (bool)GetValue(ProcessingProperty);
            set => SetValue(ProcessingProperty, value);
        }
        public static readonly DependencyProperty ProcessingProperty = DependencyProperty.Register(nameof(Processing), typeof(bool), typeof(AModKeyPicker),
             new FrameworkPropertyMetadata(default(bool)));

        public bool AllowsSearchMode
        {
            get => (bool)GetValue(AllowsSearchModeProperty);
            set => SetValue(AllowsSearchModeProperty, value);
        }
        public static readonly DependencyProperty AllowsSearchModeProperty = DependencyProperty.Register(nameof(AllowsSearchMode), typeof(bool), typeof(AModKeyPicker),
             new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object SearchableMods
        {
            get => (object)GetValue(SearchableModsProperty);
            set => SetValue(SearchableModsProperty, value);
        }
        public static readonly DependencyProperty SearchableModsProperty = DependencyProperty.Register(nameof(SearchableMods), typeof(object), typeof(AModKeyPicker),
             new FrameworkPropertyMetadata(default(object)));

        public IEnumerable<ModKey> ApplicableMods
        {
            get => (IEnumerable<ModKey>)GetValue(ApplicableModsProperty);
            set => SetValue(ApplicableModsPropertyKey, value);
        }
        public static readonly DependencyPropertyKey ApplicableModsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ApplicableMods), typeof(IEnumerable<ModKey>), typeof(AModKeyPicker),
             new FrameworkPropertyMetadata(default(IEnumerable<ModKey>)));
        public static readonly DependencyProperty ApplicableModsProperty = ApplicableModsPropertyKey.DependencyProperty;

        public ICommand PickerClickCommand
        {
            get => (ICommand)GetValue(PickerClickCommandProperty);
            set => SetValue(PickerClickCommandProperty, value);
        }
        public static readonly DependencyProperty PickerClickCommandProperty = DependencyProperty.Register(nameof(PickerClickCommand), typeof(ICommand), typeof(AModKeyPicker),
             new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static AModKeyPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AModKeyPicker), new FrameworkPropertyMetadata(typeof(AModKeyPicker)));
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            ApplicableMods = this.WhenAnyValue(x => x.SearchableMods)
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .Select(lo =>
                {
                    switch (lo)
                    {
                        case IObservable<IChangeSet<IModListingGetter>> liveLo:
                            return liveLo.Transform(x => x.ModKey);
                        case IObservable<IChangeSet<ModKey>> liveLoKeys:
                            return liveLoKeys;
                        case IEnumerable<IModListingGetter> modListings:
                            return modListings.Select(l => l.ModKey).AsObservableChangeSet();
                        case IEnumerable<ModKey> modKeys:
                            return modKeys.AsObservableChangeSet();
                        case ILoadOrderGetter loGetter:
                            return loGetter.ListedOrder.AsObservableChangeSet();
                        default:
                            return Observable.Return<IChangeSet<ModKey>>(ChangeSet<ModKey>.Empty);
                    }
                })
                .FilterSwitch(this.WhenAnyValue(x => x.InSearchMode), Observable.Return<IChangeSet<ModKey>>(ChangeSet<ModKey>.Empty))
                .Switch()
                .ObserveOnGui()
                .Filter(this.WhenAnyValue(x => x.FileName)
                    .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select<string, Func<ModKey, bool>>(term => (modKey) =>
                    {
                        return term.IsNullOrWhitespace() || modKey.ToString().Contains(term, StringComparison.OrdinalIgnoreCase);
                    }))
                .ObserveOnGui()
                .ToObservableCollection(this._unloadDisposable);

            this.WhenAnyValue(x => x.AllowsSearchMode)
                .Where(x => !x)
                .Subscribe(_ => InSearchMode = false)
                .DisposeWith(_unloadDisposable);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InSearchMode = false;
            var modKeyBox = GetTemplateChild("PART_ModKeyBox") as ModKeyBox;
            if (modKeyBox != null)
            {
                modKeyBox.WhenAnyValue(x => x.FileName)
                    .Skip(1)
                    .FilterSwitch(modKeyBox.WhenAnyValue(x => x.IsKeyboardFocusWithin))
                    .Subscribe(_ =>
                    {
                        this.InSearchMode = true;
                    })
                    .DisposeWith(_templateDisposable);
                modKeyBox.WhenAnyValue(x => x.IsKeyboardFocusWithin)
                    .DistinctUntilChanged()
                    .Where(focused => focused)
                    .Subscribe(_ =>
                    {
                        this.InSearchMode = true;
                    })
                    .DisposeWith(_templateDisposable);
                this.Events().IsKeyboardFocusWithinChanged
                    .Where(x => !((bool)x.NewValue))
                    .Delay(TimeSpan.FromMilliseconds(150), RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        this.InSearchMode = false;
                    })
                    .DisposeWith(_templateDisposable);
            }
        }
    }
}
