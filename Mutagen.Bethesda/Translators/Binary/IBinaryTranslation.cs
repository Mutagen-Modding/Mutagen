using Loqui.Internal;
using Noggog;
using System;

namespace Mutagen.Bethesda.Binary
{
    public delegate bool BinarySubParseDelegate<T>(
        MutagenFrame reader,
        out T item,
        ErrorMaskBuilder errorMask);
    public delegate bool BinaryMasterParseDelegate<T>(
        MutagenFrame reader,
        out T item,
        MasterReferences masterReferences,
        ErrorMaskBuilder errorMask);
    public delegate bool BinarySubParseRecordDelegate<T>(
        MutagenFrame reader,
        RecordType header, 
        out T item, 
        ErrorMaskBuilder errorMask);
    public delegate void BinarySubWriteDelegate<in T>(
        MutagenWriter writer,
        T item,
        ErrorMaskBuilder errorMask);
    public delegate void BinaryMasterWriteDelegate<in T>(
        MutagenWriter writer,
        T item,
        MasterReferences masterReferences,
        ErrorMaskBuilder errorMask);

    public interface IBinaryTranslation<T>
    {
        void Write(
            MutagenWriter writer, 
            T item, 
            long length, 
            ErrorMaskBuilder errorMask);
        bool Parse(
            MutagenFrame frame,
            out T item,
            ErrorMaskBuilder errorMask);
    }
}
