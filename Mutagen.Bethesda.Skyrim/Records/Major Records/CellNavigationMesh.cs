using Mutagen.Bethesda.Binary;
using Noggog;
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
        IANavigationMeshDataGetter? IANavigationMeshGetter.Data => this.Data;
        #endregion
    }

    namespace Internals
    {
        public partial class CellNavigationMeshBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IANavigationMeshDataGetter? IANavigationMeshGetter.Data => this.Data;
            #endregion

            public ICellNavigationMeshDataGetter? Data
            {
                get
                {
                    if (!_dataSpan.HasValue) return null;
                    return CellNavigationMeshDataBinaryOverlay.CellNavigationMeshDataFactory(
                        new OverlayStream(_dataSpan.Value, _package),
                        _package);
                }
            }
        }
    }
}
