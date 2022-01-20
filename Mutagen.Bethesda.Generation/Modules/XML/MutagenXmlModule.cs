using Loqui;
using Loqui.Generation;
using Loqui.Internal;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation
{
    public class MutagenXmlModule : XmlTranslationModule
    {
        public MutagenXmlModule(LoquiGenerator gen)
            : base(gen)
        {
        }

        public override void GenerateWriteToNode(ObjectGeneration obj, FileGeneration fg)
        {
            using (var args = new FunctionWrapper(fg,
                $"public static void WriteToNode{ModuleNickname}{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, defs: obj.Generics));
                args.Add($"{obj.Interface(internalInterface: true, getter: true)} item");
                args.Add($"XElement {XmlTranslationModule.XElementLine.GetParameterName(obj)}");
                args.Add($"ErrorMaskBuilder? errorMask");
                args.Add($"{nameof(TranslationCrystal)}? translationMask");
            }
            using (new BraceWrapper(fg))
            {
                if (obj.HasLoquiBaseObject)
                {
                    using (var args = new ArgsWrapper(fg,
                        $"{this.TranslationWriteClass(obj.BaseClass)}.WriteToNode{ModuleNickname}"))
                    {
                        args.Add($"item: item");
                        args.Add($"{XmlTranslationModule.XElementLine.GetParameterName(obj)}: {XmlTranslationModule.XElementLine.GetParameterName(obj)}");
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
                        using (var args = new IfWrapper(fg, ANDs: true))
                        {
                            foreach (var item in conditions)
                            {
                                args.Add(item);
                            }
                        }
                    }
                    using (new BraceWrapper(fg, doIt: conditions.Count > 0))
                    {
                        var maskType = this.Gen.MaskModule.GetMaskModule(field.GetType()).GetErrorMaskTypeStr(field);
                        generator.GenerateWrite(
                            fg: fg,
                            objGen: obj,
                            typeGen: field,
                            writerAccessor: $"{XmlTranslationModule.XElementLine.GetParameterName(obj)}",
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
                            fg.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Has))");
                        }
                        using (new BraceWrapper(fg, doIt: dataType.Nullable))
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
                                    fg.AppendLine($"if (!item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Break{subField.BreakIndex}))");
                                    fg.AppendLine("{");
                                    fg.Depth++;
                                    encounteredBreakIndex++;
                                }
                                if (subField.Range != null && !isInRange)
                                {
                                    isInRange = true;
                                    fg.AppendLine($"if (item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Range{subField.RangeIndex}))");
                                    fg.AppendLine("{");
                                    fg.Depth++;
                                }
                                if (subField.Range == null && isInRange)
                                {
                                    isInRange = false;
                                    fg.Depth--;
                                    fg.AppendLine("}");
                                }
                                generateNormal(subGenerator, subField.Field);
                            }
                            for (int i = 0; i < encounteredBreakIndex; i++)
                            {
                                fg.Depth--;
                                fg.AppendLine("}");
                                if (i == encounteredBreakIndex - 1)
                                {
                                    fg.AppendLine("else");
                                    using (new BraceWrapper(fg))
                                    {
                                        fg.AppendLine($"node.Add(new XElement(\"Has{dataType.EnumName}\"));");
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
            fg.AppendLine();
        }

        private void HandleDataTypeParsing(ObjectGeneration obj, FileGeneration fg, DataType set, DataType.DataTypeIteration subField, ref bool isInRange)
        {
            if (subField.FieldIndex == 0 && set.Nullable)
            {
                fg.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Has;");
            }
            if (subField.BreakIndex != -1)
            {
                fg.AppendLine($"item.{set.StateName} &= ~{obj.Name}.{set.EnumName}.Break{subField.BreakIndex};");
            }
            if (subField.Range != null && !isInRange)
            {
                isInRange = true;
                fg.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Range{subField.RangeIndex};");
            }
            if (subField.Range == null && isInRange)
            {
                isInRange = false;
            }
        }

        protected override async Task PreCreateLoop(ObjectGeneration obj, FileGeneration fg)
        {
            foreach (var field in obj.IterateFields(nonIntegrated: true, expandSets: SetMarkerType.ExpandSets.FalseAndInclude))
            {
                if (!(field is DataType set)) continue;
                for (int i = 0; i < set.BreakIndices.Count; i++)
                {
                    fg.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Break{i};");
                }
            }
        }

        protected override void FillPrivateElement(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.IterateFields(includeBaseClass: true).Any(f => f.ReadOnly))
            {
                using (var args = new FunctionWrapper(fg,
                    $"protected static void FillPrivateElement{ModuleNickname}"))
                {
                    args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                    args.Add($"XElement {XmlTranslationModule.XElementLine.GetParameterName(obj)}");
                    args.Add("string name");
                    args.Add($"ErrorMaskBuilder? errorMask");
                    args.Add($"{nameof(TranslationCrystal)}? translationMask");
                }
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("switch (name)");
                    using (new BraceWrapper(fg))
                    {
                        bool isInRange = false;
                        foreach (var field in obj.IterateFields(expandSets: SetMarkerType.ExpandSets.FalseAndInclude, nonIntegrated: true))
                        {
                            if (field is DataType set)
                            {
                                if (set.Nullable)
                                {
                                    fg.AppendLine($"case \"Has{set.EnumName}\":");
                                    using (new DepthWrapper(fg))
                                    {
                                        fg.AppendLine($"item.{set.StateName} |= {obj.Name}.{set.EnumName}.Has;");
                                        fg.AppendLine("break;");
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
                                    fg.AppendLine($"case \"{subField.Field.Name}\":");
                                    using (new DepthWrapper(fg))
                                    {
                                        if (generator.ShouldGenerateCopyIn(subField.Field))
                                        {
                                            generator.GenerateCopyIn(
                                                fg: fg,
                                                objGen: obj,
                                                typeGen: subField.Field,
                                                nodeAccessor: XmlTranslationModule.XElementLine.GetParameterName(obj).Result,
                                                itemAccessor: Accessor.FromType(subField.Field, "item"),
                                                translationMaskAccessor: "translationMask",
                                                errorMaskAccessor: $"errorMask");
                                        }
                                        HandleDataTypeParsing(obj, fg, set, subField, ref isInRange);
                                        fg.AppendLine("break;");
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

                                fg.AppendLine($"case \"{field.Name}\":");
                                using (new DepthWrapper(fg))
                                {
                                    if (generator.ShouldGenerateCopyIn(field))
                                    {
                                        generator.GenerateCopyIn(
                                            fg: fg,
                                            objGen: obj,
                                            typeGen: field,
                                            nodeAccessor: XmlTranslationModule.XElementLine.GetParameterName(obj).Result,
                                            itemAccessor: Accessor.FromType(field, "item"),
                                            translationMaskAccessor: "translationMask",
                                            errorMaskAccessor: $"errorMask");
                                    }
                                    fg.AppendLine("break;");
                                }
                            }
                        }

                        fg.AppendLine("default:");
                        using (new DepthWrapper(fg))
                        {
                            if (obj.HasLoquiBaseObject)
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"{obj.BaseClass.CommonClass(LoquiInterfaceType.ISetter, CommonGenerics.Class, MaskType.Normal)}.FillPrivateElement{ModuleNickname}{obj.GetBaseMask_GenericTypes(MaskType.Error)}"))
                                {
                                    args.Add("item: item");
                                    args.Add($"{XmlTranslationModule.XElementLine.GetParameterName(obj)}: {XmlTranslationModule.XElementLine.GetParameterName(obj)}");
                                    args.Add("name: name");
                                    args.Add("errorMask: errorMask");
                                    if (this.TranslationMaskParameter)
                                    {
                                        args.Add($"translationMask: translationMask");
                                    }
                                }
                            }
                            fg.AppendLine("break;");
                        }
                    }
                }
                fg.AppendLine();
            }
        }

        protected override void FillPublicElement(ObjectGeneration obj, FileGeneration fg)
        {
            using (var args = new FunctionWrapper(fg,
                $"public static void FillPublicElement{ModuleNickname}"))
            {
                args.Add($"{obj.Interface(getter: false, internalInterface: true)} item");
                args.Add($"XElement {XmlTranslationModule.XElementLine.GetParameterName(obj)}");
                args.Add("string name");
                args.Add($"ErrorMaskBuilder? errorMask");
                args.Add($"{nameof(TranslationCrystal)}? translationMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("switch (name)");
                using (new BraceWrapper(fg))
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

                                fg.AppendLine($"case \"{subField.Field.Name}\":");
                                using (new DepthWrapper(fg))
                                {
                                    if (generator.ShouldGenerateCopyIn(subField.Field))
                                    {
                                        generator.GenerateCopyIn(
                                            fg: fg,
                                            objGen: obj,
                                            typeGen: subField.Field,
                                            nodeAccessor: XmlTranslationModule.XElementLine.GetParameterName(obj).Result,
                                            itemAccessor: Accessor.FromType(subField.Field, "item"),
                                            translationMaskAccessor: "translationMask",
                                            errorMaskAccessor: $"errorMask");
                                    }
                                    HandleDataTypeParsing(obj, fg, set, subField, ref isInRange);
                                    fg.AppendLine("break;");
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

                            fg.AppendLine($"case \"{field.Name}\":");
                            using (new DepthWrapper(fg))
                            {
                                if (generator.ShouldGenerateCopyIn(field))
                                {
                                    generator.GenerateCopyIn(
                                        fg: fg,
                                        objGen: obj,
                                        typeGen: field,
                                        nodeAccessor: XmlTranslationModule.XElementLine.GetParameterName(obj).Result,
                                        itemAccessor: Accessor.FromType(field, "item"),
                                        translationMaskAccessor: "translationMask",
                                        errorMaskAccessor: $"errorMask");
                                }
                                fg.AppendLine("break;");
                            }
                        }
                    }

                    fg.AppendLine("default:");
                    using (new DepthWrapper(fg))
                    {
                        if (obj.HasLoquiBaseObject)
                        {
                            using (var args = new ArgsWrapper(fg,
                                $"{this.TranslationCreateClass(obj.BaseClass)}.FillPublicElement{ModuleNickname}"))
                            {
                                args.Add("item: item");
                                args.Add($"{XmlTranslationModule.XElementLine.GetParameterName(obj)}: {XmlTranslationModule.XElementLine.GetParameterName(obj)}");
                                args.Add("name: name");
                                args.Add("errorMask: errorMask");
                                if (this.TranslationMaskParameter)
                                {
                                    args.Add($"translationMask: translationMask");
                                }
                            }
                        }
                        fg.AppendLine("break;");
                    }
                }
            }
            fg.AppendLine();
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
}
