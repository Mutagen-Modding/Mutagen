using Loqui.Generation;
using Noggog;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

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

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (!obj.GetObjectData().MajorRecordFlags) return;
        sb.AppendLine("public MajorFlag MajorFlags");
        using (sb.CurlyBrace())
        {
            sb.AppendLine("get => (MajorFlag)this.MajorRecordFlagsRaw;");
            sb.AppendLine("set => this.MajorRecordFlagsRaw = (int)value;");
        }
    }

    public override async Task GenerateInInterface(ObjectGeneration obj, StructuredStringBuilder sb, bool internalInterface, bool getter)
    {
        if (!obj.GetObjectData().MajorRecordFlags || internalInterface) return;
        if (getter)
        {
            sb.AppendLine($"{obj.ObjectName}.MajorFlag MajorFlags {{ get; }}");
        }
        else
        {
            sb.AppendLine($"new {obj.ObjectName}.MajorFlag MajorFlags {{ get; set; }}");
        }
    }
}