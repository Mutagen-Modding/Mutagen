using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Water
    {
        public enum Flag
        {
            CausesDamage = 0x01,
            Reflective = 0x02
        }

        static partial void FillBinary_OddExtraBytes_Custom(MutagenFrame frame, Water item, ErrorMaskBuilder errorMask)
        {
            if (frame.Remaining == 2)
            {
                frame.Position += 2;
            }
        }

        static partial void FillBinary_BloodCustomLogic_Custom(MutagenFrame frame, Water item, ErrorMaskBuilder errorMask)
        {
            if (frame.Remaining == 2)
            {
                frame.Position += 2;
            }
        }

        static partial void FillBinary_NothingCustomLogic_Custom(MutagenFrame frame, Water item, ErrorMaskBuilder errorMask)
        {
            if (frame.Remaining == 2)
            {
                frame.Position += 2;
            }
        }

        static partial void FillBinary_OilCustomLogic_Custom(MutagenFrame frame, Water item, ErrorMaskBuilder errorMask)
        {
            if (frame.Remaining == 2)
            {
                frame.Position += 2;
            }
        }
    }
}
