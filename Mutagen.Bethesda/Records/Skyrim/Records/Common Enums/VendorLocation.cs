using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class VendorLocation
    {
        public enum LocationType
        {
            NearReference = 0,
            InCell = 1,
            NearPackageStart = 2,
            NearEditorLocation = 3,
            LinkedReference = 6,
            NearSelf = 12
        }
    }
}
