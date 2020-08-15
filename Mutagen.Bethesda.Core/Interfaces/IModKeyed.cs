using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IModKeyed
    {
        /// <summary>
        /// The associated ModKey
        /// </summary>
        ModKey ModKey { get; }
    }
}
