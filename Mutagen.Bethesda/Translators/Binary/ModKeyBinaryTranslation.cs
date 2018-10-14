using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Binary
{
    public class ModKeyBinaryTranslation : IBinaryTranslation<ModKey>
    {
        public readonly static ModKeyBinaryTranslation Instance = new ModKeyBinaryTranslation();

        public bool Parse(MutagenFrame frame, out ModKey item, ErrorMaskBuilder errorMask)
        {
            if (!StringBinaryTranslation.Instance.Parse(frame, out var str, errorMask))
            {
                item = default;
                return false;
            }
            
            return ModKey.TryFactory(str, out item);
        }

        public void Write(MutagenWriter writer, ModKey item, long length, ErrorMaskBuilder errorMask)
        {
            StringBinaryTranslation.Instance.Write(writer, item.ToString(), length, errorMask);
        }

        public void Write(
            MutagenWriter writer,
            ModKey item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
        {
            StringBinaryTranslation.Instance.Write(
                writer,
                item.ToString(),
                header,
                fieldIndex,
                nullable,
                errorMask,
                nullTerminate);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItem<ModKey> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask,
            bool nullTerminate = true)
        {
            if (!item.HasBeenSet) return;
            StringBinaryTranslation.Instance.Write(
                writer,
                item.Item.ToString(),
                header,
                fieldIndex,
                nullable,
                errorMask,
                nullTerminate);
        }
    }
}
