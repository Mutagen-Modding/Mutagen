using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;

namespace Mutagen.Binary
{
    public class FilePathBinaryTranslation : IBinaryTranslation<FilePath, Exception>
    {
        public static readonly FilePathBinaryTranslation Instance = new FilePathBinaryTranslation();

        public TryGet<FilePath> Parse(MutagenFrame frame, bool doMasks, out Exception errorMask)
        {
            var ret = StringBinaryTranslation.Instance.Parse(frame, doMasks, out errorMask);
            if (ret.Failed) return ret.BubbleFailure<FilePath>();
            try
            {
                return TryGet<FilePath>.Succeed(new FilePath(ret.Value));
            }
            catch (Exception ex)
            {
                if (doMasks)
                {
                    errorMask = ex;
                    return TryGet<FilePath>.Failure;
                }
                throw;
            }
        }

        void IBinaryTranslation<FilePath, Exception>.Write(
            MutagenWriter writer, 
            FilePath item, 
            ContentLength length,
            bool doMasks,
            out Exception maskObj)
        {
            ((IBinaryTranslation<string, Exception>)StringBinaryTranslation.Instance).Write(
                writer,
                item.RelativePath,
                length,
                doMasks,
                out maskObj);
        }

        public void Write(
            MutagenWriter writer,
            FilePath item,
            RecordType header,
            bool nullable,
            bool doMasks,
            out Exception errorMask)
        {
            StringBinaryTranslation.Instance.Write(
                writer,
                item.RelativePath,
                header,
                nullable,
                doMasks,
                out errorMask);
        }
    }
}
