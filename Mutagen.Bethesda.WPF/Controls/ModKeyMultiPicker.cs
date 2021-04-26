using Mutagen.Bethesda.Plugins;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mutagen.Bethesda.WPF
{
    public class ModKeyItemViewModel : ViewModel
    {
        [Reactive]
        public bool IsSelected { get; set; }

        public ModKey ModKey { get; }

        public ModKeyItemViewModel(ModKey modKey)
        {
            ModKey = modKey;
        }
    }

    [TemplatePart(Name = "PART_AddedModKeyListBox", Type = typeof(ListBox))]
    public class ModKeyMultiPicker : AModKeyPicker
    {
        public ICollection<ModKeyItemViewModel> ModKeys
        {
            get => (ICollection<ModKeyItemViewModel>)GetValue(ModKeysProperty);
            set => SetValue(ModKeysProperty, value);
        }
        public static readonly DependencyProperty ModKeysProperty = DependencyProperty.Register(nameof(ModKeys), typeof(ICollection<ModKeyItemViewModel>), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ICollection<ModKeyItemViewModel>)));

        public ModKeyItemViewModel SelectedModKey
        {
            get => (ModKeyItemViewModel)GetValue(SelectedModKeyProperty);
            set => SetValue(SelectedModKeyProperty, value);
        }
        public static readonly DependencyProperty SelectedModKeyProperty = DependencyProperty.Register(nameof(SelectedModKey), typeof(ModKeyItemViewModel), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ModKeyItemViewModel), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
                switch (o)
                {
                    case ModKey modKey:
                        ModKeys.Add(new ModKeyItemViewModel(modKey));
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
                    ModKeys.Add(new ModKeyItemViewModel(ModKey));
                });
            DeleteSelectedItemsCommand = ReactiveCommand.Create(
                canExecute: this.WhenAnyValue(x => x.SelectedModKey)
                    .Select(x => x != null),
                execute: () =>
                {
                    var toDelete = ModKeys.Where(f => f.IsSelected).ToArray();
                    foreach (var item in toDelete)
                    {
                        ModKeys.Remove(item);
                    }
                });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var modKeyBox = GetTemplateChild("PART_AddedModKeyListBox") as ListBox;
            if (modKeyBox != null)
            {
                Noggog.WPF.Drag.ListBoxDragDrop(modKeyBox, () => this.ModKeys as IList<ModKeyItemViewModel>)
                    .DisposeWith(_templateDisposable);
            }
        }
    }
}
