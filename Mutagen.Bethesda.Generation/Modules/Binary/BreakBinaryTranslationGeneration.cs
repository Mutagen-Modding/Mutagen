using Loqui;
using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class BreakBinaryTranslationGeneration : BinaryTranslationGeneration
    {
        public override int? ExpectedLength(ObjectGeneration objGen, TypeGeneration typeGen)
        {
            return null;
        }

        public override async Task GenerateCopyIn(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor readerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor)
        {
            var fieldData = typeGen.GetFieldData();
            if (fieldData.BreakIndex == null) return;
            fg.AppendLine($"if (dataFrame.Complete)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"item.{DataTypeModule.VersioningFieldName} |= {objGen.ObjectName}.{DataTypeModule.VersioningEnumName}.Break{fieldData.BreakIndex};");
                string enumName = null;
                var startIndex = objGen.Fields.IndexOf(typeGen);
                for (int i = startIndex - 1; i >= 0; i--)
                {
                    if (!objGen.Fields.TryGet(i, out var prevField)) continue;
                    if (!prevField.IntegrateField) continue;
                    enumName = prevField.IndexEnumName;
                    break;
                }
                if (enumName != null)
                {
                    enumName = $"(int){enumName}";
                }
                fg.AppendLine($"return TryGet<int?>.Succeed({enumName ?? "null"});");
            }
        }

        public override void GenerateCopyInRet(FileGeneration fg, ObjectGeneration objGen, TypeGeneration targetGen, TypeGeneration typeGen, Accessor readerAccessor, AsyncMode asyncMode, Accessor retAccessor, Accessor outItemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWrite(FileGeneration fg, ObjectGeneration objGen, TypeGeneration typeGen, Accessor writerAccessor, Accessor itemAccessor, Accessor errorMaskAccessor, Accessor translationAccessor, Accessor converterAccessor)
        {
        }

        public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
        {
            throw new NotImplementedException();
        }
    }
}
