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
                     picker.SelectedFormKeyViewModel = picker.FormKeySelectionViewModels.FirstOrDefault(x => x.Item == newFormKey);
                 }));

        public ReadOnlyObservableCollection<SelectedVm> FormKeySelectionViewModels { get; }

        public SelectedVm? SelectedFormKeyViewModel
        {
            get { return (SelectedVm?)GetValue(SelectedFormKeyViewModelProperty); }
            set { SetValue(SelectedFormKeyViewModelProperty, value); }
        }
        public static readonly DependencyProperty SelectedFormKeyViewModelProperty = DependencyProperty.Register(
            nameof(SelectedFormKeyViewModel), typeof(SelectedVm), typeof(FormKeyMultiPicker), 
            new FrameworkPropertyMetadata(default(SelectedVm?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, e) =>
                {
                    var picker = (FormKeyMultiPicker)d;
                    if (e.NewValue == null)
                    {
                        picker.SelectedFormKey = null;
                        return;
                    }
                    var newFormKey = (SelectedVm)e.NewValue;
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

        public class SelectedVm : ReactiveObject, ISelectedItem<FormKey>
        {
            [Reactive] public bool IsSelected { get; set; }
            public FormKey Item { get; }

            public SelectedVm(FormKey formKey)
            {
                Item = formKey;
            }
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
                .Select(x =>
                {
                    if (x is ObservableCollection<FormKey> obsCollection)
                    {
                        return obsCollection.ToObservableChangeSet();
                    }

                    if (x is IObservableCollection<FormKey> obsCollInterf)
                    {
                        return obsCollInterf.ToObservableChangeSet<IObservableCollection<FormKey>, FormKey>();
                    }

                    return Observable.Empty<IChangeSet<FormKey>>();
                })
                .Switch()
                .Transform(x => new SelectedVm(x))
                .Bind(out ReadOnlyObservableCollection<SelectedVm> selFormKeys)
                .Subscribe();
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
                Noggog.WPF.Drag.ListBoxDragDrop(_formKeyListBox, () => this.FormKeys as IList<FormKey>)
                    .DisposeWith(_templateDisposable);
            }
        }
    }
}
