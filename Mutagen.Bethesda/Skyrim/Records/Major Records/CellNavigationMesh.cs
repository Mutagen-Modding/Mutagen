using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class CellNavigationMesh
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IANavigationMeshDataGetter IANavigationMeshGetter.Data => this.Data;
        #endregion
    }

    namespace Internals
    {
        public partial class CellNavigationMeshBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IANavigationMeshDataGetter IANavigationMeshGetter.Data => this.Data;
            #endregion

            public ICellNavigationMeshDataGetter Data => throw new NotImplementedException();
        }
    }
}
