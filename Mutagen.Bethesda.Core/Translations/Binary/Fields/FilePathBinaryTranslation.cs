using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Loqui;

namespace Mutagen.Bethesda.Binary
{
    public class FilePathBinaryTranslation
    {
        public static readonly FilePathBinaryTranslation Instance = new FilePathBinaryTranslation();

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
            RecordType header)
        {
            StringBinaryTranslation.Instance.Write(
                writer,
                item.RelativePath,
                header);
        }
    }
}
