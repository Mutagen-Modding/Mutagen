using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using System;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Binary
{
    public class PercentBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<double>
    {
        public PercentBinaryTranslationGeneration()
            : base(expectedLen: null, typeName: "Percent")
        {
            PreferDirectTranslation = false;
            this.CustomRead = ReadPercent;
            this.CustomWrite = WritePercent;
        }

        public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            if (typeGen.GetFieldData().Binary != BinaryGenerationType.Normal) return await base.ExpectedLength(objGen, typeGen);
            var percType = typeGen as Mutagen.Bethesda.Generation.PercentType;
            switch (percType.IntegerType)
            {
                case FloatIntegerType.UInt:
                    return 4;
                case FloatIntegerType.UShort:
                    return 2;
                case FloatIntegerType.Byte:
                    return 1;
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
            var percType = typeGen as Mutagen.Bethesda.Generation.PercentType;
            return $"{nameof(PercentBinaryTranslation)}.GetPercent({dataAccessor}, {nameof(FloatIntegerType)}.{percType.IntegerType})";
        }

        bool ReadPercent(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor reader, Accessor item)
        {
            var percType = typeGen as Mutagen.Bethesda.Generation.PercentType;
            using (var args = new ArgsWrapper(fg,
                $"{item} = {nameof(PercentBinaryTranslation)}.Parse"))
            {
                args.Add($"reader: {reader}");
                args.Add($"integerType: {nameof(FloatIntegerType)}.{percType.IntegerType}");
            }
            return true;
        }

        bool WritePercent(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writer, Accessor item)
        {
            var percType = typeGen as Mutagen.Bethesda.Generation.PercentType;
            var data = percType.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"{nameof(PercentBinaryTranslation)}.Write"))
            {
                args.Add($"writer: {writer}");
                args.Add($"item: {item}");
                args.Add($"integerType: {nameof(FloatIntegerType)}.{percType.IntegerType}");
                if (data.RecordType.HasValue
                    && data.HandleTrigger)
                {
                    args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                }
            }
            return true;
        }
    }
}
