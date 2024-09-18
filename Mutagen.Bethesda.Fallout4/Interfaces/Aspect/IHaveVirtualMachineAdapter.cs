namespace Mutagen.Bethesda.Fallout4;

public interface IHaveVirtualMachineAdapterGetter : IFallout4MajorRecordGetter
{
    IAVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
}

public interface IHaveVirtualMachineAdapter : IHaveVirtualMachineAdapterGetter, IFallout4MajorRecord
{
    IAVirtualMachineAdapter? VirtualMachineAdapter { get; }
}