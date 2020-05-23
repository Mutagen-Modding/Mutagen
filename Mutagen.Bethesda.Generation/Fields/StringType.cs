using System;
using Noggog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
    public class StringType : Loqui.Generation.StringType
    {
        public StringBinaryType BinaryType;
        public StringsSource? Translated;

        public override string TypeName(bool getter, bool needsCovariance = false)
        {
            if (this.Translated.HasValue)
            {
                return nameof(TranslatedString);
            }
            else
            {
                return base.TypeName(getter, needsCovariance);
            }
        }

        public override string GetDefault(bool getter)
        {
            if (this.Translated.HasValue)
            {
                return $"default({nameof(TranslatedString)}{this.NullChar})";
            }
            else
            {
                return base.GetDefault(getter);
            }
        }

        public override async Task Load(XElement node, bool requireName = true)
        {
            this.BinaryType = node.GetAttribute<StringBinaryType>("binaryType", defaultVal: StringBinaryType.NullTerminate);
            this.Translated = node.GetAttribute<StringsSource?>("translated", defaultVal: null);
            await base.Load(node, requireName);
        }
    }
}
