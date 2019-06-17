using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class ScriptMetaSummary
    {
        internal int CompiledSizeInternal
        {
            set => this.CompiledSize = value;
        }
    }

    namespace Internals
    {
        public partial class ScriptMetaSummaryBinaryTranslation
        {
            static partial void FillBinaryCompiledSizeCustom(MutagenFrame frame, ScriptMetaSummary item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                frame.Position += 4;
            }

            static partial void WriteBinaryCompiledSizeCustom(MutagenWriter writer, IScriptMetaSummaryGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                Int32BinaryTranslation.Instance.Write(
                    writer,
                    item.CompiledSize);
            }
        }
    }
}
