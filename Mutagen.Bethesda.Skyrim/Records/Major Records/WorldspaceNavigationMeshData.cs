using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class WorldspaceNavigationMeshDataBinaryOverlay
        {
            public IFormLinkGetter<IWorldspaceGetter> Parent =>
                new FormLink<IWorldspaceGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(8))));

            public P2Int16 Coordinates => new P2Int16(
                BinaryPrimitives.ReadInt16LittleEndian(_data.Slice(12)),
                BinaryPrimitives.ReadInt16LittleEndian(_data.Slice(14)));

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                CustomLogic();
            }
        }
    }
}
