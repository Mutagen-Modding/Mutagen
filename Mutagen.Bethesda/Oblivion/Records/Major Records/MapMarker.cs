using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class MapMarker
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }

        [Flags]
        public enum Flag
        {
            Visible = 0x01,
            CanTravelTo = 0x02
        }

        public enum Type
        {
            None = 0,
            Camp = 1,
            Cave = 2,
            City = 3,
            ElvenRuin = 4,
            FortRuin = 5,
            Mine = 6,
            Landmark = 7,
            Tavern = 8,
            Settlement = 9,
            DaedricShrine = 10,
            OblivionGate = 11,
            UnknownDoorIcon = 12,
        }
    }

    namespace Internals
    {
        public partial class MapMarkerBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
