using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Generation
{
    public class GenderedItemMaskGeneration : TypicalMaskFieldGeneration
    {
        public string SubMaskString(TypeGeneration field, string typeStr)
        {
            GenderedType gendered = field as GenderedType;
            if (gendered.SubTypeGeneration is LoquiType loqui)
            {
                return $"{loqui.GetMaskString(typeStr)}?";
            }
            else
            {
                return typeStr;
            }
        }

        public override void GenerateForField(FileGeneration fg, TypeGeneration field, string typeStr)
        {
            if (!field.IntegrateField) return;
            string maskStr;
            if (field.HasBeenSet)
            {
                maskStr = $"MaskItem<{typeStr}, GenderedItem<{SubMaskString(field, typeStr)}>?>";
            }
            else
            {
                maskStr = $"GenderedItem<{SubMaskString(field, typeStr)}>";
            }
            fg.AppendLine($"public {maskStr}? {field.Name};");
        }

        public override void GenerateForAllEqual(FileGeneration fg, TypeGeneration field, Accessor accessor, bool nullCheck, bool indexed)
        {
            if (!field.IntegrateField) return;
            GenderedType gendered = field as GenderedType;
            if (field.HasBeenSet)
            {
                var isLoqui = gendered.SubTypeGeneration is LoquiType;
                using (var args = new ArgsWrapper(fg,
                    $"if (!{nameof(GenderedItem)}.{(isLoqui ? nameof(GenderedItem.AllEqualMask) : nameof(GenderedItem.AllEqual))}",
                    suffixLine: ") return false"))
                {
                    args.Add($"{accessor}");
                    args.AddPassArg("eval");
                }
            }
            else
            {
                fg.AppendLine($"if (!eval({accessor}{(indexed ? ".Value" : null)}.Male) || !eval({accessor}{(indexed ? ".Value" : null)}.Female)) return false;");
            }
        }

        public override void GenerateForTranslate(FileGeneration fg, TypeGeneration field, string retAccessor, string rhsAccessor, bool indexed)
        {
            if (!field.IntegrateField) return;
            var gendered = field as GenderedType;
            fg.AppendLine($"if ({rhsAccessor}{(indexed ? ".Value" : null)} == null)");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{retAccessor} = null;");
            }
            fg.AppendLine("else");
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"{retAccessor} = new MaskItem<R, GenderedItem<{SubMaskString(field, "R")}>?>"))
                {
                    args.Add($"eval({rhsAccessor}{(indexed ? ".Value" : null)}.Overall)");
                    if (gendered.SubTypeGeneration is LoquiType loqui)
                    {
                        args.Add($"{rhsAccessor}{(indexed ? ".Value" : null)}.Specific == null ? null : new GenderedItem<{SubMaskString(field, "R")}>({rhsAccessor}{(indexed ? ".Value" : null)}.Specific.Male?.Translate(eval), {rhsAccessor}{(indexed ? ".Value" : null)}.Specific.Female?.Translate(eval))");
                    }
                    else
                    {
                        args.Add($"{rhsAccessor}{(indexed ? ".Value" : null)}.Specific == null ? null : new GenderedItem<R>(eval({rhsAccessor}{(indexed ? ".Value" : null)}.Specific.Male), eval({rhsAccessor}{(indexed ? ".Value" : null)}.Specific.Female))");
                    }
                }
            }
        }

        public override void GenerateForCtor(FileGeneration fg, TypeGeneration field, string typeStr, string valueStr)
        {
            if (!field.IntegrateField) return;
            fg.AppendLine($"this.{field.Name} = {(field.HasBeenSet ? $"new MaskItem<T, GenderedItem<{SubMaskString(field, typeStr)}>?>({valueStr}, default)" : $"new GenderedItem<{SubMaskString(field, typeStr)}>({valueStr}, {valueStr})")};");
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

        public override string GetErrorMaskTypeStr(TypeGeneration field)
        {
            return $"MaskItem<Exception?, GenderedItem<Exception?>?>";
        }

        public override void GenerateSetException(FileGeneration fg, TypeGeneration field)
        {
            fg.AppendLine($"this.{field.Name} = new {GetErrorMaskTypeStr(field)}(ex, null);");
        }

        public override void GenerateForErrorMaskCombine(FileGeneration fg, TypeGeneration field, string accessor, string retAccessor, string rhsAccessor)
        {
            fg.AppendLine($"{retAccessor} = new {GetErrorMaskTypeStr(field)}(ExceptionExt.Combine({accessor}?.Overall, {rhsAccessor}?.Overall), GenderedItem.Combine({accessor}?.Specific, {rhsAccessor}?.Specific));");
        }

        public override void GenerateForTranslationMask(FileGeneration fg, TypeGeneration field)
        {
            fg.AppendLine($"public {GetTranslationMaskTypeStr(field)} {field.Name};");
        }

        public override string GetTranslationMaskTypeStr(TypeGeneration field)
        {
            GenderedType gendered = field as GenderedType;
            if (gendered.SubTypeGeneration is LoquiType loqui)
            {
                return $"MaskItem<bool, GenderedItem<{loqui.Mask(MaskType.Translation)}?>?>";
            }
            else
            {
                return $"MaskItem<bool, GenderedItem<bool>?>";
            }
        }

        public override void GenerateForTranslationMaskSet(FileGeneration fg, TypeGeneration field, Accessor accessor, string onAccessor)
        {
            fg.AppendLine($"{accessor.DirectAccess} = new {this.GetTranslationMaskTypeStr(field)}({onAccessor}, default);");
        }

        public override string GenerateForTranslationMaskCrystalization(TypeGeneration field)
        {
            GenderedType gendered = field as GenderedType;
            //ToDo
            //Implement crystal construction
            if (gendered.SubTypeGeneration is LoquiType loqui)
            {
                return $"({field.Name}?.Overall ?? true, null)";
            }
            else
            {
                return $"({field.Name}?.Overall ?? true, null)";
            }
        }
    }
}
