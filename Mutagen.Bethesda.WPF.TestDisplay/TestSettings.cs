using System;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Mutagen.Bethesda.WPF.TestDisplay
{
    public class TestSettings
    {
        [Tooltip("This is my bool")]
        public bool MyBool;
        
        [Tooltip("This is my string")]
        public string MyString = string.Empty;
        
        [Tooltip("This is my FormKey")]
        public FormKey MyFormKey;
        
        [Tooltip("This is my Armor")]
        public IFormLinkGetter<IArmorGetter> MyArmor = FormLink<IArmorGetter>.Null;
    }
}
