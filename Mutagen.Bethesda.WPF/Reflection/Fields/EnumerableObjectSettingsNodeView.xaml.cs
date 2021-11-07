using Noggog.WPF;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Mutagen.Bethesda.WPF.Reflection.Fields
{
    public class EnumerableObjectSettingsNodeViewBase : NoggogUserControl<EnumerableObjectSettingsVM> { }

    /// <summary>
    /// EnumerableInteraction logic for EnumerableNumericSettingsNodeView.xaml
    /// </summary>
    public partial class EnumerableObjectSettingsNodeView : EnumerableObjectSettingsNodeViewBase
    {
        public EnumerableObjectSettingsNodeView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel!.FocusSettingCommand)
                    .BindTo(this, x => x.SettingNameButton.Command)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.Meta.DisplayName)
                    .BindTo(this, x => x.SettingNameBlock.Text)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.Values.Count)
                    .BindTo(this, x => x.SettingsListBox.AlternationCount)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.Values)
                    .BindTo(this, x => x.SettingsListBox.ItemsSource)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.AddCommand)
                    .BindTo(this, x => x.AddButton.Command)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.DeleteCommand)
                    .BindTo(this, x => x.DeleteButton.Command)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.DeleteCommand.CanExecute)
                    .Switch()
                    .CombineLatest(this.WhenAnyValue(x => x.ViewModel!.IsFocused),
                        (canExecute, focused) => canExecute && focused)
                    .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                    .BindTo(this, x => x.DeleteButton.Visibility)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.SettingsListBox.SelectedItems)
                    .BindTo(this, x => x.ViewModel!.SelectedValues)
                    .DisposeWith(disposable);
                // Focus and select new item on add
                this.WhenAnyValue(x => x.ViewModel!.AddCommand)
                    .Select(x => x.EndingExecution())
                    .Switch()
                    .Delay(TimeSpan.FromMilliseconds(50), RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        var item = this.ViewModel?.Values.LastOrDefault();
                        if (item == null) return;
                        this.SettingsListBox.SelectedItem = item;
                        var listBoxItem = this.SettingsListBox
                            .ItemContainerGenerator
                            .ContainerFromItem(item) as ListBoxItem;
                        if (listBoxItem == null) return;
                        listBoxItem.GetChildOfType<WatermarkTextBox>()?.Focus();
                    })
                    .DisposeWith(disposable);

                Noggog.WPF.Drag.ListBoxDragDrop<SelectionWrapper>(SettingsListBox)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.IsFocused)
                    .Select((sel) => sel ? Visibility.Visible : Visibility.Collapsed)
                    .BindTo(this, x => x.SettingsListBox.Visibility)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.IsFocused)
                    .Select((sel) => sel ? Visibility.Visible : Visibility.Collapsed)
                    .BindTo(this, x => x.AddButton.Visibility)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.Values.Count)
                    .Select(x =>
                    {
                        if (x == 0) return "No Items";
                        return $"{x} Items";
                    })
                    .BindTo(this, x => x.NumItemsBlock.Text)
                    .DisposeWith(disposable);
            });
        }
    }
}
