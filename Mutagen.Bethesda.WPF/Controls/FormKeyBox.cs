using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using System.Reactive;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Noggog;

namespace Mutagen.Bethesda.WPF
{
    public class FormKeyBox : NoggogControl
    {
        private bool _blockSync = false;

        public FormKey FormKey
        {
            get => (FormKey)GetValue(FormKeyProperty);
            set => SetValue(FormKeyProperty, value);
        }
        public static readonly DependencyProperty FormKeyProperty = DependencyProperty.Register(nameof(FormKey), typeof(FormKey), typeof(FormKeyBox),
             new FrameworkPropertyMetadata(default(FormKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string RawString
        {
            get => (string)GetValue(RawStringProperty);
            set => SetValue(RawStringProperty, value);
        }
        public static readonly DependencyProperty RawStringProperty = DependencyProperty.Register(nameof(RawString), typeof(string), typeof(FormKeyBox),
             new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ErrorResponse Error
        {
            get => (ErrorResponse)GetValue(ErrorProperty);
            protected set => SetValue(ErrorPropertyKey, value);
        }
        public static readonly DependencyPropertyKey ErrorPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Error), typeof(ErrorResponse), typeof(FormKeyBox),
             new FrameworkPropertyMetadata(default(ErrorResponse), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ErrorProperty = ErrorPropertyKey.DependencyProperty;

        static FormKeyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormKeyBox), new FrameworkPropertyMetadata(typeof(FormKeyBox)));
        }

        public FormKeyBox()
        {
            this.WhenAnyValue(x => x.FormKey)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (_blockSync) return;
                    _blockSync = true;
                    if (x.IsNull)
                    {
                        if (RawString != String.Empty)
                        {
                            RawString = string.Empty;
                        }
                    }
                    else
                    {
                        var str = x.ToString();
                        if (!str.Equals(RawString, StringComparison.OrdinalIgnoreCase))
                        {
                            RawString = str;
                        }
                    }
                    _blockSync = false;
                })
                .DisposeWith(_unloadDisposable);
            this.WhenAnyValue(x => x.RawString)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    if (_blockSync) return;
                    _blockSync = true;
                    if (FormKey.TryFactory(x, out var formKey))
                    {
                        if (formKey != FormKey)
                        {
                            FormKey = formKey;
                        }
                        if (Error.Failed)
                        {
                            Error = ErrorResponse.Success;
                        }
                    }
                    else
                    {
                        if (x.IsNullOrWhitespace())
                        {
                            if (Error.Failed)
                            {
                                Error = ErrorResponse.Success;
                            }
                        }
                        else
                        {
                            if (Error.Succeeded)
                            {
                                Error = ErrorResponse.Fail("Could not convert to FormKey");
                            }
                        }
                        if (!FormKey.IsNull)
                        {
                            FormKey = FormKey.Null;
                        }
                    }
                    _blockSync = false;
                })
                .DisposeWith(_unloadDisposable);
        }
    }
}
