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
                this.WhenAnyFallback(x => x.ViewModel!.Path)
                    .BindTo(this, x => x.PathPicker.PickerVM)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, vm => vm.Do, v => v.DoCheckbox.IsChecked)
                    .DisposeWith(disposable);

                // Wire Delete Button
                this.WhenAnyFallback(x => x.ViewModel!.DeleteCommand)
                    .BindTo(this, v => v.DeleteButton.Command)
                    .DisposeWith(disposable);
            });
        }
    }
}
