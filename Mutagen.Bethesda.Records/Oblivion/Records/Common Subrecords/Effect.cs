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
    }

    namespace Internals
    {
        public partial class EffectBinaryCreateTranslation
        {
            static partial void FillBinaryEffectInitialCustom(MutagenFrame frame, IEffect item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var subMeta = frame.MetaData.ReadSubRecord(frame);
                if (subMeta.RecordLength != Mutagen.Bethesda.Constants.HEADER_LENGTH)
                {
                    throw new ArgumentException($"Magic effect name must be length 4.  Was: {subMeta.RecordLength}");
                }
                var magicEffName = frame.ReadSpan(4);

                var efitMeta = frame.MetaData.GetSubRecord(frame);
                if (efitMeta.RecordType != Effect_Registration.EFIT_HEADER)
                {
                    throw new ArgumentException("Expected EFIT header.");
                }
                if (efitMeta.RecordLength < Mutagen.Bethesda.Constants.HEADER_LENGTH)
                {
                    throw new ArgumentException($"Magic effect ref length was less than 4.  Was: {efitMeta.RecordLength}");
                }
                var magicEffName2 = frame.GetSpan(amount: Mutagen.Bethesda.Constants.HEADER_LENGTH, offset: efitMeta.HeaderLength);
                if (!magicEffName.SequenceEqual(magicEffName2))
                {
                    throw new ArgumentException($"Magic effect names did not match. {BinaryStringUtility.ToZString(magicEffName)} != {BinaryStringUtility.ToZString(magicEffName2)}");
                }
            }
        }

        public partial class EffectBinaryWriteTranslation
        {
            static partial void WriteBinaryEffectInitialCustom(MutagenWriter writer, IEffectGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
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
}
