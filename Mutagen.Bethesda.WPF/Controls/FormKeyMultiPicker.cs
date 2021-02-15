using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mutagen.Bethesda.WPF
{
    public class FormKeyItemViewModel : ViewModel
    {
        [Reactive]
        public bool IsSelected { get; set; }

        public FormKey FormKey { get; }

        public FormKeyItemViewModel(FormKey formKey)
        {
            FormKey = formKey;
        }
    }

    [TemplatePart(Name = "PART_AddedFormKeyListBox", Type = typeof(ListBox))]
    public class FormKeyMultiPicker : AFormKeyPicker
    {
        public ICollection<FormKeyItemViewModel> FormKeys
        {
            get => (ICollection<FormKeyItemViewModel>)GetValue(FormKeysProperty);
            set => SetValue(FormKeysProperty, value);
        }
        public static readonly DependencyProperty FormKeysProperty = DependencyProperty.Register(nameof(FormKeys), typeof(ICollection<FormKeyItemViewModel>), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ICollection<FormKeyItemViewModel>)));

        public FormKeyItemViewModel SelectedFormKey
        {
            get => (FormKeyItemViewModel)GetValue(SelectedFormKeyProperty);
            set => SetValue(SelectedFormKeyProperty, value);
        }
        public static readonly DependencyProperty SelectedFormKeyProperty = DependencyProperty.Register(nameof(SelectedFormKey), typeof(FormKeyItemViewModel), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(FormKeyItemViewModel), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
        }

        public FormKeyMultiPicker()
        {
            PickerClickCommand = ReactiveCommand.Create((object o) =>
            {
                switch (o)
                {
                    case IMajorRecordIdentifier identifier:
                        FormKeys.Add(new FormKeyItemViewModel(identifier.FormKey));
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
                    FormKeys.Add(new FormKeyItemViewModel(FormKey));
                });
            DeleteSelectedItemsCommand = ReactiveCommand.Create(
                canExecute: this.WhenAnyValue(x => x.SelectedFormKey)
                    .Select(x => x != null),
                execute: () =>
                {
                    var toDelete = FormKeys.Where(f => f.IsSelected).ToArray();
                    foreach (var item in toDelete)
                    {
                        FormKeys.Remove(item);
                    }
                });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var formKeyBox = GetTemplateChild("PART_AddedFormKeyListBox") as ListBox;
            if (formKeyBox != null)
            {
                Noggog.WPF.Drag.ListBoxDragDrop(formKeyBox, () => this.FormKeys as IList<FormKeyItemViewModel>)
                    .DisposeWith(_templateDisposable);
            }
        }
    }
}
