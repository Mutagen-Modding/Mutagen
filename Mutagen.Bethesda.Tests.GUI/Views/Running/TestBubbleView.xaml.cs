using Noggog;
using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    public class TestBubbleViewBase : ReactiveUserControl<TestVM> { }

    /// <summary>
    /// Interaction logic for TestBubbleView.xaml
    /// </summary>
    public partial class TestBubbleView : TestBubbleViewBase
    {
        public TestBubbleView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel.Test)
                    .Select(t =>
                    {
                        if (t.FilePath == null) return t.Name;
                        return $"{t.Name} {t.FilePath.Value.Name}";
                    })
                    .BindToStrict(this, x => x.Name.Text)
                    .DisposeWith(disposable);
                this.TopBorder.Events().MouseUp
                    .Unit()
                    .InvokeCommandStrict(this, x => x.ViewModel.SelectCommand)
                    .DisposeWith(disposable);
            });
        }
    }
}
