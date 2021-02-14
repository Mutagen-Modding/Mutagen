using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public class FormKeyMultiPicker : AbstractFormKeyPicker
    {
        public IEnumerable FormKeys
        {
            get => (IEnumerable)GetValue(FormKeysProperty);
            set => SetValue(FormKeysProperty, value);
        }
        public static readonly DependencyProperty FormKeysProperty = DependencyProperty.Register(nameof(FormKeys), typeof(IEnumerable), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(IEnumerable)));

        public Brush SelectedForegroundBrush
        {
            get => (Brush)GetValue(SelectedForegroundBrushProperty);
            set => SetValue(SelectedForegroundBrushProperty, value);
        }
        public static readonly DependencyProperty SelectedForegroundBrushProperty = DependencyProperty.Register(nameof(SelectedForegroundBrush), typeof(Brush), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(Application.Current.Resources[Noggog.WPF.Brushes.Constants.Processing]));

        public ICommand AddFormKeyCommand
        {
            get => (ICommand)GetValue(AddFormKeyCommandProperty);
            set => SetValue(AddFormKeyCommandProperty, value);
        }
        public static readonly DependencyProperty AddFormKeyCommandProperty = DependencyProperty.Register(nameof(AddFormKeyCommand), typeof(ICommand), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FormKeyMultiPicker()
        {
            PickerClickCommand = ReactiveCommand.Create((object o) =>
            {
                switch (o)
                {
                    case IMajorRecordIdentifier identifier:
                        if (FormKeys is not ICollection<FormKeyItemViewModel> formKeys) return;
                        formKeys.Add(new FormKeyItemViewModel(identifier.FormKey));
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
                    if (FormKeys is not ICollection<FormKeyItemViewModel> formKeys) return;
                    formKeys.Add(new FormKeyItemViewModel(FormKey));
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
