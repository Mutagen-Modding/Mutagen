using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// An interface implemented by Major Records that have scripts
    /// </summary>
    public interface IScripted : IScriptedGetter
    {
        new VirtualMachineAdapter? VirtualMachineAdapter { get; set; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have scripts
    /// </summary>
    public interface IScriptedGetter
    {
        IVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
    }
}
