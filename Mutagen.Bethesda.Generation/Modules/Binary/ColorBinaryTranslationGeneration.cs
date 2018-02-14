using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Loqui;
using Loqui.Generation;

namespace Mutagen.Bethesda.Generation
{
    public class ColorBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<Color>
    {
        protected bool ExtraByte(TypeGeneration typeGen)
        {
            if (!typeGen.CustomData.TryGetValue("ColorExtraByte", out var obj)) return false;
            return (bool)obj;
        }

        protected override IEnumerable<string> AdditionWriteParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string writerAccessor, Accessor itemAccessor, string doMaskAccessor, string maskAccessor)
        {
            if (ExtraByte(typeGen))
            {
                yield return "extraByte: true";
            }
        }

        protected override IEnumerable<string> AdditionCopyInParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string nodeAccessor, Accessor itemAccessor, string doMaskAccessor, string maskAccessor)
        {
            if (ExtraByte(typeGen))
            {
                yield return "extraByte: true";
            }
        }

        protected override IEnumerable<string> AdditionCopyInRetParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string nodeAccessor, Accessor retAccessor, string doMaskAccessor, string maskAccessor)
        {
            if (ExtraByte(typeGen))
            {
                yield return "extraByte: true";
            }
        }
    }
}
