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

        public void Write(MutagenWriter writer, ModKey item, long length)
        {
            StringBinaryTranslation.Instance.Write(writer, item.ToString(), length);
        }

        public void Write(
            MutagenWriter writer,
            ModKey item,
            RecordType header,
            bool nullable,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            StringBinaryTranslation.Instance.Write(
                writer,
                item.ToString(),
                header,
                nullable,
                binaryType);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItem<ModKey> item,
            RecordType header,
            bool nullable,
            StringBinaryType binaryType = StringBinaryType.NullTerminate)
        {
            if (!item.HasBeenSet) return;
            StringBinaryTranslation.Instance.Write(
                writer,
                item.Item.ToString(),
                header,
                nullable,
                binaryType);
        }
    }
}
