using Loqui.Generation;
using System;
using System.Threading.Tasks;
using Loqui;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation.Fields;
using Noggog;
using Mutagen.Bethesda.Generation.Modules.Aspects;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins.Records.Internals;
using DictType = Mutagen.Bethesda.Generation.Fields.DictType;

namespace Mutagen.Bethesda.Generation.Modules;

public class PluginModule : GenerationModule
{
    public override string RegionString => "Mutagen";

    public PluginModule()
    {
        SubModules.Add(new TriggeringRecordModule());
        SubModules.Add(new GenericsModule());
        SubModules.Add(new VersioningModule());
        SubModules.Add(new RecordTypeConverterModule());
        SubModules.Add(new CorrectnessModule());
        SubModules.Add(new ModModule());
        SubModules.Add(new ColorTypeModule());
        SubModules.Add(new LinkModule());
        SubModules.Add(new MajorRecordModule());
        SubModules.Add(new MajorRecordEnumerationModule());
        SubModules.Add(new ContainerParentModule());
        SubModules.Add(new MajorRecordFlagModule());
        SubModules.Add(new DataTypeModule());
        SubModules.Add(new LinkInterfaceModule());
        SubModules.Add(new WarmupModule());
        SubModules.Add(new MajorRecordRemovalModule());
        SubModules.Add(new MajorRecordContextEnumerationModule());
        SubModules.Add(new AspectInterfaceModule());
        SubModules.Add(new TypeOptionSolidifier());
        SubModules.Add(new LinkCacheExtensionsModule());
        SubModules.Add(new DuplicateModule());
        SubModules.Add(new GameCategoryExtensionsModule());
        SubModules.Add(new InterfaceDocumentationModule());
        SubModules.Add(new RecordTypeOrderExporterModule());
        SubModules.Add(new LastRequiredFieldModule());
        SubModules.Add(new MapsToGetterModule());
        SubModules.Add(new GameEnvironmentStateModule());
        SubModules.Add(new MajorRecordLinkEqualityModule());
        SubModules.Add(new ImplicitsModule());
        SubModules.Add(new BreakMarkingModule());
    }

    public bool FieldFilter(TypeGeneration field)
    {
        var data = field.GetFieldData();
        if (data.RecordType.HasValue
            || field is NothingType or PrimitiveType or FormLinkType or ContainerType or DictType)
        {
            return true;
        }

        if (field is GenderedType gender)
        {
            return FieldFilter(gender.SubTypeGeneration);
        }

        return false;
    }

    public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        var data = field.CustomData.GetOrAdd(Constants.DataKey, () => new MutagenFieldData(field)) as MutagenFieldData;
        data.Binary = node.GetAttribute<BinaryGenerationType>(Constants.Binary, BinaryGenerationType.Normal);
        data.BinaryOverlay = node.GetAttribute<BinaryGenerationType?>(Constants.BinaryOverlay, default);
        ModifyGRUPAttributes(field);
        await base.PostFieldLoad(obj, field, node);
        data.Length = node.GetAttribute<int?>(Constants.ByteLength, null);
        if (data.Length.HasValue) return;
        var index = obj.Fields.IndexOf(field);
        if (field.IntegrateField 
            && !FieldFilter(field)
            && field.GetFieldData().Binary == BinaryGenerationType.Normal
            && index != (obj.Fields.Count - 1))
        {
            throw new ArgumentException($"{obj.Name} {field.Name} have to define either length or record type.");
        }
    }

    private void ModifyGRUPAttributes(TypeGeneration field)
    {
        if (field is not LoquiType loqui) return;
        if (loqui.TargetObjectGeneration?.GetObjectType() != ObjectType.Group) return;
        loqui.Singleton = true;
        loqui.NullableProperty.OnNext((false, true));
        loqui.NotifyingProperty.OnNext((NotifyingType.None, true));
    }

    public override async Task PostLoad(ObjectGeneration obj)
    {
        await base.PostLoad(obj);
        obj.GetObjectData().CustomRecordFallback = obj.Node.GetAttribute(Constants.CustomFallback, false);
    }
}