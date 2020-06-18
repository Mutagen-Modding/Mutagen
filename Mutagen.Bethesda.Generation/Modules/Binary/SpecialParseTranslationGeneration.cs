using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class SpecialParseTranslationGeneration : BinaryTranslationGeneration
    {
        public override bool ShouldGenerateCopyIn(TypeGeneration typeGen) => true;
        public override bool ShouldGenerateWrite(TypeGeneration typeGen) => true;

        public override async Task GenerateCopyIn(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor readerAccessor,
            Accessor itemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            var data = typeGen.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"SpecialParse_{typeGen.Name}"))
            {
                args.AddPassArg("item");
                args.AddPassArg("frame");
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            TypeGeneration targetGen,
            Accessor readerAccessor,
            AsyncMode asyncMode,
            Accessor retAccessor,
            Accessor outItemAccessor,
            Accessor errorMaskAccessor,
            Accessor translationAccessor,
            Accessor converterAccessor,
            bool inline)
        {
            throw new NotImplementedException();
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
            var data = typeGen.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"{objGen.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class)}.SpecialWrite_{typeGen.Name}_Internal"))
            {
                args.AddPassArg("item");
                args.AddPassArg("writer");
            }
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }

        public override async Task<int?> GetPassedAmount(ObjectGeneration objGen, TypeGeneration typeGen) => 0;

        public override async Task<int?> ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen) => null;

        public override async Task GenerateWrapperRecordTypeParse(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor locationAccessor, 
            Accessor packageAccessor, 
            Accessor converterAccessor)
        {
            using (var args = new ArgsWrapper(fg,
                $"{typeGen.Name}SpecialParse"))
            {
                args.AddPassArg("stream");
                args.AddPassArg("offset");
                args.AddPassArg("type");
                args.AddPassArg("lastParsed");
            }
        }
    }
}
