using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    public class PassthroughGroupViewBase : ReactiveUserControl<PassthroughGroupVM> { }

    /// <summary>
    /// Interaction logic for PassthroughGroupView.xaml
    /// </summary>
    public partial class PassthroughGroupView : PassthroughGroupViewBase
    {
        public PassthroughGroupView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.BindStrict(this.ViewModel, vm => vm.NicknameSuffix, view => view.NicknameSuffix.Text)
                    .DisposeWith(disposable);
                this.WhenAny(x => x.ViewModel.AddPassthroughCommand)
                    .BindToStrict(this, x => x.AddButton.Command)
                    .DisposeWith(disposable);
                this.WhenAny(x => x.ViewModel.Passthroughs)
                    .BindToStrict(this, x => x.PassthroughsControl.ItemsSource)
                    .DisposeWith(disposable);
                this.BindStrict(this.ViewModel, vm => vm.Do, view => view.DoCheckbox.IsChecked)
                    .DisposeWith(disposable);

                // Set up combobox
                this.GameReleasesCombobox.ItemsSource = PassthroughGroupVM.GameReleases;
                this.BindStrict(this.ViewModel, vm => vm.GameRelease, v => v.GameReleasesCombobox.SelectedItem)
                    .DisposeWith(disposable);

                // Wire Delete Button
                this.WhenAny(x => x.ViewModel.DeleteCommand)
                    .BindToStrict(this, v => v.DeleteGroupButton.Command)
                    .DisposeWith(disposable);
                this.WhenAny(x => x.TopGrid.IsMouseOver)
                    .Select(b => b ? Visibility.Visible : Visibility.Hidden)
                    .BindToStrict(this, v => v.DeleteGroupButton.Visibility)
                    .DisposeWith(disposable);
            });
        }
    }
}
