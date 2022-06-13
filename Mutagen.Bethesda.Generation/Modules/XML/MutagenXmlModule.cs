using Loqui.Generation;
using Loqui.Internal;
using Mutagen.Bethesda.Generation.Fields;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation;

public class MutagenXmlModule : XmlTranslationModule
{
    public MutagenXmlModule(LoquiGenerator gen)
        : base(gen)
    {
    }

    public override void GenerateWriteToNode(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        using (var args = sb.Function(
                   $"public static void WriteToNode{ModuleNickname}{obj.GetGenericTypes(MaskType.Normal)}"))
        {
            args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
            args.Add($"{obj.Interface(internalInterface: true, getter: true)} item");
            args.Add($"XElement {XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend)}");
            args.Add($"ErrorMaskBuilder? errorMask");
            args.Add($"{nameof(TranslationCrystal)}? translationMask");
        }
        using (sb.CurlyBrace())
        {
            if (obj.HasLoquiBaseObject)
            {
                using (var args = sb.Call(
                           $"{this.TranslationWriteClass(obj.BaseClass)}.WriteToNode{ModuleNickname}"))
                {
                    args.Add($"item: item");
                    args.AddPassArg($"{XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend)}");
                    args.Add($"errorMask: errorMask");
                    args.Add($"translationMask: translationMask");
                }
            }

            void generateNormal(XmlTranslationGeneration generator, TypeGeneration field)
            {
                if (!generator.ShouldGenerateWrite(field)) return;

                List<string> conditions = new List<string>();
                if (field.Nullable)
                {
                    conditions.Add($"{field.NullableAccessor(getter: true, accessor: Accessor.FromType(field, "item"))}");
                }
                if (this.TranslationMaskParameter)
                {
                    conditions.Add(field.GetTranslationIfAccessor("translationMask"));
                }
                if (conditions.Count > 0)
                {
                    using (var args = sb.If(ands: true))
                    {
                        foreach (var item in conditions)
                        {
                            args.Add(item);
                        }
                    }
                }
                using (sb.CurlyBrace(doIt: conditions.Count > 0))
                {
                    var maskType = this.Gen.MaskModule.GetMaskModule(field.GetType()).GetErrorMaskTypeStr(field);
                    generator.GenerateWrite(
                        sb: sb,
                        objGen: obj,
                        typeGen: field,
                        writerAccessor: $"{XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend)}",
                        itemAccessor: Accessor.FromType(field, "item"),
                        errorMaskAccessor: $"errorMask",
                        translationMaskAccessor: "translationMask",
                        nameAccessor: $"nameof(item.{field.Name})");
                }
            }

            foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
            {
                if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                {
                    if (!field.IntegrateField) continue;
                    throw new ArgumentException("Unsupported type generator: " + field);
                }

                if (field is DataType dataType)
                {
                    if (dataType.Nullable)
                    {
                        sb.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Has))");
                    }
                    using (sb.CurlyBrace(doIt: dataType.Nullable))
                    {
                        bool isInRange = false;
                        int encounteredBreakIndex = 0;
                        foreach (var subField in dataType.IterateFieldsWithMeta())
                        {
                            if (!this.TryGetTypeGeneration(subField.Field.GetType(), out var subGenerator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + subField.Field);
                            }

                            var subData = subField.Field.GetFieldData();
                            if (!subGenerator.ShouldGenerateCopyIn(subField.Field)) continue;
                            if (subField.BreakIndex != -1)
                            {
                                sb.AppendLine($"if (!item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Break{subField.BreakIndex}))");
                                sb.AppendLine("{");
                                sb.Depth++;
                                encounteredBreakIndex++;
                            }
                            if (subField.Range != null && !isInRange)
                            {
                                isInRange = true;
                                sb.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Range{subField.RangeIndex}))");
                                sb.AppendLine("{");
                                sb.Depth++;
                            }
                            if (subField.Range == null && isInRange)
                            {
                                isInRange = false;
                                sb.Depth--;
                                sb.AppendLine("}");
                            }
                            generateNormal(subGenerator, subField.Field);
                        }
                        for (int i = 0; i < encounteredBreakIndex; i++)
                        {
                            sb.Depth--;
                            sb.AppendLine("}");
                            if (i == encounteredBreakIndex - 1)
                            {
                                sb.AppendLine("else");
                                using (sb.CurlyBrace())
                                {
                                    sb.AppendLine($"node.Add(new XElement(\"Has{dataType.EnumName}\"));");
                                }
                            }
                        }
                    }
                }
                else
                {
                    generateNormal(generator, field);
                }
            }
        }
        sb.AppendLine();
    }

    private void HandleDataTypeParsing(ObjectGeneration obj, StructuredStringBuilder sb, DataType set, DataType.DataTypeIteration subField, ref bool isInRange)
    {
        if (subField.FieldIndex == 0 && set.Nullable)
        {
            sb.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Has;");
        }
        if (subField.BreakIndex != -1)
        {
            sb.AppendLine($"item.{set.StateName} &= ~{obj.Name}.{set.EnumName}.Break{subField.BreakIndex};");
        }
        if (subField.Range != null && !isInRange)
        {
            isInRange = true;
            sb.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Range{subField.RangeIndex};");
        }
        if (subField.Range == null && isInRange)
        {
            isInRange = false;
        }
    }

    protected override async Task PreCreateLoop(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        foreach (var field in obj.IterateFields(nonIntegrated: true, expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
        {
            if (!(field is DataType set)) continue;
            for (int i = 0; i < set.BreakIndices.Count; i++)
            {
                sb.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Break{i};");
            }
        }
    }

    protected override void FillPrivateElement(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.IterateFields(includeBaseClass: true).Any(f => f.ReadOnly))
        {
            using (var args = sb.Function(
                       $"protected static void FillPrivateElement{ModuleNickname}"))
            {
                args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                args.Add($"XElement {XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend)}");
                args.Add("string name");
                args.Add($"ErrorMaskBuilder? errorMask");
                args.Add($"{nameof(TranslationCrystal)}? translationMask");
            }
            using (sb.CurlyBrace())
            {
                sb.AppendLine("switch (name)");
                using (sb.CurlyBrace())
                {
                    bool isInRange = false;
                    foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
                    {
                        if (field is DataType set)
                        {
                            if (set.Nullable)
                            {
                                sb.AppendLine($"case \"Has{set.EnumName}\":");
                                using (sb.IncreaseDepth())
                                {
                                    sb.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Has;");
                                    sb.AppendLine("break;");
                                }
                            }
                            foreach (var subField in set.IterateFieldsWithMeta())
                            {
                                if (subField.Field.Derivative) continue;
                                if (!subField.Field.ReadOnly) continue;
                                if (!subField.Field.IntegrateField) continue;
                                if (!this.TryGetTypeGeneration(subField.Field.GetType(), out var generator))
                                {
                                    throw new ArgumentException("Unsupported type generator: " + subField.Field);
                                }
                                sb.AppendLine($"case \"{subField.Field.Name}\":");
                                using (sb.IncreaseDepth())
                                {
                                    if (generator.ShouldGenerateCopyIn(subField.Field))
                                    {
                                        generator.GenerateCopyIn(
                                            sb: sb,
                                            objGen: obj,
                                            typeGen: subField.Field,
                                            nodeAccessor: XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend).Result,
                                            itemAccessor: Accessor.FromType(subField.Field, "item"),
                                            translationMaskAccessor: "translationMask",
                                            errorMaskAccessor: $"errorMask");
                                    }
                                    HandleDataTypeParsing(obj, sb, set, subField, ref isInRange);
                                    sb.AppendLine("break;");
                                }
                            }
                        }
                        else if (field.IntegrateField)
                        {
                            if (field.Derivative) continue;
                            if (!field.ReadOnly) continue;
                            if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + field);
                            }

                            sb.AppendLine($"case \"{field.Name}\":");
                            using (sb.IncreaseDepth())
                            {
                                if (generator.ShouldGenerateCopyIn(field))
                                {
                                    generator.GenerateCopyIn(
                                        sb: sb,
                                        objGen: obj,
                                        typeGen: field,
                                        nodeAccessor: XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend).Result,
                                        itemAccessor: Accessor.FromType(field, "item"),
                                        translationMaskAccessor: "translationMask",
                                        errorMaskAccessor: $"errorMask");
                                }
                                sb.AppendLine("break;");
                            }
                        }
                    }

                    sb.AppendLine("default:");
                    using (sb.IncreaseDepth())
                    {
                        if (obj.HasLoquiBaseObject)
                        {
                            using (var args = sb.Call(
                                       $"{obj.BaseClass.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class, MaskType.Normal)}.FillPrivateElement{ModuleNickname}{obj.GetBaseMask_GenericTypes(MaskType.Error)}"))
                            {
                                args.Add("item: item");
                                args.AddPassArg($"{XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend)}");
                                args.Add("name: name");
                                args.Add("errorMask: errorMask");
                                if (this.TranslationMaskParameter)
                                {
                                    args.Add($"translationMask: translationMask");
                                }
                            }
                        }
                        sb.AppendLine("break;");
                    }
                }
            }
            sb.AppendLine();
        }
    }

    protected override void FillPublicElement(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        using (var args = sb.Function(
                   $"public static void FillPublicElement{ModuleNickname}"))
        {
            args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
            args.Add($"XElement {XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend)}");
            args.Add("string name");
            args.Add($"ErrorMaskBuilder? errorMask");
            args.Add($"{nameof(TranslationCrystal)}? translationMask");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("switch (name)");
            using (sb.CurlyBrace())
            {
                bool isInRange = false;
                foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
                {
                    if (field is DataType set)
                    {
                        foreach (var subField in set.IterateFieldsWithMeta())
                        {
                            if (subField.Field.Derivative) continue;
                            if (subField.Field.ReadOnly) continue;
                            if (!subField.Field.IntegrateField) continue;
                            if (!this.TryGetTypeGeneration(subField.Field.GetType(), out var generator))
                            {
                                throw new ArgumentException("Unsupported type generator: " + subField.Field);
                            }

                            sb.AppendLine($"case \"{subField.Field.Name}\":");
                            using (sb.IncreaseDepth())
                            {
                                if (generator.ShouldGenerateCopyIn(subField.Field))
                                {
                                    generator.GenerateCopyIn(
                                        sb: sb,
                                        objGen: obj,
                                        typeGen: subField.Field,
                                        nodeAccessor: XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend).Result,
                                        itemAccessor: Accessor.FromType(subField.Field, "item"),
                                        translationMaskAccessor: "translationMask",
                                        errorMaskAccessor: $"errorMask");
                                }
                                HandleDataTypeParsing(obj, sb, set, subField, ref isInRange);
                                sb.AppendLine("break;");
                            }
                        }
                    }
                    else if (field.IntegrateField)
                    {
                        if (field.Derivative) continue;
                        if (field.ReadOnly) continue;
                        if (!this.TryGetTypeGeneration(field.GetType(), out var generator))
                        {
                            throw new ArgumentException("Unsupported type generator: " + field);
                        }

                        sb.AppendLine($"case \"{field.Name}\":");
                        using (sb.IncreaseDepth())
                        {
                            if (generator.ShouldGenerateCopyIn(field))
                            {
                                generator.GenerateCopyIn(
                                    sb: sb,
                                    objGen: obj,
                                    typeGen: field,
                                    nodeAccessor: XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend).Result,
                                    itemAccessor: Accessor.FromType(field, "item"),
                                    translationMaskAccessor: "translationMask",
                                    errorMaskAccessor: $"errorMask");
                            }
                            sb.AppendLine("break;");
                        }
                    }
                }

                sb.AppendLine("default:");
                using (sb.IncreaseDepth())
                {
                    if (obj.HasLoquiBaseObject)
                    {
                        using (var args = sb.Call(
                                   $"{this.TranslationCreateClass(obj.BaseClass)}.FillPublicElement{ModuleNickname}"))
                        {
                            args.Add("item: item");
                            args.AddPassArg($"{XmlTranslationModule.XElementLine.GetParameterName(obj, Context.Backend)}");
                            args.Add("name: name");
                            args.Add("errorMask: errorMask");
                            if (this.TranslationMaskParameter)
                            {
                                args.Add($"translationMask: translationMask");
                            }
                        }
                    }
                    sb.AppendLine("break;");
                }
            }
        }
        sb.AppendLine();
    }

    public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
    {
        await foreach (var item in base.RequiredUsingStatements(obj))
        {
            yield return item;
        }
        yield return "Mutagen.Bethesda.Xml";
    }
}