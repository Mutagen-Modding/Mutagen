namespace Mutagen.Bethesda.Fallout76;

public interface IHaveVirtualMachineAdapterGetter : IFallout76MajorRecordGetter
{
    IAVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
}

public interface IHaveVirtualMachineAdapter : IHaveVirtualMachineAdapterGetter, IFallout76MajorRecord
{
    IAVirtualMachineAdapter? VirtualMachineAdapter { get; }
}