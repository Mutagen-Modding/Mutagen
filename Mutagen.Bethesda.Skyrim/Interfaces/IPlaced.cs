using Noggog;
using static Mutagen.Bethesda.Skyrim.SkyrimMajorRecord;

namespace Mutagen.Bethesda.Skyrim
{
    public partial interface IPlaced : IPlacedThing, IPlacedSimple
    {
        public enum DisableType
        {
            /// <summary>
            /// Safest way to disable a placed entity. This will flag the record as Initially Disabled, set the
            /// Enable Parent to be opposite of the Player (ensuring this remains permanently disabled) and also
            /// sets the Z-Offset to -30000 (Object is completely moved completely far away i.e. Not visible in
            /// Creation Kit)
            /// </summary>
            SafeDisable,

            /// <summary>
            /// Flags the record as Initially Disabled and set the Enable Parent opposite to the Player reference,
            /// to keep it permanently disabled. Z-Offset will be unchanged (e.g. You may want this if there is an
            /// attached package and needs player to be nearby to trigger it)
            /// </summary>
            DisableWithoutZOffset,

            /// <summary>
            /// Simply flags the record as Initially Disabled.
            /// </summary>
            JustInitiallyDisabled
        }

        public Placement? Placement { get; set; }

        public EnableParent? EnableParent { get; set; }

        public int MajorRecordFlagsRaw { get; set; }

        /// <summary>
        /// Allows to easily disable placed records. The <paramref name="disableType"/> can be specified optionally
        /// to further specify how the record should be disabled.
        /// </summary>
        /// <param name="disableType">How the record should be disabled</param>
        /// <returns>Returns true if the disable operation has succeeded, false otherwise.</returns>
        public bool Disable(DisableType disableType = DisableType.SafeDisable)
        {
            // Perform standard procedure used for undelete and disable placed type records.
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

            EnumExt.SetFlag(MajorRecordFlagsRaw, (int) SkyrimMajorRecordFlag.InitiallyDisabled, true);
            return true;
        }
    }

    public partial interface IPlacedGetter : IPlacedThingGetter, IPlacedSimpleGetter
    {
    }
}