using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    public class PassthroughViewBase : ReactiveUserControl<PassthroughVM> { }

    /// <summary>
    /// Interaction logic for PassthroughView.xaml
    /// </summary>
    public partial class PassthroughView : PassthroughViewBase
    {
        [Reactive]
        public bool Doing { get; set; }

        public PassthroughView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel.Path)
                    .BindToStrict(this, x => x.PathPicker.PickerVM)
                    .DisposeWith(disposable);
                this.BindStrict(this.ViewModel, vm => vm.Do, v => v.DoCheckbox.IsChecked)
                    .DisposeWith(disposable);

                // Wire Delete Button
                this.WhenAnyValue(x => x.ViewModel.DeleteCommand)
                    .BindToStrict(this, v => v.DeleteButton.Command)
                    .DisposeWith(disposable);
            });
        }
    }
}
