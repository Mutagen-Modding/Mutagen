using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Binary
{
    public class SByteBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<sbyte>
    {
        public SByteBinaryTranslationGeneration()
            : base(expectedLen: 1)
        {
            CustomRead = (fg, o, t, reader, item) =>
            {
                fg.AppendLine($"{item} = {reader}.ReadInt8();");
                return true;
            };
        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen, 
            TypeGeneration typeGen,
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            return $"(sbyte){dataAccessor}[0]";
        }
    }
}
