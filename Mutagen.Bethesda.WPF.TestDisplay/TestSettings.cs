using System;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace Mutagen.Bethesda.WPF.TestDisplay
{
    public class TestSettings
    {
        public bool MyBool;
        public string MyString = string.Empty;
        public FormKey MyFormKey;
        public IFormLinkGetter<IArmorGetter> MyArmor = FormLink<IArmorGetter>.Null;
    }
}
