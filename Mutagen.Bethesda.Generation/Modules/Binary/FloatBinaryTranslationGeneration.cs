using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
    public class FloatBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<float>
    {
        public FloatBinaryTranslationGeneration()
            : base(expectedLen: 4, typeName: "Float")
        {
            PreferDirectTranslation = false;
            this.CustomRead = ReadFloat;
            this.CustomWrite = WriteFloat;
        }

        public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            if (typeGen.GetFieldData().Binary != BinaryGenerationType.Normal) return await base.ExpectedLength(objGen, typeGen);
            var floatType = typeGen as Mutagen.Bethesda.Generation.FloatType;
            switch (floatType.StorageType)
            {
                case Binary.FloatBinaryType.Normal:
                    return 4;
                case Binary.FloatBinaryType.Integer:
                    switch (floatType.IntegerType)
                    {
                        case FloatIntegerType.UInt:
                            return 4;
                        case FloatIntegerType.UShort:
                            return 2;
                        default:
                            throw new NotImplementedException();
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            var floatType = typeGen as Mutagen.Bethesda.Generation.FloatType;
            switch (floatType.StorageType)
            {
                case Binary.FloatBinaryType.Normal:
                    return $"SpanExt.GetFloat({dataAccessor})";
                case Binary.FloatBinaryType.Integer:
                    return $"{nameof(FloatBinaryTranslation)}.GetFloat({dataAccessor}, {nameof(FloatIntegerType)}.{floatType.IntegerType}, {floatType.IntegerDivisor})";
                default:
                    throw new NotImplementedException();
            }
        }

        bool ReadFloat(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor reader, Accessor item)
        {
            var floatType = typeGen as Mutagen.Bethesda.Generation.FloatType;
            switch (floatType.StorageType)
            {
                case Binary.FloatBinaryType.Normal:
                    return false;
                case Binary.FloatBinaryType.Integer:
                    using (var args = new ArgsWrapper(fg,
                        $"{item} = FloatBinaryTranslation.Parse"))
                    {
                        args.Add($"frame: {reader}");
                        args.Add($"integerType: {nameof(FloatIntegerType)}.{floatType.IntegerType}");
                        args.Add($"divisor: {floatType.IntegerDivisor}");
                    }
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }

        bool WriteFloat(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writer, Accessor item)
        {
            var floatType = typeGen as Mutagen.Bethesda.Generation.FloatType;
            var data = floatType.GetFieldData();
            switch (floatType.StorageType)
            {
                case Binary.FloatBinaryType.Normal:
                    return false;
                case Binary.FloatBinaryType.Integer:
                    using (var args = new ArgsWrapper(fg,
                        $"FloatBinaryTranslation.Write"))
                    {
                        args.Add($"writer: {writer}");
                        args.Add($"item: {item}");
                        args.Add($"integerType: {nameof(FloatIntegerType)}.{floatType.IntegerType}");
                        args.Add($"divisor: {floatType.IntegerDivisor}");
                        if (data.RecordType.HasValue
                            && data.HandleTrigger)
                        {
                            args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                        }
                    }
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
