using Loqui;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary> 
    /// A static class to house initialization warmup logic 
    /// </summary> 
    public static class WarmupAll
    {
        /// <summary> 
        /// Will initialize internals in a more efficient way that avoids reflection. 
        /// Not required to call, but can be used to warm up ahead of time. 
        /// </summary> 
        public static void Init()
        {
            Loqui.Initialization.SpinUp(
                new ProtocolDefinition_Oblivion(),
                new ProtocolDefinition_Skyrim());
        }
    }
}
