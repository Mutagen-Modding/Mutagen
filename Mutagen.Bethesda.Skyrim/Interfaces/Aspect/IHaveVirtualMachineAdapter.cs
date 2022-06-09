namespace Mutagen.Bethesda.Skyrim;

public interface IHaveVirtualMachineAdapterGetter
{
    IAVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
}