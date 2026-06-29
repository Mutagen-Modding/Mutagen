using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Fallout3;

public partial class IdleAnimation
{
    public enum AnimationGroupSection
    {
        Idle = 0,
        Movement = 1,
        LeftArm = 2,
        LeftHand = 3,
        Weapon = 4,
        WeaponUp = 5,
        WeaponDown = 6,
        SpecialIdle = 7,
        WholeBody = 20,
        UpperBody = 21,
    }
}

partial class IdleAnimationDataBinaryCreateTranslation
{
    public const byte SectionMask = 0x3F;
    public const byte LooseIdleFlag = 0x40;
    public const byte ReturnFileFlag = 0x80;

    public static IdleAnimation.AnimationGroupSection GetAnimationGroupSection(byte b)
        => (IdleAnimation.AnimationGroupSection)(b & SectionMask);

    public static partial void FillBinaryGroupSectionParserCustom(MutagenFrame frame, IIdleAnimationData item)
    {
        byte b = frame.ReadUInt8();
        item.AnimationGroupSection = GetAnimationGroupSection(b);
        item.LooseIdle = (b & LooseIdleFlag) != 0;
        // xEdit: bit 7 clear => "Must return a file" (active-low).
        item.MustReturnFile = (b & ReturnFileFlag) == 0;
    }
}

partial class IdleAnimationDataBinaryWriteTranslation
{
    public static partial void WriteBinaryGroupSectionParserCustom(MutagenWriter writer, IIdleAnimationDataGetter item)
    {
        byte b = (byte)((byte)item.AnimationGroupSection & IdleAnimationDataBinaryCreateTranslation.SectionMask);
        if (item.LooseIdle) b |= IdleAnimationDataBinaryCreateTranslation.LooseIdleFlag;
        if (!item.MustReturnFile) b |= IdleAnimationDataBinaryCreateTranslation.ReturnFileFlag;
        writer.Write(b);
    }
}

partial class IdleAnimationDataBinaryOverlay
{
    public IdleAnimation.AnimationGroupSection AnimationGroupSection =>
        IdleAnimationDataBinaryCreateTranslation.GetAnimationGroupSection(_structData.Span[0]);

    public bool LooseIdle =>
        (_structData.Span[0] & IdleAnimationDataBinaryCreateTranslation.LooseIdleFlag) != 0;

    public bool MustReturnFile =>
        (_structData.Span[0] & IdleAnimationDataBinaryCreateTranslation.ReturnFileFlag) == 0;
}
