using System;
using Noggog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mutagen.Bethesda.Binary;
using Loqui;
using Loqui.Generation;

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
                if (this.HasBeenSet)
                {
                    return $"default({nameof(TranslatedString)}?)";
                }
                else
                {
                    return $"string.Empty";
                }
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

        public override void GenerateClear(FileGeneration fg, Accessor identifier)
        {
            if (this.Translated.HasValue
                && !this.HasBeenSet)
            {
                fg.AppendLine($"{identifier}.Clear();");
            }
            else
            {
                base.GenerateClear(fg, identifier);
            }
        }
    }
}
