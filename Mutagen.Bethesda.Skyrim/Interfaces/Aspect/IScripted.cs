using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Skyrim;

/// <summary>
/// An interface implemented by Major Records that have scripts
/// </summary>
public interface IScripted : IScriptedGetter, IMajorRecordQueryable
{
    new VirtualMachineAdapter? VirtualMachineAdapter { get; set; }
}

/// <summary>
/// An interface implemented by Major Records that have scripts
/// </summary>
public interface IScriptedGetter : IMajorRecordQueryableGetter
{
    IVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
}