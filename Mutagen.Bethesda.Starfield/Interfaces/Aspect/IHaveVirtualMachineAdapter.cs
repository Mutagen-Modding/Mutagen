namespace Mutagen.Bethesda.Starfield;

public interface IHaveVirtualMachineAdapterGetter
{
    IAVirtualMachineAdapterGetter? VirtualMachineAdapter { get; }
}