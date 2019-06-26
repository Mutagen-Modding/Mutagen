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
    public partial class Effect
    {
        public enum EffectType
        {
            Self = 0,
            Touch = 1,
            Target = 2
        }

        static partial void SpecialParse_EffectInitial(Effect item, MutagenFrame frame, ErrorMaskBuilder errorMask)
        {
            var recType = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var contentLen);
            if (contentLen != 4)
            {
                throw new ArgumentException($"Magic effect name must be length 4.  Was: {contentLen}");
            }
            var magicEffName = frame.ReadSpan(4);
            var efit = HeaderTranslation.GetNextSubRecordType(frame.Reader, out var efitLength);
            if (efitLength < 4)
            {
                throw new ArgumentException($"Magic effect ref length was less than 4.  Was: {efitLength}");
            }
            var magicEffName2 = frame.GetSpan(amount: 4, offset: Mutagen.Bethesda.Constants.SUBRECORD_LENGTH);
            if (!magicEffName.SequenceEqual(magicEffName2))
            {
                throw new ArgumentException($"Magic effect names did not match. {BinaryStringUtility.ToZString(magicEffName)} != {BinaryStringUtility.ToZString(magicEffName2)}");
            }
        }

        static partial void SpecialWrite_EffectInitial(
            IEffectInternalGetter item,
            MutagenWriter writer,
            ErrorMaskBuilder errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, Effect_Registration.EFID_HEADER))
            {
                Mutagen.Bethesda.Binary.RecordTypeBinaryTranslation.Instance.Write(
                    writer: writer,
                    item: item.MagicEffect_Property);
            }
        }
    }
}
