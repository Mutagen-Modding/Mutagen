using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class SByteBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<sbyte>
    {
        public SByteBinaryTranslationGeneration()
            : base(expectedLen: 1)
        {
            CustomRead = (fg, o, t, reader, item) =>
            {
                fg.AppendLine($"{item.DirectAccess} = {reader.DirectAccess}.ReadInt8();");
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
