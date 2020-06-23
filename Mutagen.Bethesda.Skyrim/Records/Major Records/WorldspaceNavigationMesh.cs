using Mutagen.Bethesda.Binary;
using Noggog;
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
        IANavigationMeshDataGetter? IANavigationMeshGetter.Data => this.Data;
        #endregion
    }

    namespace Internals
    {
        public partial class WorldspaceNavigationMeshBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IANavigationMeshDataGetter? IANavigationMeshGetter.Data => this.Data;
            #endregion

            public IWorldspaceNavigationMeshDataGetter? Data
            {
                get
                {
                    if (!_dataSpan.HasValue) return null;
                    return WorldspaceNavigationMeshDataBinaryOverlay.WorldspaceNavigationMeshDataFactory(
                        new OverlayStream(_dataSpan.Value, _package),
                        _package);
                }
            }
        }
    }
}
