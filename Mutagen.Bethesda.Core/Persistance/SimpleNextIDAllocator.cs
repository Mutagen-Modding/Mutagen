using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Persistance
{
    public class SimpleNextIDAllocator : IFormKeyAllocator
    {
        private IMod mod;

        public SimpleNextIDAllocator(IMod mod)
        {
            this.mod = mod;
        }

        public FormKey GetNextFormKey()
        {
            return new FormKey(
                this.mod.ModKey,
                this.mod.NextObjectID++);
        }

        public FormKey GetNextFormKey(string editorID) => GetNextFormKey();
    }
}
