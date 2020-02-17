using System;
using System.Collections.Generic;
using System.Text;

namespace Loqui.Generation
{
    public class GenderedItemMaskGeneration : TypicalMaskFieldGeneration
    {
        public override void GenerateForField(FileGeneration fg, TypeGeneration field, string typeStr)
        {
            if (!field.IntegrateField) return;
            if (field.HasBeenSet)
            {
                fg.AppendLine($"public MaskItem<{typeStr}, GenderedItem<{typeStr}>?>? {field.Name};");
            }
            else
            {
                fg.AppendLine($"public GenderedItem<{typeStr}>? {field.Name};");
            }
        }

        public override void GenerateForAllEqual(FileGeneration fg, TypeGeneration field, Accessor accessor, bool nullCheck, bool indexed)
        {
            if (!field.IntegrateField) return;
            if (field.HasBeenSet)
            {
                fg.AppendLine($"if ({accessor}?.Specific != null && (!eval({accessor}.Specific{(indexed ? ".Value" : null)}.Male) || !eval({accessor}.Specific{(indexed ? ".Value" : null)}.Female))) return false;");
            }
            else
            {
                fg.AppendLine($"if (!eval({accessor}{(indexed ? ".Value" : null)}.Male) || !eval({accessor}{(indexed ? ".Value" : null)}.Female)) return false;");
            }
        }

        public override void GenerateForTranslate(FileGeneration fg, TypeGeneration field, string retAccessor, string rhsAccessor, bool indexed)
        {
            if (!field.IntegrateField) return;
            fg.AppendLine($"if ({rhsAccessor}{(indexed ? ".Value" : null)} == null)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{retAccessor} = null;");
            }
            fg.AppendLine("else");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var spec = {rhsAccessor}{(indexed ? ".Value" : null)}.Specific == null ? null : new GenderedItem<R>(eval({rhsAccessor}{(indexed ? ".Value" : null)}.Specific.Male), eval({rhsAccessor}{(indexed ? ".Value" : null)}.Specific.Female));");
                fg.AppendLine($"{retAccessor} = new MaskItem<R, GenderedItem<R>?>(eval({rhsAccessor}{(indexed ? ".Value" : null)}.Overall), spec);");
            }
        }

        public override void GenerateForCtor(FileGeneration fg, TypeGeneration field, string typeStr, string valueStr)
        {
            if (!field.IntegrateField) return;
            fg.AppendLine($"this.{field.Name} = {(field.HasBeenSet ? $"new MaskItem<T, GenderedItem<T>?>({valueStr}, default)" : $"new GenderedItem<T>({valueStr}, {valueStr})")};");
        }

        public override void GenerateForErrorMaskToString(FileGeneration fg, TypeGeneration field, string accessor, bool topLevel)
        {
            if (!field.IntegrateField) return;
            fg.AppendLine($"if ({accessor} != null)");
            using (new BraceWrapper(fg))
            {
                base.GenerateForErrorMaskToString(fg, field, accessor, topLevel);
            }
        }

        public override string GenerateBoolMaskCheck(TypeGeneration field, string boolMaskAccessor)
        {
            return $"{boolMaskAccessor}?.{field.Name}?.Overall ?? true";
        }
    }
}
