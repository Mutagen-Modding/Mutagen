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

public class RecordGeneratorProvider
{
    private Lazy<LoquiGenerator> _generator;
    public LoquiGenerator Generator => _generator.Value;

    public RecordGeneratorProvider()
    {
        _generator = new Lazy<LoquiGenerator>(() =>
        {
            LoquiGenerator gen = new LoquiGenerator(typical: false)
            {
                NotifyingDefault = NotifyingType.None,
                NullableDefault = true,
                ToStringDefault = false,
            };
            gen.AddTypicalTypeAssociations();
            gen.Add(gen.MaskModule);
            gen.Namespaces.Add("Mutagen.Bethesda.Internals");
            gen.MaskModule.AddTypeAssociation<FormLinkType>(MaskModule.TypicalField);
            gen.MaskModule.AddTypeAssociation<GenderedType>(new GenderedItemMaskGeneration());
            gen.GenerationModules.Add(new PluginModule());
            gen.Add(new PluginTranslationModule(gen));
            gen.AddTypeAssociation<FormLinkType>("FormLink");
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
            gen.ReplaceTypeAssociation<Loqui.Generation.EnumType, EnumType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.StringType, StringType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.LoquiType, MutagenLoquiType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.FloatType, FloatType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.PercentType, PercentType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.DictType, DictType>();
            gen.ReplaceTypeAssociation<Loqui.Generation.BoolType, BoolType>();
            return gen;
        });
    }
}
