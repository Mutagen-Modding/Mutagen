using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mutagen.Bethesda.WPF
{
    [TemplatePart(Name = "PART_AddedFormKeyListBox", Type = typeof(ListBox))]
    public class FormKeyMultiPicker : AbstractFormKeyPicker
    {
        public IEnumerable FormKeys
        {
            get => (IEnumerable)GetValue(FormKeysProperty);
            set => SetValue(FormKeysProperty, value);
        }
        public static readonly DependencyProperty FormKeysProperty = DependencyProperty.Register(nameof(FormKeys), typeof(IEnumerable), typeof(FormKeyMultiPicker),
             new FrameworkPropertyMetadata(default(IEnumerable), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FormKeyMultiPicker()
        {
            PickerClickCommand = ReactiveCommand.Create((object o) =>
            {
                switch (o)
                {
                    case IMajorRecordIdentifier identifier:
                        if (FormKeys is not ICollection<FormKey> formKeys) return;
                        formKeys.Add(identifier.FormKey);
                        break;
                    default:
                        break;
                }
            });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var formKeyBox = GetTemplateChild("PART_AddedFormKeyListBox") as ListBox;
            if (formKeyBox != null)
            {
                Noggog.WPF.Drag.ListBoxDragDrop(formKeyBox, () => this.FormKeys as IList<FormKey>)
                    .DisposeWith(_templateDisposable);
            }
        }
    }
}
