using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class CellNavigationMeshDataBinaryOverlay
        {
            public IFormLinkGetter<IWorldspaceGetter> UnusedWorldspaceParent => throw new NotImplementedException();

            public IFormLinkGetter<ICellGetter> Parent => throw new NotImplementedException();
        }
    }
}
