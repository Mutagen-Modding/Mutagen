using Loqui.Generation;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using StringType = Mutagen.Bethesda.Generation.Fields.StringType;

namespace Mutagen.Bethesda.Generation;

public class StringXmlTranslationGeneration : Loqui.Generation.PrimitiveXmlTranslationGeneration<string>
{
    public override void GenerateWrite(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor writerAccessor, 
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor nameAccessor,
        Accessor translationMaskAccessor)
    {
        StringType str = typeGen as StringType;
        if (str.Translated.HasValue)
        {
            using (var args = sb.Call(
                       $"Mutagen.Bethesda.Xml.TranslatedStringXmlTranslation.Instance.Write"))
            {
                args.Add($"{XmlTranslationModule.XElementLine.GetParameterName(objGen)}: {writerAccessor}");
                args.Add($"name: {nameAccessor}");
                args.Add($"item: {ItemWriteAccess(typeGen, itemAccessor)}");
                if (typeGen.HasIndex)
                {
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                }
                args.Add($"errorMask: {errorMaskAccessor}");
            }
        }
        else
        {
            using (var args = sb.Call(
                       $"{this.TypeName(typeGen)}XmlTranslation.Instance.Write"))
            {
                args.Add($"{XmlTranslationModule.XElementLine.GetParameterName(objGen)}: {writerAccessor}");
                args.Add($"name: {nameAccessor}");
                args.Add($"item: {ItemWriteAccess(typeGen, itemAccessor)}");
                if (typeGen.HasIndex)
                {
                    args.Add($"fieldIndex: (int){typeGen.IndexEnumName}");
                }
                args.Add($"errorMask: {errorMaskAccessor}");
            }
        }
    }

    protected override string ItemWriteAccess(TypeGeneration typeGen, Accessor itemAccessor)
    {
        return itemAccessor.Access;
    }
}