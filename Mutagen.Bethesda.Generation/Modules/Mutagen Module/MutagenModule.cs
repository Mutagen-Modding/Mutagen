using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using System.Xml.Linq;
using Noggog;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda.Generation
{
    public class MutagenModule : GenerationModule
    {
        public override string RegionString => "Mutagen";

        public MutagenModule()
        {
            this.SubModules.Add(new TriggeringRecordModule());
            this.SubModules.Add(new GenericsModule());
            this.SubModules.Add(new SpecialParseModule());
            this.SubModules.Add(new DataTypeModule());
            this.SubModules.Add(new RecordTypeConverterModule());
            this.SubModules.Add(new NumFieldsModule());
            this.SubModules.Add(new CorrectnessModule());
            this.SubModules.Add(new ModModule());
        }

        public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
        {
            var data = field.CustomData.TryCreateValue(Constants.DATA_KEY, () => new MutagenFieldData(field)) as MutagenFieldData;
            data.Optional = node.GetAttribute<bool>("optional", false);
            if (data.Optional && !data.RecordType.HasValue)
            {
                throw new ArgumentException($"{obj.Name} {field.Name} cannot have an optional field if it is not a record typed field.");
            }
            data.Length = node.GetAttribute<long?>("length", null);
            if (field is ByteArrayType byteArray
                && !data.Length.HasValue)
            {
                data.Length = 4;
            }
            if (!data.Length.HasValue
                && !data.RecordType.HasValue
                && !(field is NothingType)
                && !(field is PrimitiveType)
                && !(field is ContainerType))
            {
                throw new ArgumentException($"{obj.Name} {field.Name} have to define either length or record type.");
            }
            data.IncludeInLength = node.GetAttribute<bool>("includeInLength", true);
            data.Vestigial = node.GetAttribute<bool>("vestigial", false);
            data.CustomBinary = node.GetAttribute<bool>("customBinary", false);
            ModifyGRUPAttributes(field);
            await base.PostFieldLoad(obj, field, node);
        }

        private void ModifyGRUPAttributes(TypeGeneration field)
        {
            if (!(field is LoquiType loqui)) return;
            if (loqui.TargetObjectGeneration?.GetObjectType() != ObjectType.Group) return;
            loqui.SingletonType = SingletonLevel.Singleton;
            loqui.HasBeenSetProperty.Set(false);
            loqui.NotifyingProperty.Set(false);
        }
    }
}
