using System;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
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
using Noggog.WPF;
using Noggog.WPF.Containers;
using ReactiveUI.Fody.Helpers;

namespace Mutagen.Bethesda.WPF.Plugins
{
    [TemplatePart(Name = "PART_AddedFormKeyListBox", Type = typeof(ListBox))]
    public class FormKeyMultiPicker : AFormKeyPicker
    {
        private ListBox? _formKeyListBox;

        public IList<FormKey>? FormKeys
        {
            get => (IList<FormKey>)GetValue(FormKeysProperty);
            set => SetValue(FormKeysProperty, value);
        }
        public static readonly DependencyProperty FormKeysProperty = DependencyProperty.Register(nameof(FormKeys), typeof(IList<FormKey>), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(IList<FormKey>?)));

        public FormKey? SelectedFormKey
        {
            get => (FormKey?)GetValue(SelectedFormKeyProperty);
            set => SetValue(SelectedFormKeyProperty, value);
        }
        public static readonly DependencyProperty SelectedFormKeyProperty = DependencyProperty.Register(nameof(SelectedFormKey), typeof(FormKey?), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(FormKey?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                 (d, e) =>
                 {
                     var picker = (FormKeyMultiPicker)d;
                     if (e.NewValue == null)
                     {
                         picker.SelectedFormKeyViewModel = null;
                         return;
                     }
                     var newFormKey = (FormKey)e.NewValue;
                     picker.SelectedFormKeyViewModel = picker.FormKeySelectionViewModels.DerivativeList.FirstOrDefault(x => x.Item == newFormKey);
                 }));

        public IDerivativeSelectedCollection<FormKey> FormKeySelectionViewModels { get; }

        public SelectedVm<FormKey>? SelectedFormKeyViewModel
        {
            get { return (SelectedVm<FormKey>?)GetValue(SelectedFormKeyViewModelProperty); }
            set { SetValue(SelectedFormKeyViewModelProperty, value); }
        }
        public static readonly DependencyProperty SelectedFormKeyViewModelProperty = DependencyProperty.Register(
            nameof(SelectedFormKeyViewModel), typeof(SelectedVm<FormKey>), typeof(FormKeyMultiPicker), 
            new FrameworkPropertyMetadata(default(SelectedVm<FormKey>?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, e) =>
                {
                    var picker = (FormKeyMultiPicker)d;
                    if (e.NewValue == null)
                    {
                        picker.SelectedFormKey = null;
                        return;
                    }
                    var newFormKey = (SelectedVm<FormKey>)e.NewValue;
                    picker.SelectedFormKey = newFormKey.Item;
                }));

        public Brush SelectedForegroundBrush
        {
            get => (Brush)GetValue(SelectedForegroundBrushProperty);
            set => SetValue(SelectedForegroundBrushProperty, value);
        }
        public static readonly DependencyProperty SelectedForegroundBrushProperty = DependencyProperty.Register(nameof(SelectedForegroundBrush), typeof(Brush), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources[Noggog.WPF.Brushes.Constants.SelectedForeground]));

        public ICommand AddFormKeyCommand
        {
            get => (ICommand)GetValue(AddFormKeyCommandProperty);
            set => SetValue(AddFormKeyCommandProperty, value);
        }
        public static readonly DependencyProperty AddFormKeyCommandProperty = DependencyProperty.Register(nameof(AddFormKeyCommand), typeof(ICommand), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Brush ItemHoverBrush
        {
            get => (Brush)GetValue(ItemHoverBrushProperty);
            set => SetValue(ItemHoverBrushProperty, value);
        }
        public static readonly DependencyProperty ItemHoverBrushProperty = DependencyProperty.Register(nameof(ItemHoverBrush), typeof(Brush), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Brush SelectedBackgroundBrush
        {
            get => (Brush)GetValue(SelectedBackgroundBrushProperty);
            set => SetValue(SelectedBackgroundBrushProperty, value);
        }
        public static readonly DependencyProperty SelectedBackgroundBrushProperty = DependencyProperty.Register(nameof(SelectedBackgroundBrush), typeof(Brush), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources[Noggog.WPF.Brushes.Constants.SelectedBackground]));

        public ICommand DeleteSelectedItemsCommand
        {
            get => (ICommand)GetValue(DeleteSelectedItemsCommandProperty);
            set => SetValue(DeleteSelectedItemsCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteSelectedItemsCommandProperty = DependencyProperty.Register(nameof(DeleteSelectedItemsCommand), typeof(ICommand), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        static FormKeyMultiPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormKeyMultiPicker), new FrameworkPropertyMetadata(typeof(FormKeyMultiPicker)));
            MissingMeansNullProperty.OverrideMetadata(typeof(FormKeyMultiPicker), new FrameworkPropertyMetadata(false));
        }

        public FormKeyMultiPicker()
        {
            PickerClickCommand = ReactiveCommand.Create((object o) =>
            {
                if (FormKeys == null) return;
                switch (o)
                {
                    case IMajorRecordIdentifier identifier:
                        FormKeys.Add(identifier.FormKey);
                        break;
                    default:
                        break;
                }
            });
            AddFormKeyCommand = ReactiveCommand.Create(
                canExecute: this.WhenAnyValue(x => x.FormKey)
                    .Select(x => !x.IsNull),
                execute: () =>
                {
                    if (FormKeys == null) return;
                    FormKeys.Add(FormKey);
                });

            this.WhenAnyValue(x => x.FormKeys)
                .WrapInDerivativeSelectedCollection(out var selFormKeys);
            FormKeySelectionViewModels = selFormKeys;
            
            DeleteSelectedItemsCommand = ReactiveCommand.Create(
                canExecute: this.WhenAnyValue(x => x.SelectedFormKeyViewModel)
                    .Select(x => x != null),
                execute: () =>
                {
                    var formKeys = FormKeys;
                    if (formKeys == null) return;
                    foreach (var item in FormKeySelectionViewModels
                        .WithIndex()
                        .Where(i => i.Item.IsSelected)
                        .OrderByDescending(x => x.Index)
                        .ToArray())
                    {
                        formKeys.RemoveAt(item.Index);
                    }
                });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _formKeyListBox = GetTemplateChild("PART_AddedFormKeyListBox") as ListBox;
            if (_formKeyListBox != null)
            {
                _formKeyListBox.ItemsSource = FormKeySelectionViewModels;
                Noggog.WPF.Drag.ListBoxDragDrop<FormKey>(_formKeyListBox)
                    .DisposeWith(_templateDisposable);
            }
        }
    }
}
