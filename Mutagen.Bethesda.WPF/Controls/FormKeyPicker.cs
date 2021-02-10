using Noggog.WPF;
using ReactiveUI;
using System.Collections;
using System.Windows;
using System;
using System.Reactive.Disposables;
using Noggog;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;
using DynamicData;
using DynamicData.Binding;

namespace Mutagen.Bethesda.WPF
{
    public class FormKeyPicker : NoggogControl
    {
        private bool _updating;

        public FormKey FormKey
        {
            get => (FormKey)GetValue(FormKeyProperty);
            set => SetValue(FormKeyProperty, value);
        }
        public static readonly DependencyProperty FormKeyProperty = DependencyProperty.Register(nameof(FormKey), typeof(FormKey), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(FormKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string EditorID
        {
            get => (string)GetValue(EditorIDProperty);
            set => SetValue(EditorIDProperty, value);
        }
        public static readonly DependencyProperty EditorIDProperty = DependencyProperty.Register(nameof(EditorID), typeof(string), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ILinkCache LinkCache
        {
            get => (ILinkCache)GetValue(LinkCacheProperty);
            set => SetValue(LinkCacheProperty, value);
        }
        public static readonly DependencyProperty LinkCacheProperty = DependencyProperty.Register(nameof(LinkCache), typeof(ILinkCache), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(ILinkCache)));

        public bool Found
        {
            get => (bool)GetValue(FoundProperty);
            set => SetValue(FoundProperty, value);
        }
        public static readonly DependencyProperty FoundProperty = DependencyProperty.Register(nameof(Found), typeof(bool), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable ScopedTypes
        {
            get => (IEnumerable)GetValue(ScopedTypesProperty);
            set => SetValue(ScopedTypesProperty, value);
        }
        public static readonly DependencyProperty ScopedTypesProperty = DependencyProperty.Register(nameof(ScopedTypes), typeof(IEnumerable), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(IEnumerable), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable ScopingOptions
        {
            get => (IEnumerable)GetValue(ScopingOptionsProperty);
            set => SetValue(ScopingOptionsProperty, value);
        }
        public static readonly DependencyProperty ScopingOptionsProperty = DependencyProperty.Register(nameof(ScopingOptions), typeof(IEnumerable), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(IEnumerable)));

        public bool Processing
        {
            get => (bool)GetValue(ProcessingProperty);
            set => SetValue(ProcessingProperty, value);
        }
        public static readonly DependencyProperty ProcessingProperty = DependencyProperty.Register(nameof(Processing), typeof(bool), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(bool)));

        public ErrorResponse Error
        {
            get => (ErrorResponse)GetValue(ErrorProperty);
            set => SetValue(ErrorProperty, value);
        }
        public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register(nameof(Error), typeof(ErrorResponse), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(ErrorResponse.Succeed("FormKey is null.  No lookup required")));

        #region Theming
        public Brush ProcessingSpinnerForeground
        {
            get => (Brush)GetValue(ProcessingSpinnerForegroundProperty);
            set => SetValue(ProcessingSpinnerForegroundProperty, value);
        }
        public static readonly DependencyProperty ProcessingSpinnerForegroundProperty = DependencyProperty.Register(nameof(ProcessingSpinnerForeground), typeof(Brush), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 255, 255))));

        public Color ProcessingSpinnerGlow
        {
            get => (Color)GetValue(ProcessingSpinnerGlowProperty);
            set => SetValue(ProcessingSpinnerGlowProperty, value);
        }
        public static readonly DependencyProperty ProcessingSpinnerGlowProperty = DependencyProperty.Register(nameof(ProcessingSpinnerGlow), typeof(Color), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(Color.FromArgb(255, 0, 255, 255)));

        public Brush ErrorBrush
        {
            get => (Brush)GetValue(ErrorBrushProperty);
            set => SetValue(ErrorBrushProperty, value);
        }
        public static readonly DependencyProperty ErrorBrushProperty = DependencyProperty.Register(nameof(ErrorBrush), typeof(Brush), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources["ErrorBrush"]));

        public Brush SuccessBrush
        {
            get => (Brush)GetValue(SuccessBrushProperty);
            set => SetValue(SuccessBrushProperty, value);
        }
        public static readonly DependencyProperty SuccessBrushProperty = DependencyProperty.Register(nameof(SuccessBrush), typeof(Brush), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources["SuccessBrush"]));

        public Brush PassiveBrush
        {
            get => (Brush)GetValue(PassiveBrushProperty);
            set => SetValue(PassiveBrushProperty, value);
        }
        public static readonly DependencyProperty PassiveBrushProperty = DependencyProperty.Register(nameof(PassiveBrush), typeof(Brush), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66))));
        #endregion

        public bool AllowsPicker
        {
            get => (bool)GetValue(AllowsPickerProperty);
            set => SetValue(AllowsPickerProperty, value);
        }
        public static readonly DependencyProperty AllowsPickerProperty = DependencyProperty.Register(nameof(AllowsPicker), typeof(bool), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool PickerEnabled
        {
            get => (bool)GetValue(PickerEnabledProperty);
            set => SetValue(PickerEnabledProperty, value);
        }
        public static readonly DependencyProperty PickerEnabledProperty = DependencyProperty.Register(nameof(PickerEnabled), typeof(bool), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable ApplicableEditorIDs
        {
            get => (IEnumerable)GetValue(ApplicableEditorIDsProperty);
            protected set => SetValue(ApplicableEditorIDsPropertyKey, value);
        }
        public static readonly DependencyPropertyKey ApplicableEditorIDsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ApplicableEditorIDs), typeof(IEnumerable), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(IEnumerable)));
        public static readonly DependencyProperty ApplicableEditorIDsProperty = ApplicableEditorIDsPropertyKey.DependencyProperty;

        public IEnumerable ApplicableFormKeys
        {
            get => (IEnumerable)GetValue(ApplicableFormKeysProperty);
            set => SetValue(ApplicableFormKeysPropertyKey, value);
        }
        public static readonly DependencyPropertyKey ApplicableFormKeysPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ApplicableFormKeys), typeof(IEnumerable), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(IEnumerable)));
        public static readonly DependencyProperty ApplicableFormKeysProperty = ApplicableFormKeysPropertyKey.DependencyProperty;

        static FormKeyPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormKeyPicker), new FrameworkPropertyMetadata(typeof(FormKeyPicker)));
        }

        public FormKeyPicker()
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
                            return GetResponse<(FormKey, string?)?>.Succeed(default((FormKey, string?)?), "No LinkCache is provided for lookup");
                        }
                        if (x.FormKey.IsNull)
                        {
                            return GetResponse<(FormKey, string?)?>.Succeed(default((FormKey, string?)?), "FormKey is null.  No lookup required");
                        }
                        var scopedTypes = ScopedTypesInternal(x.Types);
                        if (x.LinkCache.TryResolveIdentifier(x.FormKey, scopedTypes, out var edid))
                        {
                            return GetResponse<(FormKey, string?)?>.Succeed((x.FormKey, edid), "Located record");
                        }
                        else
                        {
                            return GetResponse<(FormKey, string?)?>.Fail(default((FormKey, string?)?), "Could not resolve record");
                        }
                    }
                    catch (Exception ex)
                    {
                        return GetResponse<(FormKey, string?)?>.Fail(ex);
                    }
                })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(rec =>
                {
                    if (Processing)
                    {
                        Processing = false;
                    }
                    if (rec.Failed
                        || rec.Value == null)
                    {
                        if (Found)
                        {
                            Found = false;
                        }
                        if (rec.Failed)
                        {
                            Error = rec;
                        }
                        else if (!Error.Succeeded)
                        {
                            Error = rec;
                        }
                        if (EditorID != string.Empty)
                        {
                            _updating = true;
                            EditorID = string.Empty;
                            _updating = false;
                        }
                    }
                    else
                    {
                        if (!Found)
                        {
                            Found = true;
                        }
                        if (Error.Failed)
                        {
                            Error = rec;
                        }
                        var targetEdid = rec.Value.Value.Item2 ?? string.Empty;
                        if (EditorID != targetEdid)
                        {
                            _updating = true;
                            EditorID = targetEdid;
                        }
                        _updating = false;
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
                            return GetResponse<(FormKey, string?)?>.Succeed(default((FormKey, string?)?), "No LinkCache is provided for lookup");
                        }
                        if (string.IsNullOrWhiteSpace(x.EditorID))
                        {
                            return GetResponse<(FormKey, string?)?>.Succeed(default((FormKey, string?)?), "EditorID is empty.  No lookup required");
                        }
                        var scopedTypes = ScopedTypesInternal(x.Types);
                        if (x.Item2.TryResolveIdentifier(x.EditorID, scopedTypes, out var formKey))
                        {
                            return GetResponse<(FormKey, string?)?>.Succeed((formKey, x.EditorID), "Located record");
                        }
                        else
                        {
                            return GetResponse<(FormKey, string?)?>.Fail(default((FormKey, string?)?), "Could not resolve record");
                        }
                    }
                    catch (Exception ex)
                    {
                        return GetResponse<(FormKey, string?)?>.Fail(ex);
                    }
                })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(rec =>
            {
                if (Processing)
                {
                    Processing = false;
                }
                if (rec.Failed
                    || rec.Value == null)
                {
                    if (Found)
                    {
                        Found = false;
                    }
                    if (rec.Failed)
                    {
                        Error = rec;
                    }
                    else if (!Error.Succeeded)
                    {
                        Error = rec;
                    }
                    if (!FormKey.IsNull)
                    {
                        _updating = true;
                        FormKey = FormKey.Null;
                        _updating = false;
                    }
                }
                else
                {
                    if (!Found)
                    {
                        Found = true;
                    }
                    if (Error.Failed)
                    {
                        Error = rec;
                    }
                    var targetEdid = rec.Value.Value.Item2 ?? string.Empty;
                    if (FormKey != rec.Value.Value.Item1)
                    {
                        _updating = true;
                        FormKey = rec.Value.Value.Item1;
                    }
                    _updating = false;
                }
            })
            .DisposeWith(_unloadDisposable);

            ApplicableFormKeys = Observable.CombineLatest(
                    this.WhenAnyValue(x => x.LinkCache),
                    this.WhenAnyValue(x => x.ScopedTypes),
                    (LinkCache, ScopedTypes) => (LinkCache, ScopedTypes))
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x =>
                {
                    return Observable.Create<FormKey>(async (obs, cancel) =>
                    {
                        try
                        {
                            var scopedTypes = ScopedTypesInternal(x.ScopedTypes);
                            if (!scopedTypes.Any() || x.LinkCache == null)
                            {
                                return;
                            }
                            foreach (var item in x.LinkCache.AllIdentifiers(scopedTypes, cancel)
                                .Select(x => x.FormKey))
                            {
                                if (cancel.IsCancellationRequested) return;
                                obs.OnNext(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            obs.OnError(ex);
                        }
                        obs.OnCompleted();
                    }).ToObservableChangeSet();
                })
                .Switch()
                .ToObservableCollection(this._unloadDisposable);

            ApplicableEditorIDs = Observable.CombineLatest(
                    this.WhenAnyValue(x => x.LinkCache),
                    this.WhenAnyValue(x => x.ScopedTypes),
                    (LinkCache, ScopedTypes) => (LinkCache, ScopedTypes))
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x =>
                {
                    return Observable.Create<string>(async (obs, cancel) =>
                    {
                        try
                        {
                            var scopedTypes = ScopedTypesInternal(x.ScopedTypes);
                            if (!scopedTypes.Any() || x.LinkCache == null)
                            {
                                return;
                            }
                            foreach (var item in x.LinkCache.AllIdentifiers(scopedTypes, cancel)
                                .Select(x => x.EditorID))
                            {
                                if (cancel.IsCancellationRequested) return;
                                if (item.IsNullOrWhitespace()) continue;
                                obs.OnNext(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            obs.OnError(ex);
                        }
                        obs.OnCompleted();
                    }).ToObservableChangeSet();
                })
                .Switch()
                .ToObservableCollection(this._unloadDisposable);
        }

        private IEnumerable<Type> ScopedTypesInternal(IEnumerable types)
        {
            var scopedTypes = types as IEnumerable<Type>;
            if (scopedTypes == null || !scopedTypes.Any())
            {
                scopedTypes = typeof(IMajorRecordCommonGetter).AsEnumerable();
            }
            return scopedTypes;
        }
    }
}
