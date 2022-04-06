using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

partial class WorldspaceObjectBoundsBinaryCreateTranslation
{
    public static partial void FillBinaryMaxCustom(MutagenFrame frame, IWorldspaceObjectBounds item)
    {
        var subHeader = frame.ReadSubrecord();
        if (subHeader.ContentLength != 8)
        {
            throw new ArgumentException("Unexpected length");
        }
        item.Max = new P2Float(
            frame.ReadFloat() / 4096f,
            frame.ReadFloat() / 4096f);
    }

    public static partial void FillBinaryMinCustom(MutagenFrame frame, IWorldspaceObjectBounds item)
    {
        var subHeader = frame.ReadSubrecord();
        if (subHeader.ContentLength != 8)
        {
            throw new ArgumentException("Unexpected length");
        }
        item.Min = new P2Float(
            frame.ReadFloat() / 4096f,
            frame.ReadFloat() / 4096f);
    }
}

partial class WorldspaceObjectBoundsBinaryWriteTranslation
{
    public static partial void WriteBinaryMaxCustom(MutagenWriter writer, IWorldspaceObjectBoundsGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.NAM9))
        {
            var max = item.Max;
            writer.Write(max.X * 4096f);
            writer.Write(max.Y * 4096f);
        }
    }

    public static partial void WriteBinaryMinCustom(MutagenWriter writer, IWorldspaceObjectBoundsGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.NAM0))
        {
            var min = item.Min;
            writer.Write(min.X * 4096f);
            writer.Write(min.Y * 4096f);
        }
    }
}

partial class WorldspaceObjectBoundsBinaryOverlay
{
    private int? _minLoc;
    public partial P2Float GetMinCustom() => _minLoc.HasValue 
        ? new P2Float(
            _data.Slice(_minLoc.Value + 6).Float() / 4096f,
            _data.Slice(_minLoc.Value + 10).Float() / 4096f)
        : default;

    private int? _maxLoc;
    public partial P2Float GetMaxCustom() => _maxLoc.HasValue 
        ? new P2Float(
            _data.Slice(_maxLoc.Value + 6).Float() / 4096f,
            _data.Slice(_maxLoc.Value + 10).Float() / 4096f)
        : default;

    partial void MinCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        this._minLoc = stream.Position - offset;
    }

    partial void MaxCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        this._maxLoc = stream.Position - offset;
    }
}