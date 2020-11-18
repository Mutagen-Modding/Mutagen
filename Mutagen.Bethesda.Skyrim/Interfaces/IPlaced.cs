using Noggog;

namespace Mutagen.Bethesda.Skyrim
{
    public partial interface IPlaced
    {
        public Placement? Placement { get; set; }
        
        public EnableParent? EnableParent { get; set; }
        
        public int MajorRecordFlagsRaw { get; set; }

        public bool Disable()
        {
            // Perform standard procedure used by xEdit for undelete and disable records.
            if (this.IsDeleted) return false;

            if (Placement != null) Placement.Position = new P3Float(Placement.Position.X, Placement.Position.Y, -30000);
            if (EnableParent != null)
            {
                EnableParent.Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent;
                EnableParent.Reference = new FormLink<ILinkedReferenceGetter>(Constants.Player);
            }

            MajorRecordFlagsRaw = (int) SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled;
            return true;
        }
    }
}