using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Loqui;
using Noggog.Notifying;
using Loqui.Internal;

namespace Mutagen.Bethesda.Binary
{
    public class FilePathBinaryTranslation
    {
        public static readonly FilePathBinaryTranslation Instance = new FilePathBinaryTranslation();

        public void ParseInto(
            MutagenFrame frame,
            IHasItem<FilePath> item)
        {
            if (Parse(
                frame,
                out FilePath subItem))
            {
                item.Item = subItem;
            }
            else
            {
                item.Unset();
            }
        }

        public bool Parse(MutagenFrame frame, out FilePath item)
        {
            if (!StringBinaryTranslation.Instance.Parse(frame, out var str))
            {
                item = default(FilePath);
                return false;
            }
            item = new FilePath(str);
            return true;
        }

        public void Write(
            MutagenWriter writer,
            FilePath item,
            RecordType header,
            bool nullable)
        {
            StringBinaryTranslation.Instance.Write(
                writer,
                item.RelativePath,
                header,
                nullable);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<FilePath> item,
            RecordType header,
            bool nullable)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                nullable);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<FilePath> item,
            RecordType header,
            bool nullable)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                header,
                nullable);
        }
    }
}
