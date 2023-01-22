using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Generation.Modules;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using BoolType = Mutagen.Bethesda.Generation.Fields.BoolType;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;
using EnumType = Mutagen.Bethesda.Generation.Fields.EnumType;
using FloatType = Mutagen.Bethesda.Generation.Fields.FloatType;
using PercentType = Mutagen.Bethesda.Generation.Fields.PercentType;
using StringType = Mutagen.Bethesda.Generation.Fields.StringType;

namespace Mutagen.Bethesda.Generation.Generator;

public class RecordGeneratorProvider : IGenerationConstructor
{
    private readonly TypicalRecordProtocolAdder _adder;
    private readonly IGenerateCoreSettings _generateCoreSettings;
    private readonly IRecordGeneration[] _recordGenerations;

    public RecordGeneratorProvider(
        TypicalRecordProtocolAdder adder,
        IGenerateCoreSettings generateCoreSettings,
        IRecordGeneration[] recordGenerations)
    {
        _adder = adder;
        _generateCoreSettings = generateCoreSettings;
        _recordGenerations = recordGenerations;
    }

    public LoquiGenerator Construct()
    {
        LoquiGenerator gen = new LoquiGenerator(typical: false)
        {
            NullableDefault = true,
            ToStringDefault = false,
        };
        gen.AddTypicalTypeAssociations();
        gen.Add(gen.MaskModule);
        gen.MaskModule.AddTypeAssociation<FormLinkType>(MaskModule.TypicalField);
        gen.MaskModule.AddTypeAssociation<GenderedType>(new GenderedItemMaskGeneration());
        gen.GenerationModules.Add(new PluginModule());
        gen.Add(new PluginTranslationModule(gen));
        gen.AddTypeAssociation<FormLinkType>("FormLink");
        gen.AddTypeAssociation<FormLinkOrAliasType>("FormLinkOrAlias");
        gen.AddTypeAssociation<FormIDType>("FormID");
        gen.AddTypeAssociation<FormKeyType>("FormKey");
        gen.AddTypeAssociation<ModKeyType>("ModKey");
        gen.AddTypeAssociation<RecordTypeType>("RecordType");
        gen.AddTypeAssociation<BufferType>("Buffer");
        gen.AddTypeAssociation<DataType>("Data");
        gen.AddTypeAssociation<ZeroType>("Zero");
        gen.AddTypeAssociation<CustomLogic>("CustomLogic");
        gen.AddTypeAssociation<GroupType>("Group");
        gen.AddTypeAssociation<GenderedType>("Gendered");
        gen.AddTypeAssociation<BreakType>("Break");
        gen.AddTypeAssociation<MarkerType>("Marker");
        gen.AddTypeAssociation<AssetLinkType>("AssetLink");
        gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, EnumType>();
        gen.ReplaceTypeAssociation<Loqui.Generation.StringType, StringType>();
        gen.ReplaceTypeAssociation<Loqui.Generation.LoquiType, MutagenLoquiType>();
        gen.ReplaceTypeAssociation<Loqui.Generation.FloatType, FloatType>();
        gen.ReplaceTypeAssociation<Loqui.Generation.PercentType, PercentType>();
        gen.ReplaceTypeAssociation<Loqui.Generation.DictType, DictType>();
        gen.ReplaceTypeAssociation<Loqui.Generation.BoolType, BoolType>();

        var bethesdaProto = gen.AddProtocol(
            new ProtocolGeneration(
                gen,
                new ProtocolKey("Bethesda"),
                new DirectoryInfo("../../../../Mutagen.Bethesda.Core/Plugins/Records"))
            {
                DefaultNamespace = "Mutagen.Bethesda.Plugins.Records",
                DoGeneration = _generateCoreSettings.ShouldGenerate
            });
        bethesdaProto.AddProjectToModify(
            new FileInfo(Path.Combine(bethesdaProto.GenerationFolder.FullName, "../../Mutagen.Bethesda.Core.csproj")));

        foreach (var recordGen in _recordGenerations)
        {
            _adder.Add(gen, recordGen.Name);
        }

        return gen;
    }
}
