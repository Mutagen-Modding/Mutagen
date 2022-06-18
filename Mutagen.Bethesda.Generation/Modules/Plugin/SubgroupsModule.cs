using System.Xml.Linq;
using Loqui.Generation;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class SubgroupsModule : GenerationModule
{
    public static bool HasSubgroups(ObjectGeneration obj) => obj.GetObjectData().Subgroups.Count > 0;
    
    public static async Task GenerateSubgroupsParseSnippet(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (!await obj.IsMajorRecord()) return;
        if (!HasSubgroups(obj)) return;
        using (var arg = sb.Call("public static partial void ParseSubgroupsLogic"))
        {
            arg.Add("MutagenFrame frame");
            arg.Add($"{obj.GetTypeName(LoquiInterfaceType.ISetter)} obj");
        }
        sb.AppendLine();
    }
    
    public static async Task GenerateSubgroupsWriteSnippet(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (!await obj.IsMajorRecord()) return;
        if (!HasSubgroups(obj)) return;
        using (var arg = sb.Call("public static partial void WriteSubgroupsLogic"))
        {
            arg.Add("MutagenWriter writer");
            arg.Add($"{obj.GetTypeName(LoquiInterfaceType.IGetter)} obj");
        }
        sb.AppendLine();
    }
    
    public static async Task GenerateSubgroupsWrite(ObjectGeneration obj, StructuredStringBuilder sb, Accessor accessor)
    {
        if (!await obj.IsMajorRecord()) return;
        if (!HasSubgroups(obj)) return;
        using (var arg = sb.Call("WriteSubgroupsLogic"))
        {
            arg.AddPassArg("writer");
            arg.Add($"obj: {accessor}");
        }
    }
    
    public static async Task GenerateSubgroupsOverlaySnippet(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (!await obj.IsMajorRecord()) return;
        if (!HasSubgroups(obj)) return;
        using (var arg = sb.Call("public partial void ParseSubgroupsLogic"))
        {
            arg.Add("OverlayStream stream");
            arg.Add($"int finalPos");
            arg.Add($"int offset");
        }
        sb.AppendLine();
    }

    public override async Task LoadWrapup(ObjectGeneration obj)
    {
        foreach (var elem in obj.Node.Elements(XName.Get("SubgroupType", LoquiGenerator.Namespace)))
        {
            obj.GetObjectData().Subgroups.Add(int.Parse(elem.Value));
        }
    }
}