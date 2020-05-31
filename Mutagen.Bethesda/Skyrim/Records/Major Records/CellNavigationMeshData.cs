using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class CellNavigationMeshDataBinaryOverlay
        {
            public IFormLink<IWorldspaceGetter> UnusedWorldspaceParent => throw new NotImplementedException();

            public IFormLink<ICellGetter> Parent => throw new NotImplementedException();
        }
    }
}
