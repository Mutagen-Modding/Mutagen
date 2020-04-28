using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Tree
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFormLinkNullableGetter<IHarvestTargetGetter> IHarvestableGetter.Ingredient => this.Ingredient;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IFormLinkNullableGetter<ISoundDescriptorGetter> IHarvestableGetter.HarvestSound => this.HarvestSound;

        [Flags]
        public enum MajorFlag
        {
            HasDistantLOD = 0x0000_8000
        }
    }

    namespace Internals
    {
        public partial class TreeBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
