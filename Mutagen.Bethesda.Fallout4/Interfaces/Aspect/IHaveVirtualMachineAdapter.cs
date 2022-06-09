namespace Mutagen.Bethesda.Fallout4;

public interface IHaveVirtualMachineAdapterGetter
{
    IAVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
}