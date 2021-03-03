using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for something that can allocate new FormKeys when requested shared between multiple programs
    /// </summary>
    public interface ISharedFormKeyAllocator : IFormKeyAllocator
    {

    }
}
