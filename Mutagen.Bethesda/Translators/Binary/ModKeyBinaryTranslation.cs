using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Noggog;

namespace Mutagen.Bethesda.Binary
{
    public class ModKeyBinaryTranslation
    {
        public readonly static ModKeyBinaryTranslation Instance = new ModKeyBinaryTranslation();

        public bool Parse(MutagenFrame frame, out ModKey item)
        {
            if (!StringBinaryTranslation.Instance.Parse(frame, out var str))
            {
                item = default;
                return false;
            }

            return ModKey.TryFactory(str, out item);
        }

        public ModKey Parse(MutagenFrame frame)
        {
            if (!StringBinaryTranslation.Instance.Parse(frame, out var str))
            {
                return ModKey.Null;
            }

            if (!ModKey.TryFactory(str, out var item))
            {
                return ModKey.Null;
            }

            return item;
        }

        public void Write(MutagenWriter writer, ModKey item, long length)
        {
            StringBinaryTranslation.Instance.Write(writer, item.ToString(), length);
        }

        public void Write(
            MutagenWriter writer,
            ModKey item,
            RecordType header,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            StringBinaryTranslation.Instance.Write(
                writer,
                item.ToString(),
                header,
                binaryType);
        }
    }
}
