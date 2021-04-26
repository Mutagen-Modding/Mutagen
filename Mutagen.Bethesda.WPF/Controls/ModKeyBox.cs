using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using Noggog;
using System.Linq;
using System.IO;
using Noggog.WPF;
using System.ComponentModel;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.WPF
{
    [TemplatePart(Name = "PART_FileNameBox", Type = typeof(TextBox))]
    public class ModKeyBox : NoggogControl
    {
        private bool _blockSync = false;

        public ModKey ModKey
        {
            get => (ModKey)GetValue(ModKeyProperty);
            set => SetValue(ModKeyProperty, value);
        }
        public static readonly DependencyProperty ModKeyProperty = DependencyProperty.Register(nameof(ModKey), typeof(ModKey), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(default(ModKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ModType ModType
        {
            get => (ModType)GetValue(ModTypeProperty);
            set => SetValue(ModTypeProperty, value);
        }
        public static readonly DependencyProperty ModTypeProperty = DependencyProperty.Register(nameof(ModType), typeof(ModType), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(default(ModType), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable<ModType> ModTypes => EnumExt.GetValues<ModType>();

        public string Watermark
        {
            get => (string)GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof(Watermark), typeof(string), typeof(ModKeyBox),
             new FrameworkPropertyMetadata("Mod name", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ErrorResponse Error
        {
            get => (ErrorResponse)GetValue(ErrorProperty);
            protected set => SetValue(ErrorPropertyKey, value);
        }
        public static readonly DependencyPropertyKey ErrorPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(ErrorResponse), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(default(ErrorResponse), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ErrorProperty = ErrorPropertyKey.DependencyProperty;

        public double MaxSearchBoxHeight
        {
            get => (double)GetValue(MaxSearchBoxHeightProperty);
            set => SetValue(MaxSearchBoxHeightProperty, value);
        }
        public static readonly DependencyProperty MaxSearchBoxHeightProperty = DependencyProperty.Register(nameof(MaxSearchBoxHeight), typeof(double), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(double.PositiveInfinity));

        public double SearchBoxHeight
        {
            get => (double)GetValue(SearchBoxHeightProperty);
            set => SetValue(SearchBoxHeightProperty, value);
        }
        public static readonly DependencyProperty SearchBoxHeightProperty = DependencyProperty.Register(nameof(SearchBoxHeight), typeof(double), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(double.NaN));

        static ModKeyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModKeyBox), new FrameworkPropertyMetadata(typeof(ModKeyBox)));
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.WhenAnyValue(
                    x => x.FileName,
                    x => x.ModType)
                .DistinctUntilChanged()
                .Skip(1)
                .Subscribe(x =>
                {
                    if (!_blockSync)
                    {
                        _blockSync = true;
                        if (Path.HasExtension(x.Item1)
                            && ModKey.TryFromNameAndExtension(x.Item1, out var modKey1))
                        {
                            if (ModKey != modKey1)
                            {
                                ModKey = modKey1;
                            }
                            if (ModType != modKey1.Type)
                            {
                                ModType = modKey1.Type;
                            }
                            FileName = Path.GetFileNameWithoutExtension(x.Item1);
                            if (Error.Failed)
                            {
                                Error = ErrorResponse.Success;
                            }
                        }
                        else
                        {
                            if (ModKey.TryFromName(x.Item1, x.Item2, out var modKey2, out var errorReason))
                            {
                                if (Error.Failed)
                                {
                                    Error = ErrorResponse.Success;
                                }
                            }
                            else
                            {
                                Error = ErrorResponse.Fail(errorReason);
                                modKey2 = ModKey.Null;
                            }
                            if (modKey2 != ModKey)
                            {
                                ModKey = modKey2;
                            }
                        }
                        _blockSync = false;
                    }
                })
                .DisposeWith(_unloadDisposable);
            this.WhenAnyValue(x => x.ModKey)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (_blockSync) return;
                    if (x.IsNull)
                    {
                        if (FileName != string.Empty)
                        {
                            FileName = string.Empty;
                        }
                    }
                    else if (FileName != x.FileName)
                    {
                        FileName = x.FileName;
                    }
                    if (ModType != x.Type)
                    {
                        ModType = x.Type;
                    }
                })
                .DisposeWith(_unloadDisposable);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var fileNameBox = GetTemplateChild("PART_FileNameBox") as TextBox;
            fileNameBox?.GotFocusWithSelectionSkipped()
                .Subscribe(_ =>
                {
                    if (ModKey.TryFromNameAndExtension(FileName, out var modKey))
                    {
                        fileNameBox.Select(0, modKey.Name.Length);
                    }
                    else
                    {
                        fileNameBox.SelectAll();
                    }
                })
                .DisposeWith(_templateDisposable);
        }
    }
}
