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

        protected override IEnumerable<string> AdditionalWriteParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string writerAccessor, Accessor itemAccessor, string maskAccessor)
        {
            if (ExtraByte(typeGen))
            {
                yield return "extraByte: true";
            }
        }

        protected override IEnumerable<string> AdditionalCopyInParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string nodeAccessor, Accessor itemAccessor, string maskAccessor)
        {
            if (ExtraByte(typeGen))
            {
                yield return "extraByte: true";
            }
        }

        protected override IEnumerable<string> AdditionalCopyInRetParameters(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, string nodeAccessor, string retAccessor, Accessor outItemAccessor, string maskAccessor)
        {
            if (ExtraByte(typeGen))
            {
                yield return "extraByte: true";
            }
        }
    }
}
