using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections;
using Noggog;
using System.Windows.Media;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using DynamicData;
using System.Windows.Controls;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.WPF.Plugins
{
    public enum FormKeyPickerSearchMode
    {
        None,
        EditorID,
        FormKey,
    }

    [TemplatePart(Name = "PART_EditorIDBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_FormKeyBox", Type = typeof(TextBox))]
    public class AFormKeyPicker : NoggogControl
    {
        private bool _updating;

        public ILinkCache? LinkCache
        {
            get => (ILinkCache?)GetValue(LinkCacheProperty);
            set => SetValue(LinkCacheProperty, value);
        }
        public static readonly DependencyProperty LinkCacheProperty = DependencyProperty.Register(nameof(LinkCache), typeof(ILinkCache), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(ILinkCache)));

        public IEnumerable? ScopedTypes
        {
            get => (IEnumerable?)GetValue(ScopedTypesProperty);
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

        public string FormKeyStr
        {
            get => (string)GetValue(FormKeyStrProperty);
            set => SetValue(FormKeyStrProperty, value);
        }
        public static readonly DependencyProperty FormKeyStrProperty = DependencyProperty.Register(nameof(FormKeyStr), typeof(string), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string EditorID
        {
            get => (string)GetValue(EditorIDProperty);
            set => SetValue(EditorIDProperty, value);
        }
        public static readonly DependencyProperty EditorIDProperty = DependencyProperty.Register(nameof(EditorID), typeof(string), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool MissingMeansError
        {
            get => (bool)GetValue(MissingMeansErrorProperty);
            set => SetValue(MissingMeansErrorProperty, value);
        }
        public static readonly DependencyProperty MissingMeansErrorProperty = DependencyProperty.Register(nameof(MissingMeansError), typeof(bool), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(true));

        public bool? MissingMeansNull
        {
            get => (bool?)GetValue(MissingMeansNullProperty);
            set => SetValue(MissingMeansNullProperty, value);
        }
        public static readonly DependencyProperty MissingMeansNullProperty = DependencyProperty.Register(nameof(MissingMeansNull), typeof(bool?), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(bool?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

        public FormKeyPickerSearchMode SearchMode
        {
            get => (FormKeyPickerSearchMode)GetValue(SearchModeProperty);
            set => SetValue(SearchModePropertyKey, value);
        }
        public static readonly DependencyPropertyKey SearchModePropertyKey = DependencyProperty.RegisterReadOnly(nameof(SearchMode), typeof(FormKeyPickerSearchMode), typeof(AFormKeyPicker),
             new FrameworkPropertyMetadata(default(FormKeyPickerSearchMode), (o, e) =>
             {
                 var control = (AFormKeyPicker)o;
                 switch ((FormKeyPickerSearchMode)e.NewValue)
                 {
                     case FormKeyPickerSearchMode.None:
                         control.InSearchMode = false;
                         break;
                     default:
                         control.InSearchMode = true;
                         break;
                 }
             }));
        public static readonly DependencyProperty SearchModeProperty = SearchModePropertyKey.DependencyProperty;

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
            ViewAllowedTypesCommand = ReactiveCommand.Create(() => ViewingAllowedTypes = !ViewingAllowedTypes);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.WhenAnyValue(x => x.FormKey)
                .DistinctUntilChanged()
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
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
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
                            return new State(StatusIndicatorState.Failure, "Could not resolve record", x.FormKey, string.Empty);
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

                        var formKeyStr = rec.FormKey.ToString();
                        if (formKeyStr != FormKeyStr)
                        {
                            _updating = true;
                            FormKeyStr = formKeyStr;
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

                        var formKeyStr = rec.FormKey.IsNull ? string.Empty : rec.FormKey.ToString();
                        if (FormKeyStr != formKeyStr)
                        {
                            _updating = true;
                            FormKeyStr = formKeyStr;
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
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
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

                        if (FormKeyStr != string.Empty)
                        {
                            _updating = true;
                            FormKeyStr = string.Empty;
                            _updating = false;
                        }

                        if (Found)
                        {
                            Found = false;
                        }
                    }
                })
                .DisposeWith(_unloadDisposable);

            this.WhenAnyValue(x => x.FormKeyStr)
                .Skip(1)
                .Select(x => x.Trim())
                .DistinctUntilChanged()
                .Where(_ => !_updating)
                .CombineLatest(
                    this.WhenAnyValue(
                        x => x.LinkCache,
                        x => x.ScopedTypes,
                        x => x.MissingMeansError,
                        x => x.MissingMeansNull),
                    (str, sources) => (Raw: str, LinkCache: sources.Item1, Types: sources.Item2, MissingMeansError: sources.Item3, MissingMeansNull: sources.Item4))
                .Do(_ =>
                {
                    if (!Processing)
                    {
                        Processing = true;
                    }
                })
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(x.Raw))
                        {
                            return new State(StatusIndicatorState.Passive, "Input is empty.  No lookup required", FormKey.Null, string.Empty);
                        }

                        var scopedTypes = ScopedTypesInternal(x.Types);

                        if (FormKey.TryFactory(x.Raw, out var formKey))
                        {
                            if (x.LinkCache == null)
                            {
                                return new State(StatusIndicatorState.Success, "Valid FormKey", formKey, string.Empty);
                            }
                            if (x.LinkCache.TryResolveIdentifier(formKey, scopedTypes, out var edid))
                            {
                                return new State(StatusIndicatorState.Success, "Located record", formKey, edid ?? string.Empty);
                            }
                            else
                            {
                                FormKey formKeyToUse;
                                if (x.MissingMeansNull ?? x.MissingMeansError)
                                {
                                    formKeyToUse = FormKey.Null;
                                }
                                else
                                {
                                    formKeyToUse = formKey;
                                }
                                return new State(
                                    x.MissingMeansError ? StatusIndicatorState.Failure : StatusIndicatorState.Success,
                                    "Could not resolve record",
                                    formKeyToUse,
                                    string.Empty);
                            }
                        }

                        if (x.LinkCache == null)
                        {
                            return new State(StatusIndicatorState.Passive, "No LinkCache is provided for lookup", FormKey.Null, string.Empty);
                        }

                        if (FormID.TryFactory(x.Raw, out var formID, strictLength: true))
                        {
                            if (x.LinkCache.ListedOrder.Count >= formID.ModIndex.ID)
                            {
                                var targetMod = x.LinkCache.ListedOrder[formID.ModIndex.ID];
                                formKey = new FormKey(targetMod.ModKey, formID.ID);
                                if (x.LinkCache.TryResolveIdentifier(formKey, scopedTypes, out var edid))
                                {
                                    return new State(StatusIndicatorState.Success, "Located record", formKey, edid ?? string.Empty);
                                }
                                else
                                {
                                    return new State(StatusIndicatorState.Failure, "Could not resolve record", FormKey.Null, string.Empty);
                                }
                            }
                        }

                        return new State(StatusIndicatorState.Failure, "Could not resolve record", FormKey.Null, string.Empty);
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

                        var formKeyStr = rec.FormKey.ToString();
                        if (FormKeyStr != formKeyStr)
                        {
                            _updating = true;
                            FormKeyStr = formKeyStr;
                            _updating = false;
                        }

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
                        if (FormKey != rec.FormKey)
                        {
                            _updating = true;
                            FormKey = rec.FormKey;
                            _updating = false;
                        }

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
                .FlowSwitch(this.WhenAnyValue(x => x.InSearchMode), Observable.Empty<IMajorRecordIdentifier>())
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => x.ToObservableChangeSet())
                .Switch()
                .ObserveOnGui()
                .Filter(Observable.CombineLatest(
                        this.WhenAnyValue(x => x.SearchMode)
                            .DistinctUntilChanged(),
                        this.WhenAnyValue(x => x.LinkCache),
                        (SearchMode, Cache) => (SearchMode, Cache))
                    .Select(x =>
                    {
                        switch (x.SearchMode)
                        {
                            case FormKeyPickerSearchMode.None:
                                return Observable.Return<Func<IMajorRecordIdentifier, bool>>(x => false);
                            case FormKeyPickerSearchMode.EditorID:
                                return this.WhenAnyValue(x => x.EditorID)
                                    .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                                    .ObserveOn(RxApp.TaskpoolScheduler)
                                    .Select<string, Func<IMajorRecordIdentifier, bool>>(term => (ident) =>
                                    {
                                        var edid = ident.EditorID;
                                        return edid.IsNullOrWhitespace() || term.IsNullOrWhitespace() ? true : edid.ContainsInsensitive(term);
                                    });
                            case FormKeyPickerSearchMode.FormKey:

                                var modKeyToId = x.Cache?.ListedOrder
                                    .Select((mod, index) => (mod, index))
                                    .Take(ModIndex.MaxIndex)
                                    .ToDictionary(keySelector: x => x.mod.ModKey, elementSelector: x => (byte)x.index)
                                    ?? default;

                                return this.WhenAnyValue(x => x.FormKeyStr)
                                    .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                                    .ObserveOn(RxApp.TaskpoolScheduler)
                                    .Select(RawStr =>
                                    {
                                        return (RawStr: RawStr, FormKey: FormKey.TryFactory(RawStr), FormID: FormID.TryFactory(RawStr, strictLength: false));
                                    })
                                    .Select<(string RawStr, FormKey? FormKey, FormID? ID), Func<IMajorRecordIdentifier, bool>>(term => (ident) =>
                                    {
                                        var fk = ident.FormKey;
                                        if (fk == term.FormKey) return true;
                                        if (term.ID != null)
                                        {
                                            if (term.RawStr.Length <= 6)
                                            {
                                                return fk.ID == term.ID.Value.Raw;
                                            }
                                            else if (modKeyToId != null && modKeyToId.TryGetValue(fk.ModKey, out var index))
                                            {
                                                var formID = new FormID(new ModIndex(index), fk.ID);
                                                return formID.Raw == term.ID.Value.Raw;
                                            }
                                        }
                                        return false;
                                    });
                            default:
                                throw new NotImplementedException();
                        }
                    })
                    .Switch())
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
        }

        protected IEnumerable<Type> ScopedTypesInternal(IEnumerable? types)
        {
            var scopedTypes = types as IEnumerable<Type>;
            if (scopedTypes == null || !scopedTypes.Any())
            {
                scopedTypes = typeof(IMajorRecordGetter).AsEnumerable();
            }
            return scopedTypes;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SearchMode = FormKeyPickerSearchMode.None;
            var editorIdBox = GetTemplateChild("PART_EditorIDBox") as TextBox;
            AttachTextBox(editorIdBox, FormKeyPickerSearchMode.EditorID);
            var formKeyBox = GetTemplateChild("PART_FormKeyBox") as TextBox;
            AttachTextBox(formKeyBox, FormKeyPickerSearchMode.FormKey);
        }

        private void AttachTextBox(TextBox? textBox, FormKeyPickerSearchMode searchMode)
        {
            if (textBox == null) return;
            textBox.WhenAnyValue(x => x.Text)
                .Skip(1)
                .FlowSwitch(textBox.WhenAnyValue(x => x.IsKeyboardFocused))
                .Subscribe(_ =>
                {
                    this.SearchMode = searchMode;
                })
                .DisposeWith(_templateDisposable);
            textBox.WhenAnyValue(x => x.IsKeyboardFocused)
                .DistinctUntilChanged()
                .Where(focused => focused)
                .WithLatestFrom(
                    this.WhenAnyValue(x => x.Found),
                    (_, found) => found)
                .Where(found => !found)
                .Subscribe(_ =>
                {
                    this.SearchMode = searchMode;
                })
                .DisposeWith(_templateDisposable);
            this.WhenAnyValue(x => x.IsKeyboardFocusWithin)
                .Merge(this.WhenAnyValue(x => x.IsVisible))
                .Where(x => !x)
                .Delay(TimeSpan.FromMilliseconds(150), RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    this.SearchMode = FormKeyPickerSearchMode.None;
                })
                .DisposeWith(_templateDisposable);
        }
    }
}
