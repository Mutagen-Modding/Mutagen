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
            if (floatType.IntegerType.HasValue)
            {
                return floatType.IntegerType switch
                {
                    FloatIntegerType.UInt => 4,
                    FloatIntegerType.UShort => 2,
                    FloatIntegerType.Byte => 1,
                    _ => throw new NotImplementedException(),
                };
            }
            else
            {
                return 4;
            }
        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            var floatType = typeGen as Mutagen.Bethesda.Generation.FloatType;
            if (floatType.IntegerType.HasValue)
            {
                return $"{nameof(FloatBinaryTranslation)}.GetFloat({dataAccessor}, {nameof(FloatIntegerType)}.{floatType.IntegerType}, {floatType.Multiplier})";
            }
            else
            {
                return $"{dataAccessor}.Float()";
            }
        }

        bool ReadFloat(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor reader, Accessor item)
        {
            var floatType = typeGen as Mutagen.Bethesda.Generation.FloatType;
            if (floatType.IntegerType.HasValue)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{item} = FloatBinaryTranslation.Parse"))
                {
                    args.Add($"frame: {reader}");
                    args.Add($"integerType: {nameof(FloatIntegerType)}.{floatType.IntegerType}");
                    args.Add($"multiplier: {floatType.Multiplier}");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        bool WriteFloat(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writer, Accessor item)
        {
            var floatType = typeGen as Mutagen.Bethesda.Generation.FloatType;
            var data = floatType.GetFieldData();
            if (floatType.IntegerType.HasValue)
            {
                using (var args = new ArgsWrapper(fg,
                    $"FloatBinaryTranslation.Write"))
                {
                    args.Add($"writer: {writer}");
                    args.Add($"item: {item}");
                    args.Add($"integerType: {nameof(FloatIntegerType)}.{floatType.IntegerType}");
                    args.Add($"multiplier: {floatType.Multiplier}");
                    if (data.RecordType.HasValue
                        && data.HandleTrigger)
                    {
                        args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
