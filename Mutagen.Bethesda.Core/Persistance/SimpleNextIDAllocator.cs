using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Persistance
{
    public class SimpleNextIDAllocator : IFormKeyAllocator
    {
        public IMod Mod { get; }

        public SimpleNextIDAllocator(IMod mod)
        {
            this.Mod = mod;
        }

        public FormKey GetNextFormKey()
        {
            return new FormKey(
                this.Mod.ModKey,
                checked(this.Mod.NextObjectID++));
        }

        public FormKey GetNextFormKey(string editorID) => GetNextFormKey();
    }
}
