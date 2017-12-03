using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal uint VariableCountInternal
        {
            set => this.VariableCount = value;
        }

        static partial void FillBinary_CompiledSize_Custom(MutagenFrame frame, IScriptMetaSummary item, int fieldIndex, Func<ScriptMetaSummary_ErrorMask> errorMask)
        {
            frame.Position += 4;
        }

        static partial void WriteBinary_CompiledSize_Custom(MutagenWriter writer, IScriptMetaSummaryGetter item, int fieldIndex, Func<ScriptMetaSummary_ErrorMask> errorMask)
        {
            Int32BinaryTranslation.Instance.Write(
                writer,
                item.CompiledSize,
                fieldIndex,
                errorMask);
        }

        static partial void FillBinary_VariableCount_Custom(MutagenFrame frame, IScriptMetaSummary item, int fieldIndex, Func<ScriptMetaSummary_ErrorMask> errorMask)
        {
            frame.Position += 4;
        }

        static partial void WriteBinary_VariableCount_Custom(MutagenWriter writer, IScriptMetaSummaryGetter item, int fieldIndex, Func<ScriptMetaSummary_ErrorMask> errorMask)
        {
            UInt32BinaryTranslation.Instance.Write(
                writer,
                item.VariableCount,
                fieldIndex,
                errorMask);
        }
    }
}
