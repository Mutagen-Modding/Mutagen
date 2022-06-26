namespace Mutagen.Bethesda.Fallout4;

public partial class TintTemplateOption
{
    public enum Flag
    {
        OnOffOnly = 0x01,
        ChargenDetail = 0x02,
        TakesSkinTone = 0x04,
    }

    public enum TintSlot
    {
        ForeheadMask,
        EyesMask,
        NoseMask,
        EarsMask,
        CheeksMask,
        MouthMask,
        NeckMask,
        LipColor,
        CheekColor,
        Eyeliner,
        EyeSocketUpper,
        EyeSocketLower,
        SkinTone,
        Paint,
        LaughLines,
        CheekColorLower,
        Nose,
        Chin,
        Neck,
        Forehead,
        Dirt,
        Scars,
        FaceDetail,
        Brows,
        Wrinkles,
        Beards
    }
}
