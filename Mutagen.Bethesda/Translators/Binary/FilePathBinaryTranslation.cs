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
    public class FilePathBinaryTranslation : IBinaryTranslation<FilePath>
    {
        public static readonly FilePathBinaryTranslation Instance = new FilePathBinaryTranslation();

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<FilePath> item,
            ErrorMaskBuilder errorMask)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame,
                    out FilePath subItem,
                    errorMask))
                {
                    item.Item = subItem;
                }
                else
                {
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public bool Parse(MutagenFrame frame, out FilePath item, ErrorMaskBuilder errorMask)
        {
            if (!StringBinaryTranslation.Instance.Parse(frame, out var str, errorMask))
            {
                item = default(FilePath);
                return false;
            }
            try
            {
                item = new FilePath(str);
                return true;
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
                item = default(FilePath);
                return false;
            }
        }

        void IBinaryTranslation<FilePath>.Write(
            MutagenWriter writer,
            FilePath item,
            long length,
            ErrorMaskBuilder errorMask)
        {
            ((IBinaryTranslation<string>)StringBinaryTranslation.Instance).Write(
                writer,
                item.RelativePath,
                length,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            FilePath item,
            RecordType header,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            StringBinaryTranslation.Instance.Write(
                writer,
                item.RelativePath,
                header,
                nullable,
                errorMask);
        }

        public void Write(
            MutagenWriter writer,
            FilePath item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                this.Write(
                    writer,
                    item,
                    header,
                    nullable,
                    errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<FilePath> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<FilePath> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            ErrorMaskBuilder errorMask)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask);
        }
    }
}
