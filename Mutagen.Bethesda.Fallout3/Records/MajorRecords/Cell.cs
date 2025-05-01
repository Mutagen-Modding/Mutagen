using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class Cell
{
    [Flags]
    public enum Flag
    {
        IsInteriorCell = 0x0001,
        HasWater = 0x0002,
        CanTravelFromHere= 0x0004,
        NoLODWater = 0x0008,
        PublicPlace = 0x0020,
        HandChanged = 0x0040,
        BehaveLikeExteriod = 0x0080,
    }
}

partial class CellBinaryCreateTranslation
{
    public static partial void CustomBinaryEndImport(MutagenFrame frame, ICellInternal obj)
    {
        try
        {
            CustomBinaryEnd(frame, obj);
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }

    private static void CustomBinaryEnd(MutagenFrame frame, ICellInternal obj)
    {
        throw new NotImplementedException();
    }
}

partial class CellBinaryWriteTranslation
{
    public static partial void CustomBinaryEndExport(MutagenWriter writer, ICellGetter obj)
    {
        throw new NotImplementedException();
    }
}

partial class CellBinaryOverlay
{
    static readonly RecordTriggerSpecs TypicalPlacedTypes = new RecordTriggerSpecs(
        RecordCollection.Factory(
            RecordTypes.ACHR,
            RecordTypes.ACRE,
            RecordTypes.REFR));

    private ReadOnlyMemorySlice<byte>? _grupData;

    public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    private int? _persistentLocation;
    public int PersistentTimestamp => _persistentLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_persistentLocation.Value)).LastModifiedData) : 0;
    public IReadOnlyList<IPlacedGetter> Persistent { get; private set; } = Array.Empty<IPlacedGetter>();

    private int? _temporaryLocation;
    public int TemporaryTimestamp => _temporaryLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_temporaryLocation.Value)).LastModifiedData) : 0;
    public IReadOnlyList<IPlacedGetter> Temporary { get; private set; } = Array.Empty<IPlacedGetter>();

    private int? _visibleWhenDistantLocation;
    public int VisibleWhenDistantTimestamp => _visibleWhenDistantLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_visibleWhenDistantLocation.Value)).LastModifiedData) : 0;
    public IReadOnlyList<IPlacedGetter> VisibleWhenDistant { get; private set; } = Array.Empty<IPlacedGetter>();

    public static int[] ParseRecordLocations(OverlayStream stream, BinaryOverlayFactoryPackage package)
    {
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        while (!stream.Complete)
        {
            var cellMeta = stream.GetMajorRecordHeader();
            if (cellMeta.RecordType != RecordTypes.CELL) break;
            ret.Add(stream.Position - startingPos);
            stream.Position += (int)cellMeta.TotalLength;
            if (stream.Complete) break;
            while (stream.TryGetGroupHeader(out var groupMeta)
                   && groupMeta.GroupType == (int)GroupTypeEnum.CellChildren)
            {
                stream.Position += (int)groupMeta.TotalLength;
            }
        }
        return ret.ToArray();
    }

    partial void CustomEnd(OverlayStream stream, int finalPos, int _)
    {
        throw new NotImplementedException();
    }
}
