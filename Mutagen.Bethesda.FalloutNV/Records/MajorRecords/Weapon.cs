
using Mutagen.Bethesda.FalloutNV.Internals;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.FalloutNV;

public partial class Weapon
{
    partial void CustomCtor()
    {
        _PowerAttackAnimationOverride = PowerAttackAnim.Default;
    }

    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
    }

    public enum WeaponAnimationType
    {
        HandToHand = 0,
        Melee1Hand = 1,
        Melee2Hand = 2,
        PistolBallistic1Hand = 3,
        PistolEnergy1Hand = 4,
        RifleBallistic2Hand = 5,
        RifleAutomatic2Hand = 6,
        RifleEnergy2Hand = 7,
        Handle2Hand = 8,
        Launcher2Hand = 9,
        GrenadeThrow1Hand = 10,
        LandMine1Hand = 11,
        MineDrop1Hand = 12,
    }

    [Flags]
    public enum WeaponFlag : uint
    {
        // Lower byte (Flags1 in binary, offset 12, 1 byte)
        IgnoresNormalWeaponResistance = 0x0000_0001,
        IsAutomatic = 0x0000_0002,
        HasScope = 0x0000_0004,
        CantDrop = 0x0000_0008,
        HideBackpack = 0x0000_0010,
        EmbeddedWeapon = 0x0000_0020,
        DontUse1stPersonISAnimations = 0x0000_0040,
        NonPlayable = 0x0000_0080,
        // Upper bits (Flags2 in binary, offset 56, 4 bytes, shifted left 8)
        PlayerOnly = 0x0000_0100,
        NpcsUseAmmo = 0x0000_0200,
        NoJamAfterReload = 0x0000_0400,
        OverrideActionPoints = 0x0000_0800,
        MinorCrime = 0x0000_1000,
        RangeFixed = 0x0000_2000,
        NotUsedInNormalCombat = 0x0000_4000,
        OverrideDamageToWeaponMult = 0x0000_8000,
        DontUse3rdPersonISAnimations = 0x0001_0000,
        ShortBurst = 0x0002_0000,
        RumbleAlternate = 0x0004_0000,
        LongBurst = 0x0008_0000,
        ScopeHasNightVision = 0x0010_0000,
        ScopeFromMod = 0x0020_0000,
    }

    public enum WeaponOnHit
    {
        NormalFormulaBehavior = 0,
        DismemberOnly = 1,
        ExplodeOnly = 2,
        NoDismemberExplode = 3,
    }

    public enum WeaponRumblePattern
    {
        Constant = 0,
        Square = 1,
        Triangle = 2,
        Sawtooth = 3,
    }

    public enum ReloadAnim : byte
    {
        ReloadA = 0,
        ReloadB = 1,
        ReloadC = 2,
        ReloadD = 3,
        ReloadE = 4,
        ReloadF = 5,
        ReloadG = 6,
        ReloadH = 7,
        ReloadI = 8,
        ReloadJ = 9,
        ReloadK = 10,
        ReloadL = 11,
        ReloadM = 12,
        ReloadN = 13,
        ReloadO = 14,
        ReloadP = 15,
        ReloadQ = 16,
        ReloadR = 17,
        ReloadS = 18,
        ReloadW = 19,
        ReloadX = 20,
        ReloadY = 21,
        ReloadZ = 22,
        None = 255,
    }

    public enum AttackAnim : byte
    {
        AttackLeft = 26,
        AttackRight = 32,
        Attack3 = 38,
        Attack4 = 44,
        Attack5 = 50,
        Attack6 = 56,
        Attack7 = 62,
        Attack8 = 68,
        AttackLoop = 74,
        AttackSpin = 80,
        AttackSpin2 = 86,
        PlaceMine = 97,
        PlaceMine2 = 103,
        AttackThrow = 109,
        AttackThrow2 = 115,
        AttackThrow3 = 121,
        AttackThrow4 = 127,
        AttackThrow5 = 133,
        Attack9 = 144,
        AttackThrow6 = 150,
        AttackThrow7 = 156,
        AttackThrow8 = 162,
        Default = 255,
    }

    public enum EmbeddedWeaponAV : byte
    {
        Perception = 0,
        Endurance = 1,
        LeftAttack = 2,
        RightAttack = 3,
        LeftMobility = 4,
        RightMobility = 5,
        Brain = 6,
    }

    public enum PowerAttackAnim : uint
    {
        AttackCustom1Power = 97,
        AttackCustom2Power = 98,
        AttackCustom3Power = 99,
        AttackCustom4Power = 100,
        AttackCustom5Power = 101,
        Default = 255,
    }

    public enum ModEffect
    {
        None = 0,
        IncreaseWeaponDamage = 1,
        IncreaseClipCapacity = 2,
        DecreaseSpread = 3,
        DecreaseWeight = 4,
        RegenerateAmmoShots = 5,
        RegenerateAmmoSeconds = 6,
        DecreaseEquipTime = 7,
        IncreaseRateOfFire = 8,
        IncreaseProjectileSpeed = 9,
        IncreaseMaxCondition = 10,
        Silence = 11,
        SplitBeam = 12,
        VatsBonus = 13,
        IncreaseZoom = 14,
        Suppressor = 16,
    }

    // Constants for flags combination
    internal const uint UpperFlagMask = 0xFFFF_FF00;
    internal const byte UpperFlagShift = 8;
}

partial class WeaponBinaryCreateTranslation
{
    // Shared VATS field reader — handles 16 or 20 byte variants
    internal static WeaponVats ParseVatsFields(MutagenFrame dataFrame)
    {
        var vats = new WeaponVats();
        vats.VatsEffect.SetTo(FormLinkBinaryTranslation.Instance.Parse(reader: dataFrame));
        vats.VatsSkill = dataFrame.ReadFloat();
        vats.VatsDamageMult = dataFrame.ReadFloat();
        vats.VatsAp = dataFrame.ReadFloat();
        if (dataFrame.Remaining >= 4)
        {
            vats.VatsSilent = dataFrame.ReadBoolean();
            vats.VatsModRequired = dataFrame.ReadBoolean();
            vats.VatsUnused = dataFrame.ReadUInt16();
        }
        return vats;
    }

    // VATS: handle 16 or 20 byte variants
    public static partial void FillBinaryVatsCustom(MutagenFrame frame, IWeaponInternal item, PreviousParse lastParsed)
    {
        var subHeader = frame.ReadSubrecordHeader(RecordTypes.VATS);
        var dataFrame = frame.SpawnWithLength(subHeader.ContentLength);
        item.Vats = ParseVatsFields(dataFrame);
    }

    // DNAM Flags: read 1 byte, store in lower bits of unified flag
    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IWeaponInternal item)
    {
        item.Flags = (Weapon.WeaponFlag)frame.ReadUInt8();
    }

    // DNAM Flags2: read 4 bytes, shift left 8, merge with existing Flags
    public static partial void FillBinaryFlags2Custom(MutagenFrame frame, IWeaponInternal item)
    {
        var flags2 = frame.ReadUInt32();
        flags2 <<= Weapon.UpperFlagShift;
        var flags = (uint)item.Flags;
        flags &= ~Weapon.UpperFlagMask;
        flags |= flags2;
        item.Flags = (Weapon.WeaponFlag)flags;
    }
}

partial class WeaponBinaryWriteTranslation
{
    // VATS: write all fields (VATS only exists in FNV records)
    public static partial void WriteBinaryVatsCustom(MutagenWriter writer, IWeaponGetter item)
    {
        if (item.Vats is not {} vats) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.VATS))
        {
            FormLinkBinaryTranslation.Instance.Write(writer: writer, item: vats.VatsEffect);
            writer.Write(vats.VatsSkill);
            writer.Write(vats.VatsDamageMult);
            writer.Write(vats.VatsAp);
            writer.Write(vats.VatsSilent);
            writer.Write(vats.VatsModRequired);
            writer.Write(vats.VatsUnused);
        }
    }

    // DNAM Flags: write lower 8 bits as 1 byte
    public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IWeaponGetter item)
    {
        writer.Write((byte)item.Flags);
    }

    // DNAM Flags2: extract upper bits, shift right 8, write as 4 bytes
    public static partial void WriteBinaryFlags2Custom(MutagenWriter writer, IWeaponGetter item)
    {
        var flags = (uint)item.Flags;
        flags &= Weapon.UpperFlagMask;
        flags >>= Weapon.UpperFlagShift;
        writer.Write(flags);
    }
}

partial class WeaponBinaryOverlay
{
    private IWeaponVatsGetter? _Vats;

    partial void VatsCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        var vatsRec = stream.ReadSubrecord(RecordTypes.VATS);

        var vats = new WeaponVats();
        vats.VatsEffect.SetTo(FormKeyBinaryTranslation.Instance.Parse(vatsRec.Content.Slice(0, 4), _package.MetaData.MasterReferences));
        vats.VatsSkill = vatsRec.Content.Slice(4, 4).Float();
        vats.VatsDamageMult = vatsRec.Content.Slice(8, 4).Float();
        vats.VatsAp = vatsRec.Content.Slice(12, 4).Float();
        if (vatsRec.Content.Length >= 20)
        {
            vats.VatsSilent = vatsRec.Content[16] >= 1;
            vats.VatsModRequired = vatsRec.Content[17] >= 1;
            vats.VatsUnused = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(vatsRec.Content.Slice(18, 2));
        }
        _Vats = vats;
    }

    public partial IWeaponVatsGetter? GetVatsCustom() => _Vats;

    // DNAM Flags overlay: combine Flags1 (offset 0xC, 1 byte) and Flags2 (offset 0x38, 4 bytes)
    public partial Weapon.WeaponFlag GetFlagsCustom()
    {
        var flags1 = (uint)_recordData[_FlagsLocation];
        var flags2 = (uint)System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(
            _recordData.Slice(_FlagsLocation + 0x2C, 4));
        flags2 <<= Weapon.UpperFlagShift;
        return (Weapon.WeaponFlag)(flags1 | flags2);
    }
}
