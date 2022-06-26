using Loqui.Generation;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Generation.Fields;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation;

public class GenderedItemMaskGeneration : TypicalMaskFieldGeneration
{
    public string SubMaskString(TypeGeneration field, string typeStr)
    {
        GenderedType gendered = field as GenderedType;
        if (gendered.SubTypeGeneration is LoquiType loqui)
        {
            if (gendered.ItemNullable)
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

    public override void GenerateForField(StructuredStringBuilder sb, TypeGeneration field, string typeStr)
    {
        if (!field.IntegrateField) return;
        GenderedType gendered = field as GenderedType;
        string maskStr;
        if (field.Nullable || gendered.SubTypeGeneration is LoquiType)
        {
            maskStr = $"MaskItem<{typeStr}, GenderedItem<{SubMaskString(field, typeStr)}>?>?";
        }
        else
        {
            maskStr = $"GenderedItem<{SubMaskString(field, typeStr)}>";
        }
        sb.AppendLine($"public {maskStr} {field.Name};");
    }

    public override void GenerateForAll(StructuredStringBuilder sb, TypeGeneration field, Accessor accessor, bool nullCheck, bool indexed)
    {
        if (!field.IntegrateField) return;
        GenderedType gendered = field as GenderedType;
        var isLoqui = gendered.SubTypeGeneration is LoquiType;
        if (field.Nullable || isLoqui)
        {
            using (var args = sb.Call(
                       $"if (!{nameof(GenderedItem)}.{(isLoqui ? nameof(GenderedItem.AllMask) : nameof(GenderedItem.All))}",
                       suffixLine: ") return false"))
            {
                args.Add($"{accessor}");
                args.AddPassArg("eval");
            }
        }
        else
        {
            sb.AppendLine($"if (!eval({accessor}{(indexed ? ".Value" : null)}.Male) || !eval({accessor}{(indexed ? ".Value" : null)}.Female)) return false;");
        }
    }

    public override void GenerateForAny(StructuredStringBuilder sb, TypeGeneration field, Accessor accessor, bool nullCheck, bool indexed)
    {
        if (!field.IntegrateField) return;
        GenderedType gendered = field as GenderedType;
        var isLoqui = gendered.SubTypeGeneration is LoquiType;
        if (field.Nullable || isLoqui)
        {
            using (var args = sb.Call(
                       $"if ({nameof(GenderedItem)}.{(isLoqui ? nameof(GenderedItem.AnyMask) : nameof(GenderedItem.Any))}",
                       suffixLine: ") return true"))
            {
                args.Add($"{accessor}");
                args.AddPassArg("eval");
            }
        }
        else
        {
            sb.AppendLine($"if (eval({accessor}{(indexed ? ".Value" : null)}.Male) || eval({accessor}{(indexed ? ".Value" : null)}.Female)) return true;");
        }
    }

    public override void GenerateForTranslate(StructuredStringBuilder sb, TypeGeneration field, string retAccessor, string rhsAccessor, bool indexed)
    {
        if (!field.IntegrateField) return;
        var gendered = field as GenderedType;
        var loqui = gendered.SubTypeGeneration as LoquiType;
        if (field.Nullable || loqui != null)
        {
            using (var args = sb.Call(
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
            using (var args = sb.Call(
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

    public override void GenerateForCtor(StructuredStringBuilder sb, TypeGeneration field, string typeStr, string valueStr)
    {
        if (!field.IntegrateField) return;
        var gendered = field as GenderedType;
        if (field.Nullable || gendered.SubTypeGeneration is LoquiType)
        {
            sb.AppendLine($"this.{field.Name} = new MaskItem<{MaskModule.GenItem}, GenderedItem<{SubMaskString(field, typeStr)}>?>({valueStr}, default);");
        }
        else
        {
            sb.AppendLine($"this.{field.Name} = new GenderedItem<{SubMaskString(field, typeStr)}>({valueStr}, {valueStr});");
        }
    }

    public override void GenerateMaskToString(StructuredStringBuilder sb, TypeGeneration field, Accessor accessor, bool topLevel, bool printMask)
    {
        if (!field.IntegrateField) return;
        bool doIf;
        using (var args = sb.If(ands: true))
        {
            if (field.Nullable)
            {
                args.Add($"{accessor} != null");
            }
            if (printMask)
            {
                args.Add($"({GenerateBoolMaskCheck(field, "printMask")})");
            }
            doIf = !args.Empty;
        }
        using (sb.CurlyBrace(doIf))
        {
            sb.AppendLine($"sb.{nameof(StructuredStringBuilder.AppendLine)}($\"{field.Name} => {{{accessor}}}\");");
        }
    }

    public override string GenerateBoolMaskCheck(TypeGeneration field, string boolMaskAccessor)
    {
        if (field.Nullable)
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

    public override void GenerateSetException(StructuredStringBuilder sb, TypeGeneration field)
    {
        sb.AppendLine($"this.{field.Name} = new {GetErrorMaskTypeStr(field)}(ex, null);");
    }

    public override void GenerateForErrorMaskCombine(StructuredStringBuilder sb, TypeGeneration field, string accessor, string retAccessor, string rhsAccessor)
    {
        sb.AppendLine($"{retAccessor} = new {GetErrorMaskTypeStr(field)}(ExceptionExt.Combine({accessor}?.Overall, {rhsAccessor}?.Overall), GenderedItem.Combine({accessor}?.Specific, {rhsAccessor}?.Specific));");
    }

    public override void GenerateForTranslationMask(StructuredStringBuilder sb, TypeGeneration field)
    {
        sb.AppendLine($"public {GetTranslationMaskTypeStr(field)} {field.Name};");
    }

    public override string GetTranslationMaskTypeStr(TypeGeneration field)
    {
        GenderedType gendered = field as GenderedType;
        if (gendered.SubTypeGeneration is LoquiType loqui)
        {
            return $"GenderedItem<{loqui.Mask(MaskType.Translation)}>?";
        }
        else
        {
            return $"GenderedItem<bool>?";
        }
    }

    public override void GenerateForTranslationMaskSet(StructuredStringBuilder sb, TypeGeneration field, Accessor accessor, string onAccessor)
    {
        // Nothing
    }

    public override string GenerateForTranslationMaskCrystalization(TypeGeneration field)
    {
        GenderedType gendered = field as GenderedType;
        //ToDo
        //Implement crystal construction
        if (gendered.SubTypeGeneration is LoquiType loqui)
        {
            return $"({field.Name} != null || DefaultOn, null)";
        }
        else
        {
            return $"({field.Name} != null || DefaultOn, null)";
        }
    }
}