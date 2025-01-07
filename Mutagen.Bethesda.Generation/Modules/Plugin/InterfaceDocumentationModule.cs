using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Aspects;
using Noggog;
using Noggog.IO;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class InterfaceDocumentationModule : GenerationModule
{
    public override async Task FinalizeGeneration(ProtocolGeneration proto)
    {
        StructuredStringBuilder sb = new StructuredStringBuilder();

        if (LinkInterfaceModule.ObjectMappings.TryGetValue(proto.Protocol, out var linkInterfaces))
        {
            Dictionary<string, List<string>> reverse = new Dictionary<string, List<string>>();

            sb.AppendLine("# Link Interfaces");
            sb.AppendLine("Link Interfaces are used by FormLinks to point to several record types at once.  For example, a Container record might be able to contain Armors, Weapons, Ingredients, etc.");
            sb.AppendLine();
            sb.AppendLine("An interface would be defined such as 'IItem', which all Armor, Weapon, Ingredients would all implement.");
            sb.AppendLine();
            sb.AppendLine("A `FormLink<IItem>` could then point to all those record types by pointing to the interface instead.");
            sb.AppendLine($"## Interfaces to Concrete Classes");
            foreach (var interf in linkInterfaces.OrderBy(x => x.Key))
            {
                sb.AppendLine($"### {interf.Key}");
                foreach (var obj in interf.Value.OrderBy(o => o.Name))
                {
                    sb.AppendLine($"- {obj.Name}");
                    reverse.GetOrAdd(obj.Name).Add(interf.Key);
                }
            }

            sb.AppendLine($"## Concrete Classes to Interfaces");
            foreach (var obj in reverse.OrderBy(x => x.Key))
            {
                sb.AppendLine($"### {obj.Key}");
                foreach (var interf in obj.Value.OrderBy(x => x))
                {
                    sb.AppendLine($"- {interf}");
                }
            }
        }

        var path = Path.Combine(proto.DefFileLocation.FullName, $"../Documentation/LinkInterfaceDocumentation{Loqui.Generation.Constants.AutogeneratedMarkerString}.md");
        if (sb.Count > 0)
        {
            ExportStringToFile exportStringToFile = new(IFileSystemExt.DefaultFilesystem);
            exportStringToFile.ExportToFile(path, sb.GetString());
        }


        sb = new StructuredStringBuilder();

        if (AspectInterfaceModule.ObjectMappings.TryGetValue(proto.Protocol, out var aspectInterfaces))
        {
            Dictionary<string, List<string>> reverse = new Dictionary<string, List<string>>();

            sb.AppendLine("# Aspect Interfaces");
            sb.AppendLine("Aspect Interfaces expose common aspects of records.  For example, `INamed` are implemented by all records that have a `Name`.");
            sb.AppendLine();
            sb.AppendLine("Functions can then be written that take in `INamed`, allowing any record that has a name to be passed in.");
            sb.AppendLine($"## Interfaces to Concrete Classes");
            foreach (var interf in aspectInterfaces.OrderBy(x => x.Key.Nickname))
            {
                sb.AppendLine($"### {interf.Key.Nickname}");
                foreach (var obj in interf.Value
                             .SelectMany(x => x.Value)
                             .Distinct()
                             .OrderBy(x => x.Name))
                {
                    sb.AppendLine($"- {obj.Name}");
                    reverse.GetOrAdd(obj.Name).Add(interf.Key.Nickname);
                }
            }

            sb.AppendLine($"## Concrete Classes to Interfaces");
            foreach (var obj in reverse.OrderBy(x => x.Key))
            {
                sb.AppendLine($"### {obj.Key}");
                foreach (var interf in obj.Value.OrderBy(x => x))
                {
                    sb.AppendLine($"- {interf}");
                }
            }
        }

        path = Path.Combine(proto.DefFileLocation.FullName, $"../Documentation/AspectInterfaceDocumentation{Loqui.Generation.Constants.AutogeneratedMarkerString}.md");
        if (sb.Count > 0)
        {
            ExportStringToFile exportStringToFile = new(IFileSystemExt.DefaultFilesystem);
            exportStringToFile.ExportToFile(path, sb.GetString());
        }
    }
}