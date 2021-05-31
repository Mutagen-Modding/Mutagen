using Mutagen.Bethesda.WPF.Reflection.Fields;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;

namespace Mutagen.Bethesda.WPF.Reflection
{
    public class ReflectionSettingsVM : ViewModel
    {
        public ObjectSettingsVM ObjVM { get; }

        [Reactive]
        public SettingsNodeVM SelectedSettings { get; set; }

        [Reactive]
        public SettingsNodeVM? ScrolledToSettings { get; set; }

        public ReflectionSettingsVM(ReflectionSettingsParameters param)
        {
            ObjVM = new ObjectSettingsVM(
                param with 
                {
                    MainVM = this
                },
                FieldMeta.Empty with 
                { 
                    DisplayName = "Top Level",
                    MainVM = this
                });
            CompositeDisposable.Add(ObjVM);
            SelectedSettings = ObjVM;
        }
    }
}
