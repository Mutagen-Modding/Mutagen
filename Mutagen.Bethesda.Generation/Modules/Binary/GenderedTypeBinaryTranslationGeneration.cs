using Loqui;
using Loqui.Generation;
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
            using (var args = new ArgsWrapper(fg,
                $"{this.Namespace}GenderedItemBinaryTranslation<{gender.SubTypeGeneration.TypeName(getter: false)}>.Instance.Parse"))
            {
                args.AddPassArg($"frame");
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
                fg.AppendLine($"if ({itemAccessor} != null)");
            }
            using (new BraceWrapper(fg, doIt: typeGen.HasBeenSet))
            {
                gen.GenerateWrite(fg, objGen, gendered.SubTypeGeneration, writerAccessor, $"{itemAccessor}.Male", errorMaskAccessor, translationAccessor);
                gen.GenerateWrite(fg, objGen, gendered.SubTypeGeneration, writerAccessor, $"{itemAccessor}.Female", errorMaskAccessor, translationAccessor);
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
                fg.AppendLine($"public IGenderedItemGetter<{gendered.SubTypeGeneration.TypeName(getter: true)}>{(typeGen.HasBeenSet ? "?" : null)} {typeGen.Name} => _{typeGen.Name}Location.HasValue ? new GenderedItem<{gendered.SubTypeGeneration.TypeName(getter: true)}>({subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, dataAccessor, "_package")}, {subBin.GenerateForTypicalWrapper(objGen, gendered.SubTypeGeneration, $"{dataAccessor}.Slice({subLen})", "_package")}) : {typeGen.GetDefault()};");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
