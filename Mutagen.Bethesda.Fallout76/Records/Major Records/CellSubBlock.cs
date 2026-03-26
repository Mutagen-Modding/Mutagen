using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Fallout76.Internals;

namespace Mutagen.Bethesda.Fallout76;

partial class CellSubBlockBinaryOverlay
{
    public IReadOnlyList<ICellGetter> Cells { get; private set; } = [];

    partial void CellsCustomParse(OverlayStream stream, int finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        this.Cells = BinaryOverlayList.FactoryByArray<ICellGetter>(
            mem: stream.RemainingMemory,
            package: _package,
            translationParams: null,
            getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, recConv),
            locs: ParseCellRecordLocations(
                stream: stream,
                package: _package));
    }

    internal static int[] ParseCellRecordLocations(OverlayStream stream, BinaryOverlayFactoryPackage package)
    {
        var ret = new System.Collections.Generic.List<int>();
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
}
