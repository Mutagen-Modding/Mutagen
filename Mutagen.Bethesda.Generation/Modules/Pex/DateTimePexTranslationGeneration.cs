using Mutagen.Bethesda.Generation.Modules.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Pex
{
    public class DateTimePexTranslationGeneration : PrimitiveBinaryTranslationGeneration<DateTime>
    {
        public DateTimePexTranslationGeneration(bool? nullable = null) 
            : base(expectedLen: 8, nameof(DateTime), nullable)
        {
        }
    }
}
