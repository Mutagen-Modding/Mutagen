using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout3;

public partial class Package
{
    public APackageFlags? Flags { get; set; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IAPackageFlagsGetter? IPackageGetter.Flags => Flags;
}

partial class PackageBinaryCreateTranslation
{
    public static partial void FillBinaryIsRepeatableCustom(MutagenFrame frame, IPackageInternal item, PreviousParse lastParsed)
    {
        // PKPT 'Patrol Flags': u8 bool + 1 unused byte (always zero), truncatable to just the
        // bool byte. Read whichever form is present.
        var content = frame.ReadSubrecord().Content;
        item.IsRepeatable = content[0] > 0;
    }

    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, IPackageInternal item, PreviousParse lastParsed)
    {
        var c = frame.ReadSubrecord().Content;
        item.GeneralFlags = (Package.Flag)BinaryPrimitives.ReadUInt32LittleEndian(c);
        item.Unused = c[5];
        item.BehaviorFlags = (Package.BehaviorFlag)BinaryPrimitives.ReadUInt16LittleEndian(c.Slice(6));
        item.Unused2 = c.Length >= 12 ? BinaryPrimitives.ReadUInt16LittleEndian(c.Slice(10)) : default(ushort?);
        item.Flags = APackageFlags.Create(c);
    }
}

partial class PackageBinaryWriteTranslation
{
    public static partial void WriteBinaryIsRepeatableCustom(MutagenWriter writer, IPackageGetter item)
    {
        // Always export the full 2-byte form; the passthrough processor pads truncated 1-byte
        // PKPTs in the reference to match.
        if (item.IsRepeatable is not { } isRepeatable) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.PKPT))
        {
            writer.Write(isRepeatable, length: 2);
        }
    }

    public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IPackageGetter item)
    {
        var flags = item.Flags;
        if (flags == null) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.PKDT))
        {
            writer.Write((uint)item.GeneralFlags);
            writer.Write((byte)APackageFlags.TypeOf(flags));
            writer.Write(item.Unused);
            writer.Write((ushort)item.BehaviorFlags);
            if (item.Unused2 != null)
            {
                writer.Write(APackageFlags.GetFlags(flags));
                writer.Write(item.Unused2.Value);
            }
        }
    }
}

partial class PackageBinaryOverlay
{
    private int? _isRepeatableLocation;
    bool GetIsRepeatableIsSetCustom() => _isRepeatableLocation.HasValue;
    public partial Boolean? GetIsRepeatableCustom() => _isRepeatableLocation.HasValue
        ? HeaderTranslation.ExtractSubrecordMemory(_recordData, _isRepeatableLocation.Value, _package.MetaData.Constants)[0] > 0
        : default(bool?);
    partial void IsRepeatableCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _isRepeatableLocation = (ushort)(stream.Position - offset);
    }

    private ReadOnlyMemorySlice<byte> _pkdtContent;
    private IAPackageFlagsGetter? _flags;

    public Package.Flag GeneralFlags => (Package.Flag)BinaryPrimitives.ReadUInt32LittleEndian(_pkdtContent);
    public Byte Unused => _pkdtContent[5];
    public Package.BehaviorFlag BehaviorFlags => (Package.BehaviorFlag)BinaryPrimitives.ReadUInt16LittleEndian(_pkdtContent.Slice(6));
    public UInt16? Unused2 => _pkdtContent.Length >= 12 ? BinaryPrimitives.ReadUInt16LittleEndian(_pkdtContent.Slice(10)) : default(ushort?);
    public IAPackageFlagsGetter? Flags => _flags;

    partial void FlagsCustomParse(OverlayStream stream, int finalPos, int offset);
    partial void FlagsCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _pkdtContent = stream.ReadSubrecord().Content;
        _flags = APackageFlags.Create(_pkdtContent);
    }
}
