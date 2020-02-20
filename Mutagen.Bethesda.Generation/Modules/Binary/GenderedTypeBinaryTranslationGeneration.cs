using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Generation
{
    public class GenderedTypeBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            throw new NotImplementedException();
        }

        public override void GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
        {
            GenderedType gender = typeGen as GenderedType;

            if (!this.Module.TryGetTypeGeneration(gender.SubTypeGeneration.GetType(), out var subTransl))
            {
                throw new ArgumentException("Unsupported type generator: " + gender.SubTypeGeneration);
            }

            if (typeGen.GetFieldData().HasTrigger)
            {
                fg.AppendLine($"{readerAccessor}.Position += {readerAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
            }

            using (var args = new ArgsWrapper(fg,
                $"{itemAccessor} = {this.Namespace}GenderedItemBinaryTranslation<{gender.SubTypeGeneration.TypeName(getter: false)}>.Parse"))
            {
                args.AddPassArg($"frame");
                if (gender.SubTypeGeneration is FormLinkType
                    || gender.SubTypeGeneration is LoquiType)
                {
                    args.AddPassArg("masterReferences");
                }
                if (gender.SubTypeGeneration is LoquiType loqui
                    && !loqui.CanStronglyType)
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(gender.SubTypeGeneration, getter: false)}.Parse<{loqui.TypeName(getter: false)}>");
                }
                else
                {
                    args.Add($"transl: {subTransl.GetTranslatorInstance(gender.SubTypeGeneration, getter: false)}.Parse");
                }
            }
        }

        public override void GenerateCopyInRet(FileGeneration fg, ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen, Accessor readerAccessor, AsyncMode asyncMode, Accessor retAccessor, Accessor outItemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
        {
            GenderedType gendered = typeGen as GenderedType;
            var gen = this.Module.GetTypeGeneration(gendered.SubTypeGeneration.GetType());
            if (typeGen.HasBeenSet)
            {
                fg.AppendLine($"if ({itemAccessor}.TryGet(out var {typeGen.Name}item))");
                itemAccessor = $"{typeGen.Name}item";
            }
            using (new BraceWrapper(fg, doIt: typeGen.HasBeenSet))
            {
                if (typeGen.HasBeenSet)
                {
                    fg.AppendLine($"using ({nameof(HeaderExport)}.{nameof(HeaderExport.ExportSubRecordHeader)}({writerAccessor}, recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(typeGen.GetFieldData().RecordType.Value)})))");
                }
                using (new BraceWrapper(fg, doIt: typeGen.HasBeenSet))
                {
                    using (new BraceWrapper(fg))
                    {
                        gen.GenerateWrite(fg, objGen, gendered.SubTypeGeneration, writerAccessor, $"{itemAccessor}.Male", errorMaskAccessor, translationAccessor);
                    }
                    using (new BraceWrapper(fg))
                    {
                        gen.GenerateWrite(fg, objGen, gendered.SubTypeGeneration, writerAccessor, $"{itemAccessor}.Female", errorMaskAccessor, translationAccessor);
                    }
                }
            }
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrapperFields(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor, int? passedLength, DataType data = null)
        {
            var gendered = typeGen as GenderedType;
            this.Module.TryGetTypeGeneration(gendered.SubTypeGeneration.GetType(), out var subBin);
            if (typeGen.HasBeenSet
                && !gendered.ItemHasBeenSet)
            {
                var subLen = subBin.ExpectedLength(objGen, gendered.SubTypeGeneration).Value;
                fg.AppendLine($"private int? _{typeGen.Name}Location;");
                fg.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true)}>{(typeGen.HasBeenSet ? "?" : null)} {typeGen.Name}");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("get");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"if (!_{typeGen.Name}Location.HasValue) return {typeGen.GetDefault()};");
                        fg.AppendLine($"var data = HeaderTranslation.ExtractSubrecordMemory(_data, _{typeGen.Name}Location.Value, _package.Meta);");
                        using (var args = new ArgsWrapper(fg,
                            $"return new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true)}>"))
                        {
                            args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, "data", "_package")}");
                            args.Add($"{subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, $"data.Slice({subLen})", "_package")}");
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
