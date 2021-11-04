using System;
using Mutagen.Bethesda.Plugins;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DynamicData;
using DynamicData.Binding;
using Noggog;

namespace Mutagen.Bethesda.WPF.Plugins
{
    [TemplatePart(Name = "PART_AddedModKeyListBox", Type = typeof(ListBox))]
    public class ModKeyMultiPicker : AModKeyPicker
    {
        private ListBox? _modKeyListBox;
        
        public IList<ModKey>? ModKeys
        {
            get => (IList<ModKey>)GetValue(ModKeysProperty);
            set => SetValue(ModKeysProperty, value);
        }
        public static readonly DependencyProperty ModKeysProperty = DependencyProperty.Register(nameof(ModKeys), typeof(IList<ModKey>), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(IList<ModKey?>)));

        public ModKey? SelectedModKey
        {
            get => (ModKey?)GetValue(SelectedModKeyProperty);
            set => SetValue(SelectedModKeyProperty, value);
        }
        public static readonly DependencyProperty SelectedModKeyProperty = DependencyProperty.Register(nameof(SelectedModKey), typeof(ModKey?), typeof(ModKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ModKey?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                 (d, e) =>
                 {
                     var picker = (ModKeyMultiPicker)d;
                     if (e.NewValue == null)
                     {
                         picker.SelectedModKeyViewModel = null;
                         return;
                     }
                     var newModKey = (ModKey)e.NewValue;
                     picker.SelectedModKeyViewModel = picker.ModKeySelectionViewModels.FirstOrDefault(x => x.Item == newModKey);
                 }));

        public ReadOnlyObservableCollection<SelectedVm> ModKeySelectionViewModels { get; }

        public SelectedVm? SelectedModKeyViewModel
        {
            get { return (SelectedVm?)GetValue(SelectedModKeyViewModelProperty); }
            set { SetValue(SelectedModKeyViewModelProperty, value); }
        }
        public static readonly DependencyProperty SelectedModKeyViewModelProperty = DependencyProperty.Register(
            nameof(SelectedModKeyViewModel), typeof(SelectedVm), typeof(ModKeyMultiPicker), 
            new FrameworkPropertyMetadata(default(SelectedVm?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, e) =>
                {
                    var picker = (ModKeyMultiPicker)d;
                    if (e.NewValue == null)
                    {
                        picker.SelectedModKey = null;
                        return;
                    }
                    var newFormKey = (SelectedVm)e.NewValue;
                    picker.SelectedModKey = newFormKey.Item;
                }));

        public class SelectedVm : ReactiveObject, ISelectedItem<ModKey>
        {
            [Reactive] public bool IsSelected { get; set; }
            public ModKey Item { get; }

            public SelectedVm(ModKey modKey)
            {
                Item = modKey;
            }
        }

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

            this.WhenAnyValue(x => x.ModKeys)
                .Select(x =>
                {
                    if (x is ObservableCollection<ModKey> obsCollection)
                    {
                        return obsCollection.ToObservableChangeSet();
                    }

                    if (x is IObservableCollection<ModKey> obsCollInterf)
                    {
                        return obsCollInterf.ToObservableChangeSet<IObservableCollection<ModKey>, ModKey>();
                    }

                    return Observable.Empty<IChangeSet<ModKey>>();
                })
                .Switch()
                .Transform(x => new SelectedVm(x))
                .Bind(out ReadOnlyObservableCollection<SelectedVm> selModKeys)
                .Subscribe();
            ModKeySelectionViewModels = selModKeys;
            
            DeleteSelectedItemsCommand = ReactiveCommand.Create(
                canExecute: this.WhenAnyValue(x => x.SelectedModKeyViewModel)
                    .Select(x => x != null),
                execute: () =>
                {
                    var modKeys = ModKeys;
                    if (modKeys == null) return;
                    foreach (var item in ModKeySelectionViewModels
                        .WithIndex()
                        .Where(i => i.Item.IsSelected)
                        .OrderByDescending(x => x.Index)
                        .ToArray())
                    {
                        modKeys.RemoveAt(item.Index);
                    }
                });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _modKeyListBox = GetTemplateChild("PART_AddedModKeyListBox") as ListBox;
            if (_modKeyListBox != null)
            {
                _modKeyListBox.ItemsSource = ModKeySelectionViewModels;
                Noggog.WPF.Drag.ListBoxDragDrop(_modKeyListBox, () => this.ModKeys as IList<ModKey>)
                    .DisposeWith(_templateDisposable);
            }
        }
    }
}
