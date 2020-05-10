using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MoveableStatic
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ObjectBounds? IObjectBoundedOptional.ObjectBounds
        {
            get => this.ObjectBounds;
            set => this.ObjectBounds = value ?? new ObjectBounds();
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter? IObjectBoundedOptionalGetter.ObjectBounds => this.ObjectBounds;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            MustUpdateAnims = 0x0000_0100,
            HiddenFromLocalMap = 0x0000_0200,
            HasDistantLOD = 0x0000_8000,
            RandomAnimStart = 0x0001_0000,
            HasCurrents = 0x0008_0000,
            Obstacle = 0x0200_0000,
            NavMeshGenerationFilter = 0x0400_0000,
            NavMeshGenerationBoundingBox = 0x0800_0000,
            NavMeshGenerationGround = 0x4000_0000,
        }

        [Flags]
        public enum Flag
        {
            OnLocalMap = 0x01
        }
    }

    namespace Internals
    {
        public partial class MoveableStaticBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
            #endregion
        }
    }
}
