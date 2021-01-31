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

namespace Mutagen.Bethesda.WPF
{
    public class FormKeyPicker : NoggogControl
    {
        public FormKey FormKey
        {
            get => (FormKey)GetValue(FormKeyProperty);
            set => SetValue(FormKeyProperty, value);
        }
        public static readonly DependencyProperty FormKeyProperty = DependencyProperty.Register(nameof(FormKey), typeof(FormKey), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(FormKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ILinkCache LinkCache
        {
            get => (ILinkCache)GetValue(LinkCacheProperty);
            set => SetValue(LinkCacheProperty, value);
        }
        public static readonly DependencyProperty LinkCacheProperty = DependencyProperty.Register(nameof(LinkCache), typeof(ILinkCache), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(ILinkCache)));

        public IMajorRecordCommonGetter? SelectedRecord
        {
            get => (IMajorRecordCommonGetter?)GetValue(SelectedRecordProperty);
            set => SetValue(SelectedRecordProperty, value);
        }
        public static readonly DependencyProperty SelectedRecordProperty = DependencyProperty.Register(nameof(SelectedRecord), typeof(IMajorRecordCommonGetter), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(default(IMajorRecordCommonGetter?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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

        static FormKeyPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormKeyPicker), new FrameworkPropertyMetadata(typeof(FormKeyPicker)));
        }

        public FormKeyPicker()
        {
            this.WhenAnyValue(
                    x => x.FormKey,
                    x => x.LinkCache,
                    x => x.ScopedTypes)
                .ObserveOn(RxApp.MainThreadScheduler)
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
                        if (x.Item2 == null)
                        {
                            return GetResponse<IMajorRecordCommonGetter?>.Succeed(default(IMajorRecordCommonGetter?), "No LinkCache is provided for lookup");
                        }
                        if (x.Item1.IsNull)
                        {
                            return GetResponse<IMajorRecordCommonGetter?>.Succeed(default(IMajorRecordCommonGetter?), "FormKey is null.  No lookup required");
                        }
                        var scopedTypes = x.Item3 as IEnumerable<Type>;
                        if (scopedTypes == null || !scopedTypes.Any())
                        {
                            scopedTypes = typeof(IMajorRecordCommonGetter).AsEnumerable();
                        }
                        if (x.Item2.TryResolve(x.Item1, scopedTypes, out var rec))
                        {
                            return GetResponse<IMajorRecordCommonGetter?>.Succeed(rec, "Located record");
                        }
                        else
                        {
                            return GetResponse<IMajorRecordCommonGetter?>.Fail(default(IMajorRecordCommonGetter?), "Could not resolve record");
                        }
                    }
                    catch (Exception ex)
                    {
                        return GetResponse<IMajorRecordCommonGetter?>.Fail(ex);
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
                        if (SelectedRecord != null)
                        {
                            SelectedRecord = null;
                        }
                        if (rec.Failed)
                        {
                            Error = rec;
                        }
                        else if (!Error.Succeeded)
                        {
                            Error = rec;
                        }
                    }
                    else
                    {
                        if (!Found)
                        {
                            Found = true;
                        }
                        if (!ReferenceEquals(SelectedRecord, rec.Value))
                        {
                            SelectedRecord = rec.Value;
                        }
                        if (Error.Failed)
                        {
                            Error = rec;
                        }
                    }
                })
                .DisposeWith(_unloadDisposable);
        }
    }
}
