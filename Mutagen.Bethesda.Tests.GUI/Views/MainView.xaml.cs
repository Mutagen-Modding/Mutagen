using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    public class MainViewBase : ReactiveUserControl<MainVM> { }

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MainViewBase
    {
        public MainView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.SettingsView.ViewModel = this.ViewModel;

                this.WhenAny(x => x.ViewModel.SelectedConfigPath)
                    .BindToStrict(this, x => x.SettingsPicker.PickerVM)
                    .DisposeWith(disposable);

                // Set up passthrough group pane
                this.WhenAny(x => x.ViewModel.Groups)
                    .BindToStrict(this, x => x.PassthroughGroupsList.ItemsSource)
                    .DisposeWith(disposable);
                this.WhenAny(x => x.ViewModel.AddPassthroughGroupCommand)
                    .BindToStrict(this, x => x.AddPassthroughButton.Command)
                    .DisposeWith(disposable);

                // Setup run button
                this.WhenAny(x => x.ViewModel.RunAllCommand)
                    .BindToStrict(this, x => x.RunButton.Command)
                    .DisposeWith(disposable);

                // Set up valid enabled states
                var enabledObs = this.WhenAny(x => x.ViewModel.SelectedSettings)
                    .Select(x => x != null)
                    .Replay(1)
                    .RefCount();
                enabledObs
                    .BindToStrict(this, x => x.RunButton.IsEnabled)
                    .DisposeWith(disposable);
                enabledObs
                    .BindToStrict(this, x => x.SettingsView.IsEnabled)
                    .DisposeWith(disposable);
                enabledObs
                    .BindToStrict(this, x => x.AddPassthroughButton.IsEnabled)
                    .DisposeWith(disposable);
            });
        }
    }
}
