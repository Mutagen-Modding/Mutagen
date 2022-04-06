using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

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

    public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
    {
        await base.GenerateInClass(obj, fg);

        switch (obj.GetObjectType())
        {
            case ObjectType.Record:
                await GenerateForRecord(obj, fg);
                break;
            case ObjectType.Mod:
                await GenerateForMod(obj, fg);
                break;
            case ObjectType.Subrecord:
            case ObjectType.Group:
            default:
                break;
        }
    }

    private async Task GenerateForMod(ObjectGeneration obj, FileGeneration fg)
    {
        using (var args = new FunctionWrapper(fg,
                   $"public static Task<{obj.Name}> CreateFromXmlFolder"))
        {
            args.Add("DirectoryPath dir");
            args.Add("ModKey modKey");
        }
        using (new BraceWrapper(fg))
        {
            using (var args = new ArgsWrapper(fg,
                       "return CreateFromXmlFolder"))
            {
                args.Add("dir: dir");
                args.Add("modKey: modKey");
                args.Add("errorMask: null");
            }
        }
        fg.AppendLine();

        using (var args = new FunctionWrapper(fg,
                   $"public static async Task<({obj.Name} Mod, {obj.Mask(MaskType.Error)} ErrorMask)> CreateFromXmlFolderWithErrorMask"))
        {
            args.Add("DirectoryPath dir");
            args.Add("ModKey modKey");
        }
        using (new BraceWrapper(fg))
        {
            fg.AppendLine("ErrorMaskBuilder? errorMaskBuilder = new ErrorMaskBuilder();");
            using (var args = new ArgsWrapper(fg,
                       "var ret = await CreateFromXmlFolder"))
            {
                args.Add("dir: dir");
                args.Add("modKey: modKey");
                args.Add("errorMask: errorMaskBuilder");
            }
            fg.AppendLine($"var errorMask = {obj.Mask(MaskType.Error)}.Factory(errorMaskBuilder);");
            fg.AppendLine("return (ret, errorMask);");
        }
        fg.AppendLine();

        using (var args = new FunctionWrapper(fg,
                   $"public static async Task<{obj.Name}> CreateFromXmlFolder"))
        {
            args.Add("DirectoryPath dir");
            args.Add("ModKey modKey");
            args.Add("ErrorMaskBuilder? errorMask");
        }
        using (new BraceWrapper(fg))
        {
            fg.AppendLine($"var item = new {obj.Name}(modKey);");
            fg.AppendLine($"var tasks = new List<Task>();");
            foreach (var field in obj.IterateFields())
            {
                if (field.GetFieldData().CustomFolder)
                {
                    using (var args = new ArgsWrapper(fg,
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
                        using (var args = new ArgsWrapper(fg,
                                   $"item.{field.Name}.CopyInFromXml"))
                        {
                            args.Add($"path: Path.Combine(dir.Path, \"{field.Name}.xml\")");
                            args.Add($"errorMask: errorMask");
                            args.Add($"translationMask: null");
                        }
                        break;
                    case ObjectType.Group:
                        if (!loqui.TryGetSpecificationAsObject("T", out var subObj)) continue;
                        using (var args = new ArgsWrapper(fg,
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
            fg.AppendLine("await Task.WhenAll(tasks);");
            fg.AppendLine("return item;");
        }
        fg.AppendLine();

        using (var args = new FunctionWrapper(fg,
                   $"public async Task<{obj.Mask(MaskType.Error)}?> WriteToXmlFolder"))
        {
            args.Add("DirectoryPath dir");
            args.Add("bool doMasks = true");
        }
        using (new BraceWrapper(fg))
        {
            fg.AppendLine($"ErrorMaskBuilder? errorMaskBuilder = null;");
            fg.AppendLine("dir.Create();");
            fg.AppendLine("using (new FolderCleaner(dir, FolderCleaner.CleanType.AccessTime))");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var tasks = new List<Task>();");
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui))
                    {
                        throw new ArgumentException();
                    }
                    if (field.GetFieldData().CustomFolder)
                    {
                        using (var args = new ArgsWrapper(fg,
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
                            using (var args = new ArgsWrapper(fg,
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
                                using (var args = new ArgsWrapper(fg,
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
                                using (var args = new ArgsWrapper(fg,
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
                fg.AppendLine("await Task.WhenAll(tasks);");
            }
            fg.AppendLine("return null;");
        }
    }

    private async Task GenerateForRecord(ObjectGeneration obj, FileGeneration fg)
    {
        if (!obj.IsTopClass) return;

        using (var args = new FunctionWrapper(fg,
                   $"public{obj.FunctionOverride()}async Task WriteToXmlFolder"))
        {
            args.Add("DirectoryPath dir");
            args.Add("string name");
            args.Add("XElement node");
            args.Add("int counter");
            args.Add($"ErrorMaskBuilder? errorMask");
        }
        using (new BraceWrapper(fg))
        {
            using (var args = new ArgsWrapper(fg,
                       "this.WriteToXml"))
            {
                args.Add("node: node");
                args.Add("errorMask: errorMask");
                args.Add("translationMask: null");
            }
        }
    }
}