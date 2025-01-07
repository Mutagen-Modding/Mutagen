namespace Mutagen.Bethesda.Skyrim;

public interface IHaveVirtualMachineAdapterGetter : ISkyrimMajorRecordGetter
{
    IAVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
}

public interface IHaveVirtualMachineAdapter : IHaveVirtualMachineAdapterGetter, ISkyrimMajorRecord
{
    IAVirtualMachineAdapter? VirtualMachineAdapter { get; }
}