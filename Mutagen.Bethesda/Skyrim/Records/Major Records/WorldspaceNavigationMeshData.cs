using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class WorldspaceNavigationMeshDataBinaryOverlay
        {
            public IFormLink<IWorldspaceGetter> Parent => throw new NotImplementedException();

            public P2Int16 Coordinates => throw new NotImplementedException();

            partial void CustomCtor(BinaryMemoryReadStream stream, int finalPos, int offset)
            {
                CustomLogic();
            }
        }
    }
}
