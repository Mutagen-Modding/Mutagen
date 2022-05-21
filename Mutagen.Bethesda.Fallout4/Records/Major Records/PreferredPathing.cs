using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Overlay;

namespace Mutagen.Bethesda.Fallout4;

partial class PreferredPathingBinaryOverlay
{
    public IReadOnlyList<INavmeshSetGetter> NavmeshSets { get; private set; } = Array.Empty<NavmeshSetBinaryOverlay>();

    partial void CustomNavmeshSetsEndPos()
    {
        var count = BinaryPrimitives.ReadInt32LittleEndian(this._data);
        int[] locs = new int[count];
        var span = this._data.Slice(4);
        int loc = 0;
        for (int i = 0; i < count; i++)
        {
            var subCount = BinaryPrimitives.ReadInt32LittleEndian(span);
            var len = 4 + subCount * 4;
            span = span.Slice(len);
            locs[i] = loc;
            loc += len;
        }
        this.NavmeshSets = BinaryOverlayList.FactoryByArray<INavmeshSetGetter>(
            this._data.Slice(4),
            this._package,
            (s, p) => NavmeshSetBinaryOverlay.NavmeshSetFactory(s, p),
            locs);
        this.NavmeshSetsEndingPos = 4 + loc;
    }
}
