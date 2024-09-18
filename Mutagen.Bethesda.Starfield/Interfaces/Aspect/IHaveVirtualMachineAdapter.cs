namespace Mutagen.Bethesda.Starfield;

public interface IHaveVirtualMachineAdapterGetter : IStarfieldMajorRecordGetter
{
    IAVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
}

public interface IHaveVirtualMachineAdapter : IHaveVirtualMachineAdapterGetter, IStarfieldMajorRecord
{
    IAVirtualMachineAdapter? VirtualMachineAdapter { get; }
}