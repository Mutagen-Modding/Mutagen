using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Loqui;
using System.Diagnostics;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Race
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
            Playable = 1,
        }

        public enum FaceIndex
        {
            Head = 0,
            EarMale = 1,
            EarFemale = 2,
            Mouth = 3,
            TeethLower = 4,
            TeethUpper = 5,
            Tongue = 6,
            EyeLeft = 7,
            EyeRight = 8,
        }

        public enum BodyIndex
        {
            UpperBody = 0,
            LowerBody = 1,
            Hand = 2,
            Foot = 3,
            Tail = 4,
        }
    }

    namespace Internals
    {
        public partial class RaceBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
