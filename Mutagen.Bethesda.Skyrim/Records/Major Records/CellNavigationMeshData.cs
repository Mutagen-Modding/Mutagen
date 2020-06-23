using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class CellNavigationMeshDataBinaryOverlay
        {
            public IFormLink<IWorldspaceGetter> UnusedWorldspaceParent =>
                new FormLink<IWorldspaceGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(8))));

            public IFormLink<ICellGetter> Parent =>
                new FormLink<ICellGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(12))));

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                CustomLogic();
            }
        }
    }
}
