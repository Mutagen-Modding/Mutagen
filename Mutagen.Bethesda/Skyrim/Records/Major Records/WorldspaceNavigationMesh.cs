using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class WorldspaceNavigationMesh
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IANavigationMeshDataGetter IANavigationMeshGetter.Data => this.Data;
        #endregion
    }

    namespace Internals
    {
        public partial class WorldspaceNavigationMeshBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IANavigationMeshDataGetter IANavigationMeshGetter.Data => this.Data;
            #endregion

            public IWorldspaceNavigationMeshDataGetter Data => throw new NotImplementedException();
        }
    }
}
