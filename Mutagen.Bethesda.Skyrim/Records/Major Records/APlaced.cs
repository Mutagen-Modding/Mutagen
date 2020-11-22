using System;
using Noggog;
using static Mutagen.Bethesda.Skyrim.IPlaced;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class APlaced : IPlaced
    {
        public Placement? Placement { get; set; }

        public EnableParent? EnableParent { get; set; }

        /// <summary>
        /// Disables the placed object using standard undelete and disable procedure. This will flag the record <br/>
        /// as Initially Disabled, set the Enable Parent to be opposite of the Player and set the Z-Offset to
        /// -30000.
        /// </summary>
        /// <returns>True if the disable was successful.</returns>
        public override bool Disable()
        {
            return Disable(DisableType.SafeDisable);
        }

        public bool Disable(DisableType disableType)
        {
            if (this.IsDeleted) return false;

            if (Placement != null && disableType == DisableType.SafeDisable)
            {
                Placement.Position = new P3Float(Placement.Position.X, Placement.Position.Y, -30000);
            }

            if (EnableParent != null && disableType != DisableType.JustInitiallyDisabled)
            {
                EnableParent.Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent;
                EnableParent.Reference = new FormLink<ILinkedReferenceGetter>(Constants.Player);
            }

            MajorRecordFlagsRaw = EnumExt.SetFlag(MajorRecordFlagsRaw, (int) SkyrimMajorRecordFlag.InitiallyDisabled, true);
            return true;
        }
    }
}