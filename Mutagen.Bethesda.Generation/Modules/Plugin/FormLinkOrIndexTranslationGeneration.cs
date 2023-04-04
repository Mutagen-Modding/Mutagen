using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class FormLinkOrIndexTranslationGeneration : FormLinkBinaryTranslationGeneration
{
    public override string Typename(TypeGeneration typeGen)
    {
        return "FormLinkOrIndex";
    }

    public override string GenerateForTypicalWrapper(ObjectGeneration objGen, TypeGeneration typeGen, Accessor dataAccessor,
        Accessor packageAccessor)
    {
        FormLinkType linkType = typeGen as FormLinkType;
        return $"{linkType.DirectTypeName(getter: true, internalInterface: true)}.Factory(this, FormKey.Factory({packageAccessor}.{nameof(BinaryOverlayFactoryPackage.MetaData)}.{nameof(ParsingBundle.MasterReferences)}!, BinaryPrimitives.ReadUInt32LittleEndian({dataAccessor})), BinaryPrimitives.ReadUInt32LittleEndian({dataAccessor}))";
    }

    public override async Task GenerateCopyIn(
        StructuredStringBuilder sb, 
        ObjectGeneration objGen,
        TypeGeneration typeGen,
        Accessor frameAccessor, 
        Accessor itemAccessor,
        Accessor errorMaskAccessor,
        Accessor translationAccessor)
    {
        var data = typeGen.GetFieldData();
        if (data.RecordType.HasValue)
        {
            sb.AppendLine($"{frameAccessor}.Position += {frameAccessor}.{nameof(MutagenBinaryReadStream.MetaData)}.{nameof(ParsingBundle.Constants)}.{nameof(GameConstants.SubConstants)}.{nameof(RecordHeaderConstants.HeaderLength)};");
        }

        using (var args = sb.Call(
                   $"{this.NamespacePrefix}{this.Typename(typeGen)}BinaryTranslation.Instance.ParseInto"))
        {
            args.Add("reader: frame");
            args.Add($"item: {itemAccessor}");
        }
    }
}