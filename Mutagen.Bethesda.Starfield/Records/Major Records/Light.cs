namespace Mutagen.Bethesda.Starfield;

partial class Light
{
    [Flags]
    public enum MajorFlag
    {
        RandomAnimStart = 0x0001_0000,
        Obstacle = 0x0200_0000,
        PortalStrict = 0x1000_0000
    }

    [Flags]
    public enum Flag
    {
        CanBeCarried = 0x0000_0002,
        OffByDefault = 0x0000_0020,
        DisableSpecular = 0x0000_0040,
        DisableDistanceAttenuation = 0x0000_0080,
        IsDirectSpotlight = 0x0000_0100,
        UsePbrValue = 0x0000_0200,
        FocusSpotlightBeam = 0x0000_0400,
        ExternallyControlled = 0x0000_2000,
    }

    public enum LightType
    {
        Omnidirectional,
        ShadowSpotlight,
        NonShadowSpotlight
    }

    public enum FlickerEffectOption
    {
        None,
        Flicker,
        Pulse
    }

    public enum LightLayer
    {
        Default = 0x1,
        Helmet = 0x2,
        ShipInterior = 0x4,
        ShipExterior = 0x8,
        All = 0x10,
    }
}