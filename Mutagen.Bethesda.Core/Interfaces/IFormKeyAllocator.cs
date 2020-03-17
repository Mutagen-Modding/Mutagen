using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IFormKeyAllocator
    {
        FormKey GetNextFormKey();
    }
}
