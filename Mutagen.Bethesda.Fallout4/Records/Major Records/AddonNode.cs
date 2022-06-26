namespace Mutagen.Bethesda.Fallout4;

partial class AddonNode
{
    [Flags]
    public enum Flag
    {
        NoMasterParticleSystem = 0,
        MasterParticleSystem = 1,
        AlwaysLoaded = 2,
        MasterParticleSystemAndAlwaysLoaded = 3,
    }
}
