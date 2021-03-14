using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mutagen.Bethesda.Skyrim.IPlaced;
using static Mutagen.Bethesda.Skyrim.SkyrimMajorRecord;

namespace Mutagen.Bethesda.Skyrim
{
    public static class IPlacedExt
    {
        /// <summary>
        /// Disables the placed object using standard undelete and disable procedure. This will flag the record <br/>
        /// as Initially Disabled, set the Enable Parent to be opposite of the Player and set the Z-Offset to
        /// -30000.
        /// </summary>
        /// <returns>True if the disable was successful.</returns>
        public static bool Disable(this IPlaced placed)
        {
            return placed.Disable(DisableType.SafeDisable);
        }

        /// <summary>
        /// Allows to easily disable placed records. Specify <paramref name="disableType"/> to further designate
        /// how the record should be disabled.
        /// </summary>
        /// <param name="placed">IPlaced object to disable</param>
        /// <param name="disableType">How the record should be disabled</param>
        /// <returns>Returns true if the disable operation has succeeded, false otherwise.</returns>
        public static bool Disable(this IPlaced placed, DisableType disableType)
        {
            if (placed.IsDeleted) return false;

            if (placed.Placement != null && disableType == DisableType.SafeDisable)
            {
                placed.Placement.Position = new P3Float(placed.Placement.Position.X, placed.Placement.Position.Y, -30000);
            }

            if (placed.EnableParent != null && disableType != DisableType.JustInitiallyDisabled)
            {
                placed.EnableParent.Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent;
                placed.EnableParent.Reference.FormKey = Constants.Player;
            }

            placed.MajorRecordFlagsRaw = EnumExt.SetFlag(placed.MajorRecordFlagsRaw, (int)SkyrimMajorRecordFlag.InitiallyDisabled, true);
            return true;
        }
    }
}
