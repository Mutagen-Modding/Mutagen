using Loqui.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Generation
{
    public class FormIDXmlTranslationGeneration : PrimitiveXmlTranslation<FormID>
    {
        public readonly static FormIDXmlTranslationGeneration Instance = new FormIDXmlTranslationGeneration();

        protected override FormID ParseNonNullString(string str)
        {
            if (FormID.TryFactory(str, out FormID parsed))
            {
                return parsed;
            }
            throw new ArgumentException($"Could not convert to {NullableName}");
        }
    }
}
