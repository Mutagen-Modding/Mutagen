using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Skyrim;

partial class CellNavigationMeshDataBinaryOverlay
{
    public IFormLinkGetter<IWorldspaceGetter> UnusedWorldspaceParent =>
        new FormLink<IWorldspaceGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(8))));

    public IFormLinkGetter<ICellGetter> Parent =>
        new FormLink<ICellGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(12))));

    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        CustomLogic();
    }
}