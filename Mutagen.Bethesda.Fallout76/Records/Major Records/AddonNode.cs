namespace Mutagen.Bethesda.Fallout76;

partial class AddonNode
{
    [Flags]
    public enum MajorFlag
    {
    }

    public enum Flag
    {
        NoMasterParticleSystem = 0,
        MasterParticleSystem = 1,
        AlwaysLoaded = 2,
        MasterParticleSystemAndAlwaysLoaded = 3,
    }
}

partial class AddonNodeBinaryOverlay
{
}
