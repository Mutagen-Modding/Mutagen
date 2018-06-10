using Loqui;
using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class StringBinaryTranslation : IBinaryTranslation<string, Exception>
    {
        public readonly static StringBinaryTranslation Instance = new StringBinaryTranslation();

        public virtual TryGet<string> Parse(
            MutagenFrame frame,
            bool doMasks,
            out Exception errorMask)
        {
            return Parse(
                frame: frame,
                doMasks: doMasks,
                parseWhole: true,
                errorMask: out errorMask);
        }

        public virtual TryGet<string> Parse(
            MutagenFrame frame,
            bool parseWhole,
            bool doMasks,
            out Exception errorMask)
        {
            try
            {
                errorMask = null;
                string str;
                if (parseWhole)
                {
                    str = frame.Reader.ReadString(checked((int)frame.Remaining));
                    str = str.TrimEnd('\0');
                }
                else
                {
                    str = ReadStringUntil(frame.Reader, '\0', include: false);
                    frame.Reader.Position += 1;
                }
                return TryGet<string>.Succeed(str);
            }
            catch (Exception ex)
            {
                if (doMasks)
                {
                    errorMask = ex;
                    return TryGet<string>.Failure;
                }
                throw;
            }
        }

        public static string ReadStringUntil(
            IBinaryStream reader,
            char stopChar,
            bool include)
        {
            List<byte> chars = new List<byte>();
            while (!reader.Complete)
            {
                var nextChar = reader.ReadUInt8();
                if (nextChar == stopChar)
                {
                    if (include)
                    {
                        chars.Add(nextChar);
                    }
                    else
                    {
                        reader.Position -= 1;
                    }
                    break;
                }
                chars.Add(nextChar);
            }
            return BinaryUtility.BytesToString(chars.ToArray());
        }



        public TryGet<string> Parse<M>(
            MutagenFrame frame,
            int fieldIndex,
            bool parseWhole,
            Func<M> errorMask)
            where M : IErrorMask
        {
            var ret = this.Parse(
                frame: frame,
                parseWhole: parseWhole,
                doMasks: errorMask != null,
                errorMask: out var ex);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                ex);
            return ret;
        }

        public void Write(
            MutagenWriter writer,
            string item,
            bool doMasks,
            out Exception errorMask,
            bool nullTerminate = true)
        {
            try
            {
                writer.Write(item);
                if (nullTerminate)
                {
                    writer.Write((byte)0);
                }
                errorMask = null;
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }
        }

        public void Write<M>(
            MutagenWriter writer,
            string item,
            int fieldIndex,
            Func<M> errorMask,
            bool nullTerminate = true)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item,
                errorMask != null,
                out var subMask,
                nullTerminate: nullTerminate);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<string> item,
            int fieldIndex,
            Func<M> errorMask,
            bool nullTerminate = true)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                errorMask != null,
                out var subMask,
                nullTerminate: nullTerminate);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<string> item,
            int fieldIndex,
            Func<M> errorMask,
            bool nullTerminate = true)
            where M : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                errorMask != null,
                out var subMask,
                nullTerminate: nullTerminate);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write(
            MutagenWriter writer,
            string item,
            RecordType header,
            bool nullable,
            bool doMasks,
            out Exception errorMask,
            bool nullTerminate = true)
        {
            try
            {
                if (item == null)
                {
                    if (nullable)
                    {
                        errorMask = null;
                        return;
                    }
                    throw new ArgumentException("Non optional string was null.");
                }
                using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
                {
                    this.Write(
                        writer,
                        item,
                        doMasks,
                        out errorMask,
                        nullTerminate: nullTerminate);
                }
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = ex;
            }
        }

        public void Write<M>(
            MutagenWriter writer,
            string item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            Func<M> errorMask,
            bool nullTerminate = true)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item,
                header,
                nullable,
                errorMask != null,
                out var subMask,
                nullTerminate: nullTerminate);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<string> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            Func<M> errorMask,
            bool nullTerminate = true)
            where M : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask,
                nullTerminate: nullTerminate);
        }

        public void Write<M>(
            MutagenWriter writer,
            IHasItemGetter<string> item,
            RecordType header,
            int fieldIndex,
            bool nullable,
            Func<M> errorMask,
            bool nullTerminate = true)
            where M : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                header,
                fieldIndex,
                nullable,
                errorMask,
                nullTerminate: nullTerminate);
        }

        void IBinaryTranslation<string, Exception>.Write(MutagenWriter writer, string item, long length, bool doMasks, out Exception maskObj)
        {
            if (length != item.Length)
            {
                var ex = new ArgumentException($"Expected length was {item.Length}, but was passed {length}.");
                if (doMasks)
                {
                    maskObj = ex;
                    return;
                }
                throw ex;
            }
            Write(writer, item, doMasks, out maskObj);
        }
    }
}
