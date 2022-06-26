using Loqui.Generation;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class PartialFormModule : GenerationModule
{
    // public bool IsApplicableToProtocol(ObjectGeneration obj)
    // {
    //     return obj.ProtoGen.ObjectGenerationsByID.Values.Any(CanHaveOrphanedGroups);
    // }

    public override async Task PostLoad(ObjectGeneration obj)
    {
        obj.GetObjectData().PartialForm = obj.Node.GetAttribute("partialForm", false);
        if (obj.GetObjectData().PartialForm 
            && obj.GetObjectType() != ObjectType.Record)
        {
            throw new ArgumentException();
        }
    }

    public override async Task GenerateInRegistration(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (obj.GetObjectData().PartialForm)
        {
            sb.AppendLine($"public static bool {Mutagen.Bethesda.Plugins.Internals.Constants.PartialFormMember} => true;");
        }
    }

    // public static bool CanHaveOrphanedGroups(ObjectGeneration obj)
    // {
    //     return SubgroupsModule.HasSubgroups(obj)
    //            && obj.GetObjectData().MajorRecordFlags
    //            && obj.GetObjectData().PartialForm;
    // }
}