using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

partial class NpcRaceOverride
{
}

partial class NpcRaceOverrideBinaryCreateTranslation
{
    public enum OverrideFlags
    {
        Size = 0x0001,
        Unknown1 = 0x0002,
        Unknown2 = 0x0004,
        UnarmedWeapon = 0x0008,
        Flag = 0x0010,
        Unknown5 = 0x0020,
        General = 0x0040,
        Unknown7 = 0x0080,
    }
    
    public static partial void FillBinaryFlagParseCustom(
        MutagenFrame frame,
        INpcRaceOverride item)
    {
        var flags = (OverrideFlags)frame.ReadInt32();
        if (flags.HasFlag(OverrideFlags.Size))
        {
            item.Size = NpcRaceOverrideSize.CreateFromBinary(frame);
        }
        if (flags.HasFlag(OverrideFlags.Unknown1))
        {
            item.Unknown1 = NpcRaceOverrideUnknown1.CreateFromBinary(frame);
        }
        if (flags.HasFlag(OverrideFlags.Unknown2))
        {
            item.Unknown2 = NpcRaceOverrideUnknown2.CreateFromBinary(frame);
        }
        if (flags.HasFlag(OverrideFlags.UnarmedWeapon))
        {
            item.UnarmedWeapon = NpcRaceOverrideUnarmedWeapon.CreateFromBinary(frame);
        }
        if (flags.HasFlag(OverrideFlags.Flag))
        {
            item.Flag = NpcRaceOverrideFlag.CreateFromBinary(frame);
        }
        if (flags.HasFlag(OverrideFlags.Unknown5))
        {
            item.Unknown5 = NpcRaceOverrideUnknown5.CreateFromBinary(frame);
        }
        if (flags.HasFlag(OverrideFlags.General))
        {
            item.General = NpcRaceOverrideGeneral.CreateFromBinary(frame);
        }
        if (flags.HasFlag(OverrideFlags.Unknown7))
        {
            item.Unknown7 = NpcRaceOverrideUnknown7.CreateFromBinary(frame);
        }
    }
}

partial class NpcRaceOverrideBinaryWriteTranslation
{
    public static partial void WriteBinaryFlagParseCustom(
        MutagenWriter writer,
        INpcRaceOverrideGetter item)
    {
        NpcRaceOverrideBinaryCreateTranslation.OverrideFlags flags = default;
        flags = flags.SetFlag(NpcRaceOverrideBinaryCreateTranslation.OverrideFlags.Size, item.Size != null);
        flags = flags.SetFlag(NpcRaceOverrideBinaryCreateTranslation.OverrideFlags.Unknown1, item.Unknown1 != null);
        flags = flags.SetFlag(NpcRaceOverrideBinaryCreateTranslation.OverrideFlags.Unknown2, item.Unknown2 != null);
        flags = flags.SetFlag(NpcRaceOverrideBinaryCreateTranslation.OverrideFlags.UnarmedWeapon, item.UnarmedWeapon != null);
        flags = flags.SetFlag(NpcRaceOverrideBinaryCreateTranslation.OverrideFlags.Flag, item.Flag != null);
        flags = flags.SetFlag(NpcRaceOverrideBinaryCreateTranslation.OverrideFlags.Unknown5, item.Unknown5 != null);
        flags = flags.SetFlag(NpcRaceOverrideBinaryCreateTranslation.OverrideFlags.General, item.General != null);
        flags = flags.SetFlag(NpcRaceOverrideBinaryCreateTranslation.OverrideFlags.Unknown7, item.Unknown7 != null);
        writer.Write((int)flags);
        item.Size?.WriteToBinary(writer);
        item.Unknown1?.WriteToBinary(writer);
        item.Unknown2?.WriteToBinary(writer);
        item.UnarmedWeapon?.WriteToBinary(writer);
        item.Flag?.WriteToBinary(writer);
        item.Unknown5?.WriteToBinary(writer);
        item.General?.WriteToBinary(writer);
        item.Unknown7?.WriteToBinary(writer);
    }
}