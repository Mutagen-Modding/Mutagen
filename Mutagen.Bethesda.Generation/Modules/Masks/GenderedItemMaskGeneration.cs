using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Internals;
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
                if (gendered.ItemHasBeenSet)
                {
                    return $"MaskItem<{typeStr}, {loqui.GetMaskString(typeStr)}?>?";
                }
                else
                {
                    return $"{loqui.GetMaskString(typeStr)}?";
                }
            }
            else
            {
                return typeStr;
            }
        }

        public override void GenerateForField(FileGeneration fg, TypeGeneration field, string typeStr)
        {
            if (!field.IntegrateField) return;
            GenderedType gendered = field as GenderedType;
            string maskStr;
            if (field.HasBeenSet || gendered.SubTypeGeneration is LoquiType)
            {
                maskStr = $"MaskItem<{typeStr}, GenderedItem<{SubMaskString(field, typeStr)}>?>?";
            }
            else
            {
                maskStr = $"GenderedItem<{SubMaskString(field, typeStr)}>";
            }
            fg.AppendLine($"public {maskStr} {field.Name};");
        }

        public override void GenerateForAll(FileGeneration fg, TypeGeneration field, Accessor accessor, bool nullCheck, bool indexed)
        {
            if (!field.IntegrateField) return;
            GenderedType gendered = field as GenderedType;
            var isLoqui = gendered.SubTypeGeneration is LoquiType;
            if (field.HasBeenSet || isLoqui)
            {
                using (var args = new ArgsWrapper(fg,
                    $"if (!{nameof(GenderedItem)}.{(isLoqui ? nameof(GenderedItem.AllMask) : nameof(GenderedItem.All))}",
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

        public override void GenerateForAny(FileGeneration fg, TypeGeneration field, Accessor accessor, bool nullCheck, bool indexed)
        {
            if (!field.IntegrateField) return;
            GenderedType gendered = field as GenderedType;
            var isLoqui = gendered.SubTypeGeneration is LoquiType;
            if (field.HasBeenSet || isLoqui)
            {
                using (var args = new ArgsWrapper(fg,
                    $"if ({nameof(GenderedItem)}.{(isLoqui ? nameof(GenderedItem.AnyMask) : nameof(GenderedItem.Any))}",
                    suffixLine: ") return true"))
                {
                    args.Add($"{accessor}");
                    args.AddPassArg("eval");
                }
            }
            else
            {
                fg.AppendLine($"if (eval({accessor}{(indexed ? ".Value" : null)}.Male) || eval({accessor}{(indexed ? ".Value" : null)}.Female)) return true;");
            }
        }

        public override void GenerateForTranslate(FileGeneration fg, TypeGeneration field, string retAccessor, string rhsAccessor, bool indexed)
        {
            if (!field.IntegrateField) return;
            var gendered = field as GenderedType;
            var loqui = gendered.SubTypeGeneration as LoquiType;
            if (field.HasBeenSet || loqui != null)
            {
                using (var args = new ArgsWrapper(fg,
                    $"{retAccessor} = GenderedItem.TranslateHelper"))
                {
                    args.Add($"{rhsAccessor}{(indexed ? ".Value" : null)}");
                    args.Add($"eval");
                    if (loqui != null)
                    {
                        args.Add($"(m, e) => m?.Translate(e)");
                    }
                }
            }
            else
            {
                using (var args = new ArgsWrapper(fg,
                    $"{retAccessor} = new GenderedItem<{SubMaskString(field, "R")}>"))
                {
                    if (loqui != null)
                    {
                        args.Add($"{rhsAccessor}.Male.Translate(eval)");
                        args.Add($"{rhsAccessor}.Female.Translate(eval)");
                    }
                    else
                    {
                        args.Add($"eval({rhsAccessor}.Male)");
                        args.Add($"eval({rhsAccessor}.Female)");
                    }
                }
            }
        }

        public override void GenerateForCtor(FileGeneration fg, TypeGeneration field, string typeStr, string valueStr)
        {
            if (!field.IntegrateField) return;
            var gendered = field as GenderedType;
            if (field.HasBeenSet || gendered.SubTypeGeneration is LoquiType)
            {
                fg.AppendLine($"this.{field.Name} = new MaskItem<{MaskModule.GenItem}, GenderedItem<{SubMaskString(field, typeStr)}>?>({valueStr}, default);");
            }
            else
            {
                fg.AppendLine($"this.{field.Name} = new GenderedItem<{SubMaskString(field, typeStr)}>({valueStr}, {valueStr});");
            }
        }

        public override void GenerateMaskToString(FileGeneration fg, TypeGeneration field, Accessor accessor, bool topLevel, bool printMask)
        {
            if (!field.IntegrateField) return;
            bool doIf;
            using (var args = new IfWrapper(fg, ANDs: true))
            {
                if (field.HasBeenSet)
                {
                    args.Add($"{accessor} != null");
                }
                if (printMask)
                {
                    args.Add($"({GenerateBoolMaskCheck(field, "printMask")})");
                }
                doIf = !args.Empty;
            }
            using (new BraceWrapper(fg, doIf))
            {
                fg.AppendLine($"fg.{nameof(FileGeneration.AppendLine)}($\"{field.Name} => {{{accessor}}}\");");
            }
        }

        public override string GenerateBoolMaskCheck(TypeGeneration field, string boolMaskAccessor)
        {
            if (field.HasBeenSet)
            {
                return $"{boolMaskAccessor}?.{field.Name}?.Overall ?? true";
            }
            else
            {
                // ToDo
                // Properly implement
                return $"true";
            }
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
