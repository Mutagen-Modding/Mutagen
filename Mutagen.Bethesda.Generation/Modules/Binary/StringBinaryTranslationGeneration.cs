using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class StringBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<string>
    {
        public override bool AllowDirectParse(
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            bool squashedRepeatedList)
        {
            var str = typeGen as Mutagen.Bethesda.Generation.StringType;
            if (str.BinaryType != StringBinaryType.NullTerminate) return false;
            return !squashedRepeatedList;
        }

        public override bool AllowDirectWrite(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            var str = typeGen as Mutagen.Bethesda.Generation.StringType;
            if (str.BinaryType != StringBinaryType.NullTerminate) return false;
            return true;
        }

        public StringBinaryTranslationGeneration()
            : base(nullable: true, expectedLen: null)
        {
        }

        public override void GenerateWrite(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor writerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor,
            Accessor converterAccessor)
        {
            var stringType = typeGen as Mutagen.Bethesda.Generation.StringType;
            var data = typeGen.CustomData[Constants.DataKey] as MutagenFieldData;
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}StringBinaryTranslation.Instance.Write{(typeGen.HasBeenSet ? "Nullable" : null)}"))
            {
                args.Add($"writer: {writerAccessor}");
                args.Add($"item: {itemAccessor.DirectAccess}");
                if (this.DoErrorMasks)
                {
                    if (typeGen.HasIndex)
                    {
                        args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                    }
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                if (data.RecordType.HasValue)
                {
                    args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                }
                else if (data.Length.HasValue)
                {
                    args.Add($"length: {data.Length.Value}"); 
                }
                args.Add($"binaryType: {nameof(StringBinaryType)}.{stringType.BinaryType}");
                if (stringType.Translated.HasValue)
                {
                    args.Add($"source: {nameof(StringsSource)}.{stringType.Translated.Value}");
                }
            }
        }

        public override async Task GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            Accessor frameAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            var str = typeGen as StringType;
            var data = typeGen.GetFieldData();
            if (data.HasTrigger)
            {
                fg.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
            }

            List<string> extraArgs = new List<string>();
            extraArgs.Add($"frame: {frameAccessor}{(data.HasTrigger ? ".SpawnWithLength(contentLength)" : null)}");
            if (str.Translated.HasValue)
            {
                extraArgs.Add($"source: {nameof(StringsSource)}.{str.Translated.Value}");
            }
            extraArgs.Add($"stringBinaryType: {nameof(StringBinaryType)}.{str.BinaryType}");
            switch (str.BinaryType)
            {
                case StringBinaryType.NullTerminate:
                    if (!data.HasTrigger)
                    {
                        extraArgs.Add("parseWhole: false");
                    }
                    break;
                default:
                    break;
            }

            TranslationGeneration.WrapParseCall(
                new TranslationWrapParseArgs()
                {
                    FG = fg,
                    TypeGen = typeGen,
                    TranslatorLine = $"{this.Namespace}StringBinaryTranslation.Instance",
                    MaskAccessor = errorMaskAccessor,
                    ItemAccessor = itemAccessor,
                    TranslationMaskAccessor = null,
                    IndexAccessor = typeGen.HasIndex ? typeGen.IndexEnumInt : null,
                    ExtraArgs = extraArgs.ToArray(),
                    SkipErrorMask = !this.DoErrorMasks
                });
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen,
            TypeGeneration targetGen,
            TypeGeneration typeGen,
            Accessor nodeAccessor,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor,
            Accessor converterAccessor,
            bool inline)
        {
            if (inline) throw new NotImplementedException();
            if (asyncMode != AsyncMode.Off) throw new NotImplementedException();
            var data = typeGen.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"{retAccessor}{this.Namespace}StringBinaryTranslation.Instance.Parse"))
            {
                args.Add(nodeAccessor.DirectAccess);
                if (this.DoErrorMasks)
                {
                    args.Add($"errorMask: {errorMaskAccessor}");
                }
                args.Add($"item: out {outItemAccessor.DirectAccess}");
                args.Add($"parseWhole: {(data.HasTrigger ? "true" : "false")}");

                if (data.Length.HasValue)
                {
                    args.Add($"length: {data.Length.Value}");
                }
            }
        }

        public override bool CanInline(ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen)
        {
            return false;
        }

        public override async Task GenerateWrapperFields(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor, 
            int? currentPosition, 
            string passedLengthAccessor, 
            DataType dataType = null)
        {
            StringType str = typeGen as StringType;
            var data = str.GetFieldData();
            if (data.HasTrigger)
            {
                await base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, passedLengthAccessor, dataType);
                return;
            }
            switch (str.BinaryType)
            {
                case StringBinaryType.NullTerminate:
                    fg.AppendLine($"public {typeGen.TypeName(getter: true)}{str.NullChar} {typeGen.Name} {{ get; private set; }} = string.Empty;");
                    break;
                default:
                    await base.GenerateWrapperFields(fg, objGen, typeGen, dataAccessor, currentPosition, passedLengthAccessor, dataType);
                    return;
            }

        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            StringType str = typeGen as StringType;
            if (str.Translated.HasValue)
            {
                return $"StringBinaryTranslation.Instance.Parse({dataAccessor}, {nameof(StringsSource)}.{str.Translated.Value}, _package.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.StringsLookup)})";
            }
            else
            {
                switch (str.BinaryType)
                {
                    case StringBinaryType.Plain:
                    case StringBinaryType.NullTerminate:
                        var data = typeGen.GetFieldData();
                        var gendered = data.Parent as GenderedType;
                        if (data.HasTrigger
                            || (gendered?.MaleMarker.HasValue ?? false))
                        {
                            return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ProcessWholeToZString)}({dataAccessor})";
                        }
                        else
                        {
                            return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParseUnknownLengthString)}({dataAccessor})";
                        }
                    case StringBinaryType.PrependLength:
                        return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParsePrependedString)}({dataAccessor}, lengthLength: 4)";
                    case StringBinaryType.PrependLengthUShort:
                        return $"{nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParsePrependedString)}({dataAccessor}, lengthLength: 2)";
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public override async Task GenerateWrapperUnknownLengthParse(
            FileGeneration fg, 
            ObjectGeneration objGen,
            TypeGeneration typeGen,
            int? passedLength,
            string passedLengthAccessor)
        {
            StringType str = typeGen as StringType;
            switch (str.BinaryType)
            {
                case StringBinaryType.PrependLength:
                    fg.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}BinaryPrimitives.ReadInt32LittleEndian(ret._data{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}) + 4;");
                    break;
                case StringBinaryType.PrependLengthUShort:
                    fg.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}BinaryPrimitives.ReadUInt16LittleEndian(ret._data{(passedLengthAccessor == null ? null : $".Slice({passedLengthAccessor})")}) + 2;");
                    break;
                case StringBinaryType.NullTerminate:
                    fg.AppendLine($"ret.{typeGen.Name} = {nameof(BinaryStringUtility)}.{nameof(BinaryStringUtility.ParseUnknownLengthString)}(ret._data.Slice({passedLengthAccessor}));");
                    fg.AppendLine($"ret.{typeGen.Name}EndingPos = {(passedLengthAccessor == null ? null : $"{passedLengthAccessor} + ")}{(str.Translated == null ? $"ret.{typeGen.Name}.Length + 1" : "5")};");
                    break;
                default:
                    if (typeGen.GetFieldData().Binary == BinaryGenerationType.Custom) return;
                    throw new NotImplementedException();
            }
        }
    }
}
