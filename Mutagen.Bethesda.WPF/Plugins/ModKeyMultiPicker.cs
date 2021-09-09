using Mutagen.Bethesda.Plugins;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Noggog;

namespace Mutagen.Bethesda.WPF.Plugins
{
    [TemplatePart(Name = "PART_AddedModKeyListBox", Type = typeof(ListBox))]
    public class ModKeyMultiPicker : AModKeyPicker
    {
        private ListBox? _modKeyListBox;
        
        public ICollection<ModKey>? ModKeys
        {
            get => (ICollection<ModKey>)GetValue(ModKeysProperty);
            set => SetValue(ModKeysProperty, value);
        }
        public static readonly DependencyProperty ModKeysProperty = DependencyProperty.Register(nameof(ModKeys), typeof(ICollection<ModKey>), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ICollection<ModKey?>)));

        public ModKey? SelectedModKey
        {
            get => (ModKey?)GetValue(SelectedModKeyProperty);
            set => SetValue(SelectedModKeyProperty, value);
        }
        public static readonly DependencyProperty SelectedModKeyProperty = DependencyProperty.Register(nameof(SelectedModKey), typeof(ModKey?), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ModKey?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Brush SelectedForegroundBrush
        {
            get => (Brush)GetValue(SelectedForegroundBrushProperty);
            set => SetValue(SelectedForegroundBrushProperty, value);
        }
        public static readonly DependencyProperty SelectedForegroundBrushProperty = DependencyProperty.Register(nameof(SelectedForegroundBrush), typeof(Brush), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources[Noggog.WPF.Brushes.Constants.SelectedForeground]));

        public ICommand AddModKeyCommand
        {
            get => (ICommand)GetValue(AddModKeyCommandProperty);
            set => SetValue(AddModKeyCommandProperty, value);
        }
        public static readonly DependencyProperty AddModKeyCommandProperty = DependencyProperty.Register(nameof(AddModKeyCommand), typeof(ICommand), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Brush ItemHoverBrush
        {
            get => (Brush)GetValue(ItemHoverBrushProperty);
            set => SetValue(ItemHoverBrushProperty, value);
        }
        public static readonly DependencyProperty ItemHoverBrushProperty = DependencyProperty.Register(nameof(ItemHoverBrush), typeof(Brush), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Brush SelectedBackgroundBrush
        {
            get => (Brush)GetValue(SelectedBackgroundBrushProperty);
            set => SetValue(SelectedBackgroundBrushProperty, value);
        }
        public static readonly DependencyProperty SelectedBackgroundBrushProperty = DependencyProperty.Register(nameof(SelectedBackgroundBrush), typeof(Brush), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources[Noggog.WPF.Brushes.Constants.SelectedBackground]));

        public ICommand DeleteSelectedItemsCommand
        {
            get => (ICommand)GetValue(DeleteSelectedItemsCommandProperty);
            set => SetValue(DeleteSelectedItemsCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteSelectedItemsCommandProperty = DependencyProperty.Register(nameof(DeleteSelectedItemsCommand), typeof(ICommand), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static ModKeyMultiPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModKeyMultiPicker), new FrameworkPropertyMetadata(typeof(ModKeyMultiPicker)));
        }

        public ModKeyMultiPicker()
        {
            PickerClickCommand = ReactiveCommand.Create((object o) =>
            {
                if (ModKeys == null) return;
                switch (o)
                {
                    case ModKey modKey:
                        ModKeys.Add(modKey);
                        break;
                    default:
                        break;
                }
            });
            AddModKeyCommand = ReactiveCommand.Create(
                canExecute: this.WhenAnyValue(x => x.ModKey)
                    .Select(x => !x.IsNull),
                execute: () =>
                {
                    if (ModKeys == null) return;
                    ModKeys.Add(ModKey);
                });
            DeleteSelectedItemsCommand = ReactiveCommand.Create(
                canExecute: this.WhenAnyValue(x => x.SelectedModKey)
                    .Select(x => x != null),
                execute: () => _modKeyListBox?.TryRemoveSelected());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _modKeyListBox = GetTemplateChild("PART_AddedModKeyListBox") as ListBox;
            if (_modKeyListBox != null)
            {
                Noggog.WPF.Drag.ListBoxDragDrop(_modKeyListBox, () => this.ModKeys as IList<ModKey>)
                    .DisposeWith(_templateDisposable);
            }
        }
    }
}
