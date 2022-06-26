using Loqui.Generation;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation.Fields;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules;

public class FolderExportModule : GenerationModule
{
    public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
    {
        yield return "System.Threading.Tasks";
        yield return "Noggog.Utility";
    }

    public override async Task PostFieldLoad(ObjectGeneration obj, TypeGeneration field, XElement node)
    {
        await base.PostFieldLoad(obj, field, node);
        field.GetFieldData().CustomFolder = node.TryGetAttribute<bool>("customFolder", out var customFolder) && customFolder;
    }

    public override async Task GenerateInClass(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        await base.GenerateInClass(obj, sb);

        switch (obj.GetObjectType())
        {
            case ObjectType.Record:
                await GenerateForRecord(obj, sb);
                break;
            case ObjectType.Mod:
                await GenerateForMod(obj, sb);
                break;
            case ObjectType.Subrecord:
            case ObjectType.Group:
            default:
                break;
        }
    }

    private async Task GenerateForMod(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        using (var args = sb.Function(
                   $"public static Task<{obj.Name}> CreateFromXmlFolder"))
        {
            args.Add("DirectoryPath dir");
            args.Add("ModKey modKey");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       "return CreateFromXmlFolder"))
            {
                args.Add("dir: dir");
                args.Add("modKey: modKey");
                args.Add("errorMask: null");
            }
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public static async Task<({obj.Name} Mod, {obj.Mask(MaskType.Error)} ErrorMask)> CreateFromXmlFolderWithErrorMask"))
        {
            args.Add("DirectoryPath dir");
            args.Add("ModKey modKey");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine("ErrorMaskBuilder? errorMaskBuilder = new ErrorMaskBuilder();");
            using (var args = sb.Call(
                       "var ret = await CreateFromXmlFolder"))
            {
                args.Add("dir: dir");
                args.Add("modKey: modKey");
                args.Add("errorMask: errorMaskBuilder");
            }
            sb.AppendLine($"var errorMask = {obj.Mask(MaskType.Error)}.Factory(errorMaskBuilder);");
            sb.AppendLine("return (ret, errorMask);");
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public static async Task<{obj.Name}> CreateFromXmlFolder"))
        {
            args.Add("DirectoryPath dir");
            args.Add("ModKey modKey");
            args.Add("ErrorMaskBuilder? errorMask");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"var item = new {obj.Name}(modKey);");
            sb.AppendLine($"var tasks = new List<Task>();");
            foreach (var field in obj.IterateFields())
            {
                if (field.GetFieldData().CustomFolder)
                {
                    using (var args = sb.Call(
                               $"tasks.Add(Task.Run(() => item.CreateFromXmlFolder{field.Name}",
                               suffixLine: "))"))
                    {
                        args.Add("dir: dir");
                        args.Add($"name: nameof({field.Name})");
                        args.Add($"index: (int){field.IndexEnumName}");
                        args.Add($"errorMask: errorMask");
                    }
                    continue;
                }
                if (!(field is LoquiType loqui))
                {
                    throw new ArgumentException();
                }
                switch (loqui.TargetObjectGeneration.GetObjectType())
                {
                    case ObjectType.Record:
                        using (var args = sb.Call(
                                   $"item.{field.Name}.CopyInFromXml"))
                        {
                            args.Add($"path: Path.Combine(dir.Path, \"{field.Name}.xml\")");
                            args.Add($"errorMask: errorMask");
                            args.Add($"translationMask: null");
                        }
                        break;
                    case ObjectType.Group:
                        if (!loqui.TryGetSpecificationAsObject("T", out var subObj)) continue;
                        using (var args = sb.Call(
                                   $"tasks.Add(Task.Run(() => item.{field.Name}.CreateFromXmlFolder<{subObj.Name}>",
                                   suffixLine: "))"))
                        {
                            args.Add($"dir: dir");
                            args.Add($"name: nameof({field.Name})");
                            args.Add($"errorMask: errorMask");
                            args.Add($"index: (int){field.IndexEnumName}");
                        }
                        break;
                    default:
                        break;
                }
            }
            sb.AppendLine("await Task.WhenAll(tasks);");
            sb.AppendLine("return item;");
        }
        sb.AppendLine();

        using (var args = sb.Function(
                   $"public async Task<{obj.Mask(MaskType.Error)}?> WriteToXmlFolder"))
        {
            args.Add("DirectoryPath dir");
            args.Add("bool doMasks = true");
        }
        using (sb.CurlyBrace())
        {
            sb.AppendLine($"ErrorMaskBuilder? errorMaskBuilder = null;");
            sb.AppendLine("dir.Create();");
            sb.AppendLine("using (new FolderCleaner(dir, FolderCleaner.CleanType.AccessTime))");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"var tasks = new List<Task>();");
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui))
                    {
                        throw new ArgumentException();
                    }
                    if (field.GetFieldData().CustomFolder)
                    {
                        using (var args = sb.Call(
                                   $"tasks.Add(Task.Run(() => WriteToXmlFolder{field.Name}",
                                   suffixLine: "))"))
                        {
                            args.Add("dir: dir");
                            args.Add($"name: nameof({field.Name})"); ;
                            args.Add($"index: (int){field.IndexEnumName}");
                            args.Add($"errorMask: errorMaskBuilder");
                        }
                        continue;
                    }
                    switch (loqui.TargetObjectGeneration.GetObjectType())
                    {
                        case ObjectType.Record:
                            using (var args = sb.Call(
                                       $"tasks.Add(Task.Run(() => this.{field.Name}.WriteToXml",
                                       suffixLine: "))"))
                            {
                                args.Add($"path: Path.Combine(dir.Path, \"{field.Name}.xml\")");
                                args.Add($"errorMask: errorMaskBuilder");
                                args.Add($"translationMask: null");
                            }
                            break;
                        case ObjectType.Group:
                            ObjectGeneration subObj;
                            if (field is GroupType group)
                            {
                                if (!group.TryGetSpecificationAsObject("T", out subObj)) continue;
                                using (var args = sb.Call(
                                           $"tasks.Add(Task.Run(() => {field.Name}.WriteToXmlFolder<{subObj.Name}, {subObj.Mask(MaskType.Error)}>",
                                           suffixLine: "))"))
                                {
                                    args.Add($"dir: dir.Path");
                                    args.Add($"name: nameof({field.Name})");
                                    args.Add($"errorMask: errorMaskBuilder");
                                    args.Add($"index: (int){field.IndexEnumName}");
                                }
                            }
                            else
                            {
                                using (var args = sb.Call(
                                           $"tasks.Add(Task.Run(() => {field.Name}.WriteToXmlFolder",
                                           suffixLine: "))"))
                                {
                                    args.Add($"dir: dir.Path");
                                    args.Add($"name: nameof({field.Name})");
                                    args.Add($"errorMask: errorMaskBuilder");
                                    args.Add($"index: (int){field.IndexEnumName}");
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                sb.AppendLine("await Task.WhenAll(tasks);");
            }
            sb.AppendLine("return null;");
        }
    }

    private async Task GenerateForRecord(ObjectGeneration obj, StructuredStringBuilder sb)
    {
        if (!obj.IsTopClass) return;

        using (var args = sb.Function(
                   $"public{obj.FunctionOverride()}async Task WriteToXmlFolder"))
        {
            args.Add("DirectoryPath dir");
            args.Add("string name");
            args.Add("XElement node");
            args.Add("int counter");
            args.Add($"ErrorMaskBuilder? errorMask");
        }
        using (sb.CurlyBrace())
        {
            using (var args = sb.Call(
                       "this.WriteToXml"))
            {
                args.Add("node: node");
                args.Add("errorMask: errorMask");
                args.Add("translationMask: null");
            }
        }
    }
}