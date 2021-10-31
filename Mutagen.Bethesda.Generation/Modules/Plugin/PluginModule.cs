using Loqui.Generation;
using System;
using System.Threading.Tasks;
using Loqui;
using System.Xml.Linq;
using Noggog;
using Mutagen.Bethesda.Generation.Modules.Aspects;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Generation.Modules.Binary;
using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda.Generation.Modules
{
    public class PluginModule : GenerationModule
    {
        public override string RegionString => "Mutagen";

        public PluginModule()
        {
            this.SubModules.Add(new TriggeringRecordModule());
            this.SubModules.Add(new GenericsModule());
            this.SubModules.Add(new VersioningModule());
            this.SubModules.Add(new RecordTypeConverterModule());
            this.SubModules.Add(new CorrectnessModule());
            this.SubModules.Add(new ModModule());
            this.SubModules.Add(new ColorTypeModule());
            this.SubModules.Add(new LinkModule());
            this.SubModules.Add(new MajorRecordModule());
            this.SubModules.Add(new MajorRecordEnumerationModule());
            this.SubModules.Add(new ContainerParentModule());
            this.SubModules.Add(new MajorRecordFlagModule());
            this.SubModules.Add(new DataTypeModule());
            this.SubModules.Add(new LinkInterfaceModule());
            this.SubModules.Add(new WarmupModule());
            this.SubModules.Add(new MajorRecordRemovalModule());
            this.SubModules.Add(new MajorRecordContextEnumerationModule());
            this.SubModules.Add(new AspectInterfaceModule());
            this.SubModules.Add(new TypeOptionSolidifier());
            this.SubModules.Add(new LinkCacheExtensionsModule());
            this.SubModules.Add(new DuplicateModule());
            this.SubModules.Add(new GameCategoryExtensionsModule());
            this.SubModules.Add(new InterfaceDocumentationModule());
            this.SubModules.Add(new MapsToGetterModule());
            this.SubModules.Add(new GameEnvironmentStateModule());
            this.SubModules.Add(new MajorRecordLinkEqualityModule());
            this.SubModules.Add(new ImplicitsModule());
            this.SubModules.Add(new BreakMarkingModule());
        }

        public bool FieldFilter(TypeGeneration field)
        {
            var data = field.GetFieldData();
            if (data.RecordType.HasValue
                || field is NothingType
                || field is PrimitiveType
                || field is FormLinkType
                || field is ContainerType
                || field is DictType)
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
            if (!(field is LoquiType loqui)) return;
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
}
