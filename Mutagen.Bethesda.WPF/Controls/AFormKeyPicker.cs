using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Collections;
using Noggog;
using System.Windows.Media;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using DynamicData;
using System.Windows.Controls;

namespace Mutagen.Bethesda.WPF
{
    [TemplatePart(Name = "PART_EditorIDBox", Type = typeof(TextBox))]
    public class AFormKeyPicker : NoggogControl
    {
        private bool _updating;

        public ILinkCache LinkCache
        {
            get => (ILinkCache)GetValue(LinkCacheProperty);
            set => SetValue(LinkCacheProperty, value);
        }
        public static readonly DependencyProperty LinkCacheProperty = DependencyProperty.Register(nameof(LinkCache), typeof(ILinkCache), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(ILinkCache)));

        public IEnumerable ScopedTypes
        {
            get => (IEnumerable)GetValue(ScopedTypesProperty);
            set => SetValue(ScopedTypesProperty, value);
        }
        public static readonly DependencyProperty ScopedTypesProperty = DependencyProperty.Register(nameof(ScopedTypes), typeof(IEnumerable), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(IEnumerable), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool Found
        {
            get => (bool)GetValue(FoundProperty);
            set => SetValue(FoundProperty, value);
        }
        public static readonly DependencyProperty FoundProperty = DependencyProperty.Register(nameof(Found), typeof(bool), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool Processing
        {
            get => (bool)GetValue(ProcessingProperty);
            set => SetValue(ProcessingProperty, value);
        }
        public static readonly DependencyProperty ProcessingProperty = DependencyProperty.Register(nameof(Processing), typeof(bool), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(bool)));

        public FormKey FormKey
        {
            get => (FormKey)GetValue(FormKeyProperty);
            set => SetValue(FormKeyProperty, value);
        }
        public static readonly DependencyProperty FormKeyProperty = DependencyProperty.Register(nameof(FormKey), typeof(FormKey), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(FormKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string EditorID
        {
            get => (string)GetValue(EditorIDProperty);
            set => SetValue(EditorIDProperty, value);
        }
        public static readonly DependencyProperty EditorIDProperty = DependencyProperty.Register(nameof(EditorID), typeof(string), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public StatusIndicatorState Status
        {
            get => (StatusIndicatorState)GetValue(StatusProperty);
            set => SetValue(StatusPropertyKey, value);
        }
        public static readonly DependencyPropertyKey StatusPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Status), typeof(StatusIndicatorState), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(StatusIndicatorState)));
        public static readonly DependencyProperty StatusProperty = StatusPropertyKey.DependencyProperty;

        public string StatusString
        {
            get => (string)GetValue(StatusStringProperty);
            set => SetValue(StatusStringPropertyKey, value);
        }
        public static readonly DependencyPropertyKey StatusStringPropertyKey = DependencyProperty.RegisterReadOnly(nameof(StatusString), typeof(string), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(string)));
        public static readonly DependencyProperty StatusStringProperty = StatusStringPropertyKey.DependencyProperty;

        public ICommand PickerClickCommand
        {
            get => (ICommand)GetValue(PickerClickCommandProperty);
            set => SetValue(PickerClickCommandProperty, value);
        }
        public static readonly DependencyProperty PickerClickCommandProperty = DependencyProperty.Register(nameof(PickerClickCommand), typeof(ICommand), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool InSearchMode
        {
            get => (bool)GetValue(InSearchModeProperty);
            set => SetValue(InSearchModePropertyKey, value);
        }
        public static readonly DependencyPropertyKey InSearchModePropertyKey = DependencyProperty.RegisterReadOnly(nameof(InSearchMode), typeof(bool), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(bool)));
        public static readonly DependencyProperty InSearchModeProperty = InSearchModePropertyKey.DependencyProperty;

        #region Theming
        public Brush ProcessingSpinnerForeground
        {
            get => (Brush)GetValue(ProcessingSpinnerForegroundProperty);
            set => SetValue(ProcessingSpinnerForegroundProperty, value);
        }
        public static readonly DependencyProperty ProcessingSpinnerForegroundProperty = DependencyProperty.Register(nameof(ProcessingSpinnerForeground), typeof(Brush), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 255, 255))));

        public Color ProcessingSpinnerGlow
        {
            get => (Color)GetValue(ProcessingSpinnerGlowProperty);
            set => SetValue(ProcessingSpinnerGlowProperty, value);
        }
        public static readonly DependencyProperty ProcessingSpinnerGlowProperty = DependencyProperty.Register(nameof(ProcessingSpinnerGlow), typeof(Color), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 255, 255)));

        public Brush ErrorBrush
        {
            get => (Brush)GetValue(ErrorBrushProperty);
            set => SetValue(ErrorBrushProperty, value);
        }
        public static readonly DependencyProperty ErrorBrushProperty = DependencyProperty.Register(nameof(ErrorBrush), typeof(Brush), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources[Noggog.WPF.Brushes.Constants.ErrorForeground]));

        public Brush SuccessBrush
        {
            get => (Brush)GetValue(SuccessBrushProperty);
            set => SetValue(SuccessBrushProperty, value);
        }
        public static readonly DependencyProperty SuccessBrushProperty = DependencyProperty.Register(nameof(SuccessBrush), typeof(Brush), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources[Noggog.WPF.Brushes.Constants.SuccessForeground]));

        public Brush PassiveBrush
        {
            get => (Brush)GetValue(PassiveBrushProperty);
            set => SetValue(PassiveBrushProperty, value);
        }
        public static readonly DependencyProperty PassiveBrushProperty = DependencyProperty.Register(nameof(PassiveBrush), typeof(Brush), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources[Noggog.WPF.Brushes.Constants.PassiveForeground]));

        public bool AllowsSearchMode
        {
            get => (bool)GetValue(AllowsSearchModeProperty);
            set => SetValue(AllowsSearchModeProperty, value);
        }
        public static readonly DependencyProperty AllowsSearchModeProperty = DependencyProperty.Register(nameof(AllowsSearchMode), typeof(bool), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable ApplicableEditorIDs
        {
            get => (IEnumerable)GetValue(ApplicableEditorIDsProperty);
            protected set => SetValue(ApplicableEditorIDsPropertyKey, value);
        }
        public static readonly DependencyPropertyKey ApplicableEditorIDsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ApplicableEditorIDs), typeof(IEnumerable), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(IEnumerable)));
        public static readonly DependencyProperty ApplicableEditorIDsProperty = ApplicableEditorIDsPropertyKey.DependencyProperty;

        public bool ViewingAllowedTypes
        {
            get => (bool)GetValue(ViewingAllowedTypesProperty);
            set => SetValue(ViewingAllowedTypesProperty, value);
        }
        public static readonly DependencyProperty ViewingAllowedTypesProperty = DependencyProperty.Register(nameof(ViewingAllowedTypes), typeof(bool), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(bool)));

        public ICommand ViewAllowedTypesCommand
        {
            get => (ICommand)GetValue(ViewAllowedTypesCommandProperty);
            set => SetValue(ViewAllowedTypesCommandProperty, value);
        }
        public static readonly DependencyProperty ViewAllowedTypesCommandProperty = DependencyProperty.Register(nameof(ViewAllowedTypesCommand), typeof(ICommand), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(ICommand)));
        #endregion

        record State(StatusIndicatorState Status, string Text, FormKey FormKey, string Edid);

        static AFormKeyPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AFormKeyPicker), new FrameworkPropertyMetadata(typeof(AFormKeyPicker)));
        }

        public AFormKeyPicker()
        {
            this.WhenAnyValue(x => x.FormKey)
                .DistinctUntilChanged()
                .Skip(1)
                .Where(x => !_updating)
                .CombineLatest(
                    this.WhenAnyValue(
                        x => x.LinkCache,
                        x => x.ScopedTypes),
                    (form, sources) => (FormKey: form, LinkCache: sources.Item1, Types: sources.Item2))
                .Do(_ =>
                {
                    if (!Processing)
                    {
                        Processing = true;
                    }
                })
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x =>
                {
                    try
                    {
                        if (x.LinkCache == null)
                        {
                            return new State(StatusIndicatorState.Passive, "No LinkCache is provided for lookup", FormKey.Null, string.Empty);
                        }
                        if (x.FormKey.IsNull)
                        {
                            return new State(StatusIndicatorState.Passive, "FormKey is null.  No lookup required", FormKey.Null, string.Empty);
                        }
                        var scopedTypes = ScopedTypesInternal(x.Types);
                        if (x.LinkCache.TryResolveIdentifier(x.FormKey, scopedTypes, out var edid))
                        {
                            return new State(StatusIndicatorState.Success, "Located record", x.FormKey, edid ?? string.Empty);
                        }
                        else
                        {
                            return new State(StatusIndicatorState.Failure, "Could not resolve record", FormKey.Null, string.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new State(StatusIndicatorState.Failure, ex.ToString(), FormKey.Null, string.Empty);
                    }
                })
                .StartWith(new State(StatusIndicatorState.Passive, "FormKey is null.  No lookup required", FormKey.Null, string.Empty))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(rec =>
                {
                    if (Processing)
                    {
                        Processing = false;
                    }

                    if (StatusString != rec.Text)
                    {
                        StatusString = rec.Text;
                    }

                    if (Status != rec.Status)
                    {
                        Status = rec.Status;
                    }

                    if (rec.Status == StatusIndicatorState.Success)
                    {
                        if (EditorID != rec.Edid)
                        {
                            _updating = true;
                            EditorID = rec.Edid;
                            _updating = false;
                        }

                        if (!Found)
                        {
                            Found = true;
                        }
                    }
                    else
                    {
                        if (EditorID != string.Empty)
                        {
                            _updating = true;
                            EditorID = string.Empty;
                            _updating = false;
                        }

                        if (Found)
                        {
                            Found = false;
                        }
                    }
                })
                .DisposeWith(_unloadDisposable);

            this.WhenAnyValue(x => x.EditorID)
                .Skip(1)
                .DistinctUntilChanged()
                .Where(x => !_updating)
                .CombineLatest(
                    this.WhenAnyValue(
                        x => x.LinkCache,
                        x => x.ScopedTypes),
                    (edid, sources) => (EditorID: edid, LinkCache: sources.Item1, Types: sources.Item2))
                .Do(_ =>
                {
                    if (!Processing)
                    {
                        Processing = true;
                    }
                })
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x =>
                {
                    try
                    {
                        if (x.LinkCache == null)
                        {
                            return new State(StatusIndicatorState.Passive, "No LinkCache is provided for lookup", FormKey.Null, string.Empty);
                        }
                        if (string.IsNullOrWhiteSpace(x.EditorID))
                        {
                            return new State(StatusIndicatorState.Passive, "EditorID is empty.  No lookup required", FormKey.Null, string.Empty);
                        }
                        var scopedTypes = ScopedTypesInternal(x.Types);
                        if (x.LinkCache.TryResolveIdentifier(x.EditorID, scopedTypes, out var formKey))
                        {
                            return new State(StatusIndicatorState.Success, "Located record", formKey, x.EditorID ?? string.Empty);
                        }
                        else
                        {
                            return new State(StatusIndicatorState.Failure, "Could not resolve record", FormKey.Null, string.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new State(StatusIndicatorState.Failure, ex.ToString(), FormKey.Null, string.Empty);
                    }
                })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(rec =>
                {
                    if (Processing)
                    {
                        Processing = false;
                    }

                    if (StatusString != rec.Text)
                    {
                        StatusString = rec.Text;
                    }

                    if (Status != rec.Status)
                    {
                        Status = rec.Status;
                    }

                    if (rec.Status == StatusIndicatorState.Success)
                    {
                        if (FormKey != rec.FormKey)
                        {
                            _updating = true;
                            FormKey = rec.FormKey;
                            _updating = false;
                        }

                        if (!Found)
                        {
                            Found = true;
                        }
                    }
                    else
                    {
                        if (!FormKey.IsNull)
                        {
                            _updating = true;
                            FormKey = FormKey.Null;
                            _updating = false;
                        }

                        if (Found)
                        {
                            Found = false;
                        }
                    }
                })
                .DisposeWith(_unloadDisposable);

            ApplicableEditorIDs = Observable.CombineLatest(
                    this.WhenAnyValue(x => x.LinkCache),
                    this.WhenAnyValue(x => x.ScopedTypes),
                    (LinkCache, ScopedTypes) => (LinkCache, ScopedTypes))
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x =>
                {
                    return Observable.Create<IMajorRecordIdentifier>(async (obs, cancel) =>
                    {
                        try
                        {
                            var scopedTypes = ScopedTypesInternal(x.ScopedTypes);
                            if (!scopedTypes.Any() || x.LinkCache == null)
                            {
                                return;
                            }
                            foreach (var item in x.LinkCache.AllIdentifiers(scopedTypes, cancel))
                            {
                                if (cancel.IsCancellationRequested) return;
                                var edid = item.EditorID;
                                if (edid.IsNullOrWhitespace()) continue;
                                obs.OnNext(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            obs.OnError(ex);
                        }
                        obs.OnCompleted();
                    });
                })
                .FilterSwitch(this.WhenAnyValue(x => x.InSearchMode), Observable.Empty<IMajorRecordIdentifier>())
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => x.ToObservableChangeSet())
                .Switch()
                .ObserveOnGui()
                .Filter(this.WhenAnyValue(x => x.EditorID)
                    .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select<string, Func<IMajorRecordIdentifier, bool>>(term => (ident) =>
                    {
                        var edid = ident.EditorID;
                        return edid.IsNullOrWhitespace() || term.IsNullOrWhitespace() ? true : edid.ContainsInsensitive(term);
                    }))
                .ObserveOnGui()
                .ToObservableCollection(this._unloadDisposable);

            this.WhenAnyValue(x => x.AllowsSearchMode)
                .Where(x => !x)
                .Subscribe(_ => InSearchMode = false)
                .DisposeWith(_unloadDisposable);

            this.WhenAnyValue(x => x.InSearchMode)
                .Where(x => !x)
                .ObserveOnGui()
                .Subscribe(_ => ViewingAllowedTypes = false)
                .DisposeWith(_unloadDisposable);

            ViewAllowedTypesCommand = ReactiveCommand.Create(() => ViewingAllowedTypes = !ViewingAllowedTypes);
        }

        protected IEnumerable<Type> ScopedTypesInternal(IEnumerable types)
        {
            var scopedTypes = types as IEnumerable<Type>;
            if (scopedTypes == null || !scopedTypes.Any())
            {
                scopedTypes = typeof(IMajorRecordCommonGetter).AsEnumerable();
            }
            return scopedTypes;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var editorIdBox = GetTemplateChild("PART_EditorIDBox") as TextBox;
            if (editorIdBox != null)
            {
                editorIdBox.WhenAnyValue(x => x.Text)
                    .Skip(1)
                    .FilterSwitch(editorIdBox.WhenAnyValue(x => x.IsKeyboardFocused))
                    .Subscribe(_ =>
                    {
                        this.InSearchMode = true;
                    })
                    .DisposeWith(_templateDisposable);
                editorIdBox.WhenAnyValue(x => x.IsKeyboardFocused)
                    .DistinctUntilChanged()
                    .Where(focused => focused)
                    .WithLatestFrom(
                        this.WhenAnyValue(x => x.Found),
                        (_, found) => found)
                    .Where(found => !found)
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
