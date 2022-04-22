using Loqui;
using Loqui.Generation;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation;

public class DataTypeXmlTranslationGeneration : XmlTranslationGeneration
{
    public override bool ShouldGenerateWrite(TypeGeneration typeGen)
    {
        return true;
    }

    public override void GenerateCopyIn(
        FileGeneration fg,
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor nodeAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationMaskAccessor)
    {
        throw new NotImplementedException();
    }

    public override void GenerateForCommonXSD(XElement rootElement, TypeGeneration typeGen)
    {
        throw new NotImplementedException();
    }

    public override XElement GenerateForXSD(
        ObjectGeneration objGen,
        XElement rootElement,
        XElement choiceElement,
        TypeGeneration typeGen,
        string nameOverride)
    {
        throw new NotImplementedException();
    }

    public override void GenerateWrite(
        FileGeneration fg,
        ObjectGeneration obj,
        TypeGeneration typeGen,
        Accessor writerAccessor,
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor nameAccessor,
        Accessor translationMaskAccessor)
    {
        var dataType = typeGen as DataType;
        bool isInRange = false;
        var origDepth = fg.Depth;
        foreach (var subField in dataType.IterateFieldsWithMeta())
        {
            if (!this.XmlMod.TryGetTypeGeneration(subField.Field.GetType(), out var subGenerator))
            {
                throw new ArgumentException("Unsupported type generator: " + subField.Field);
            }

            var subData = subField.Field.GetFieldData();
            if (!subGenerator.ShouldGenerateWrite(subField.Field)) continue;
            if (subField.BreakIndex != -1)
            {
                fg.AppendLine($"if (!item.{dataType.StateName}.HasFlag({obj.Name}.{dataType.EnumName}.Break{subField.BreakIndex}))");
                fg.AppendLine("{");
                fg.Depth++;
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

            List<string> conditions = new List<string>();
            if (this.XmlMod.TranslationMaskParameter
                && subField.Field.IntegrateField)
            {
                conditions.Add(subField.Field.GetTranslationIfAccessor("translationMask"));
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
                subGenerator.GenerateWrite(
                    fg: fg,
                    objGen: obj,
                    nameAccessor: $"nameof(item.{subField.Field.Name})",
                    typeGen: subField.Field,
                    writerAccessor: writerAccessor,
                    translationMaskAccessor: "translationMask",
                    itemAccessor: Accessor.FromType(subField.Field, "item"),
                    errorMaskAccessor: $"errorMask");
            }
        }
        for (int i = fg.Depth - origDepth; i > 0; i--)
        {
            fg.Depth--;
            fg.AppendLine("}");
        }
    }

    public override string GetTranslatorInstance(TypeGeneration typeGen, bool getter)
    {
        throw new NotImplementedException();
    }
}