using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

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

        static partial void SpecialParse_EffectInitial(Effect item, MutagenFrame frame, Func<Effect_ErrorMask> errorMask)
        {
            var recType = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var contentLen);
            if (contentLen != 4)
            {
                throw new ArgumentException($"Magic effect name must be length 4.  Was: {contentLen}");
            }
            var magicEffName = frame.Reader.ReadString(4);
            var pos = frame.Position;
            var efit = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var efitLength);
            if (efitLength < 4)
            {
                throw new ArgumentException($"Magic effect ref length was less than 4.  Was: {efitLength}");
            }
            var magicEffName2 = frame.Reader.ReadString(4);
            if (!magicEffName.Equals(magicEffName2))
            {
                throw new ArgumentException($"Magic effect names did not match. {magicEffName} != {magicEffName2}");
            }
            frame.Position = pos;
        }

        static partial void SpecialWrite_EffectInitial(
            IEffectGetter item,
            MutagenWriter writer,
            Func<Effect_ErrorMask> errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, Effect_Registration.EFID_HEADER))
            {
                writer.Write(item.MagicEffect_Property.EDID.Type);
            }
        }
    }
}
