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

        public override void GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string readerAccessor, Accessor itemAccessor, string maskAccessor, string translationMaskAccessor)
        {
            var data = typeGen.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"SpecialParse_{typeGen.Name}"))
            {
                args.Add("item: item");
                args.Add("frame: frame");
                args.Add("errorMask: errorMask");
            }
        }

        public override void GenerateCopyInRet(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            TypeGeneration targetGen, 
            string readerAccessor,
            bool squashedRepeatedList,
            string retAccessor,
            Accessor outItemAccessor,
            string maskAccessor,
            string translationMaskAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string writerAccessor, Accessor itemAccessor, string maskAccessor,
            string translationMaskAccessor)
        {
            var data = typeGen.GetFieldData();
            using (var args = new ArgsWrapper(fg,
                $"{objGen.ObjectName}.SpecialWrite_{typeGen.Name}_Internal"))
            {
                args.Add("item: item");
                args.Add("writer: writer");
                args.Add("errorMask: errorMask");
            }
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen)
        {
            throw new NotImplementedException();
        }
    }
}
