using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using Noggog;
using System.Linq;
using System.IO;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;

namespace Mutagen.Bethesda.WPF.Controls
{
    [TemplatePart(Name = "PART_FileNameBox", Type = typeof(TextBox))]
    public class ModKeyBox : NoggogControl
    {
        private static IEnumerable<ModType> _modTypes = EnumExt.GetValues<ModType>().ToArray();
        private bool _blockSync = false;

        public ModKey ModKey
        {
            get => (ModKey)GetValue(ModKeyProperty);
            set => SetValue(ModKeyProperty, value);
        }
        public static readonly DependencyProperty ModKeyProperty = DependencyProperty.Register(nameof(ModKey), typeof(ModKey), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(default(ModKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ModType ModType
        {
            get => (ModType)GetValue(ModTypeProperty);
            set => SetValue(ModTypeProperty, value);
        }
        public static readonly DependencyProperty ModTypeProperty = DependencyProperty.Register(nameof(ModType), typeof(ModType), typeof(ModKeyBox),
             new FrameworkPropertyMetadata(default(ModType), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable<ModType> ModTypes => _modTypes;

        public string Watermark
        {
            get => (string)GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof(Watermark), typeof(string), typeof(ModKeyBox),
             new FrameworkPropertyMetadata("Mod name", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static ModKeyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModKeyBox), new FrameworkPropertyMetadata(typeof(ModKeyBox)));
        }

        public ModKeyBox()
        {
            this.WhenAnyValue(
                    x => x.FileName,
                    x => x.ModType)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (_blockSync) return;
                    if (Path.HasExtension(x.Item1))
                    {
                        if (ModKey.TryFromNameAndExtension(x.Item1, out var modKey))
                        {
                            _blockSync = true;
                            if (ModKey != modKey)
                            {
                                ModKey = modKey;
                            }
                            if (ModType != modKey.Type)
                            {
                                ModType = modKey.Type;
                            }
                            FileName = Path.GetFileNameWithoutExtension(x.Item1);
                            _blockSync = false;
                        }
                        else if (!ModKey.IsNull)
                        {
                            _blockSync = true;
                            ModKey = ModKey.Null;
                            _blockSync = false;
                        }
                    }
                    else
                    {
                        var modKey = new ModKey(x.Item1, x.Item2);
                        if (modKey != ModKey)
                        {
                            _blockSync = true;
                            ModKey = modKey;
                            _blockSync = false;
                        }
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
                            _blockSync = true;
                            FileName = string.Empty;
                            _blockSync = false;
                        }
                    }
                    else if (FileName != x.FileName)
                    {
                        _blockSync = true;
                        FileName = x.FileName;
                        _blockSync = false;
                    }
                    if (ModType != x.Type)
                    {
                        _blockSync = true;
                        ModType = x.Type;
                        _blockSync = false;
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
