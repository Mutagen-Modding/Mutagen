using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Aspects
{
    /// <summary>
    /// An attribute that marks an interface as one that should be considered by Source Generator systems
    /// to be a custom Aspect Interface defined by the user
    /// </summary>
    public class CustomAspectInterface : Attribute
    {
        public Type[] KnownTypes;
        
        public CustomAspectInterface(params Type[] knownTypes)
        {
            KnownTypes = knownTypes;
        }
    }
}