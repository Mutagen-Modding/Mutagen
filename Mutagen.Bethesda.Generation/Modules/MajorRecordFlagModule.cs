using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Records;
using Noggog;
using System;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules
{
    public class MajorRecordFlagModule : GenerationModule
    {
        public override async Task PostLoad(ObjectGeneration obj)
        {
            obj.GetObjectData().MajorRecordFlags = obj.Node.GetAttribute("majorFlag", false);
            if (obj.GetObjectData().MajorRecordFlags 
                && obj.GetObjectType() != ObjectType.Record)
            {
                throw new ArgumentException();
            }
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (!obj.GetObjectData().MajorRecordFlags) return;
            fg.AppendLine("public MajorFlag MajorFlags");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("get => (MajorFlag)this.MajorRecordFlagsRaw;");
                fg.AppendLine("set => this.MajorRecordFlagsRaw = (int)value;");
            }
        }

        public override async Task GenerateInInterface(ObjectGeneration obj, FileGeneration fg, bool internalInterface, bool getter)
        {
            if (!obj.GetObjectData().MajorRecordFlags || internalInterface) return;
            if (getter)
            {
                fg.AppendLine($"{obj.ObjectName}.MajorFlag MajorFlags {{ get; }}");
            }
            else
            {
                fg.AppendLine($"new {obj.ObjectName}.MajorFlag MajorFlags {{ get; set; }}");
            }
        }
    }
}
