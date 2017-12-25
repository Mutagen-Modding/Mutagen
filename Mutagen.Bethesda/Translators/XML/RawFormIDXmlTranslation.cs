using Loqui.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class RawFormIDXmlTranslation : PrimitiveXmlTranslation<RawFormID>
    {
        public readonly static RawFormIDXmlTranslation Instance = new RawFormIDXmlTranslation();

        protected override RawFormID ParseNonNullString(string str)
        {
            if (RawFormID.TryFactory(str, out RawFormID parsed))
            {
                return parsed;
            }
            throw new ArgumentException($"Could not convert to {NullableName}");
        }
    }
}
