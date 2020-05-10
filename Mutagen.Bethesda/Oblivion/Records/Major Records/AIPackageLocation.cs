using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class AIPackageLocation
    {
        public enum LocationType
        {
            NearReference = 0,
            InCell = 1,
            NearCurrentLocation = 2,
            NearEditorLocation = 3,
            ObjectID = 4,
            ObjectType = 5
        }
    }
}
