using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class FolderExportModule : GenerationModule
    {
        public override async Task<IEnumerable<string>> RequiredUsingStatements(ObjectGeneration obj)
        {
            return new string[]
            {
                "System.Threading.Tasks",
                "Noggog.Utility"
            };
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
                $"public static Task<{obj.Name}> Create_Xml_Folder"))
            {
                args.Add("DirectoryPath dir");
                args.Add("ModKey modKey");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    "return Create_Xml_Folder"))
                {
                    args.Add("dir: dir");
                    args.Add("modKey: modKey");
                    args.Add("errorMask: null");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public static async Task<({obj.Name} Mod, {obj.Mask(MaskType.Error)} ErrorMask)> Create_Xml_Folder_WithErrors"))
            {
                args.Add("DirectoryPath dir");
                args.Add("ModKey modKey");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("ErrorMaskBuilder errorMaskBuilder = new ErrorMaskBuilder();");
                using (var args = new ArgsWrapper(fg,
                    "var ret = await Create_Xml_Folder"))
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
                $"public static async Task<{obj.Name}> Create_Xml_Folder"))
            {
                args.Add("DirectoryPath dir");
                args.Add("ModKey modKey");
                args.Add("ErrorMaskBuilder errorMask");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var ret = new {obj.Name}(modKey);");
                fg.AppendLine($"var tasks = new List<Task>();");
                foreach (var field in obj.IterateFields())
                {
                    if (field.GetFieldData().CustomFolder)
                    {
                        using (var args = new ArgsWrapper(fg,
                            $"tasks.Add(Task.Run(() => ret.Create_Xml_Folder_{field.Name}",
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
                                $"ret.{field.Name}.CopyFieldsFrom({loqui.TypeName}.Create_Xml",
                                suffixLine: ");")
                            {
                                SemiColon = false
                            })
                            {
                                args.Add($"path: Path.Combine(dir.Path, \"{field.Name}.xml\")");
                                args.Add($"errorMask: errorMask");
                                args.Add($"translationMask: null");
                            }
                            break;
                        case ObjectType.Group:
                            if (!loqui.TryGetSpecificationAsObject("T", out var subObj)) continue;
                            using (var args = new ArgsWrapper(fg,
                                $"tasks.Add(Task.Run(() => ret.{field.Name}.Create_Xml_Folder<{subObj.Name}>",
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
                BinaryTranslationModule.GenerateModLinking(obj, fg);
                fg.AppendLine("return ret;");
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public async Task<{obj.Mask(MaskType.Error)}> Write_XmlFolder"))
            {
                args.Add("DirectoryPath dir");
                args.Add("bool doMasks = true");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"ErrorMaskBuilder errorMaskBuilder = null;");
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
                                $"tasks.Add(Task.Run(() =>  Write_Xml_Folder_{field.Name}",
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
                                    $"tasks.Add(Task.Run(() => this.{field.Name}.Write_Xml",
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
                                        $"tasks.Add(Task.Run(() => {field.Name}.Write_Xml_Folder<{subObj.Name}, {subObj.Mask(MaskType.Error)}>",
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
                                        $"tasks.Add(Task.Run(() => {field.Name}.Write_Xml_Folder",
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
                $"public{obj.FunctionOverride()}async Task Write_Xml_Folder"))
            {
                args.Add("DirectoryPath? dir");
                args.Add("string name");
                args.Add("XElement node");
                args.Add("int counter");
                args.Add($"ErrorMaskBuilder errorMask");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    "Write_Xml"))
                {
                    args.Add("node: node");
                    args.Add("errorMask: errorMask");
                    args.Add("translationMask: null");
                }
            }
        }
    }
}
