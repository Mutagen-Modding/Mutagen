using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class FolderExportModule : GenerationModule
    {
        public override IEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
        {
            if (obj.GetObjectType() != ObjectType.Mod) yield break;
            yield return "System.Threading.Tasks";
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
                    GenerateForMod(obj, fg);
                    break;
                case ObjectType.Subrecord:
                case ObjectType.Group:
                default:
                    break;
            }
        }

        private void GenerateForMod(ObjectGeneration obj, FileGeneration fg)
        {
            using (var args = new FunctionWrapper(fg,
                $"public async Task<{obj.Mask(MaskType.Error)}> Write_XmlFolder"))
            {
                args.Add("DirectoryPath dir");
                args.Add("bool doMasks = true");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{obj.Mask(MaskType.Error)} errMaskRet = null;");
                fg.AppendLine("List<Task> tasks = new List<Task>();");
                fg.AppendLine($"Func<{obj.Mask(MaskType.Error)}> errMaskFunc = doMasks ? () => errMaskRet ?? (errMaskRet = new {obj.Mask(MaskType.Error)}()) : default(Func<{obj.Mask(MaskType.Error)}>);");
                fg.AppendLine("try");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("dir.Create();");
                    foreach (var field in obj.IterateFields())
                    {
                        if (!(field is LoquiType loqui))
                        {
                            throw new ArgumentException();
                        }
                        switch (loqui.TargetObjectGeneration.GetObjectType())
                        {
                            case ObjectType.Record:
                                using (var args = new ArgsWrapper(fg,
                                    $"this.{field.Name}.Write_Xml_Folder"))
                                {
                                    args.Add($"path: Path.Combine(dir.Path, \"{field.Name}.xml\")");
                                    args.Add($"errorMask: out var {field.Name}ErrorMask");
                                    args.Add($"doMasks: doMasks");
                                }
                                using (var args = new ArgsWrapper(fg,
                                    $"ErrorMask.HandleErrorMask"))
                                {
                                    args.Add($"creator: errMaskFunc");
                                    args.Add($"index: (int){field.IndexEnumName}");
                                    args.Add($"errMaskObj: {field.Name}ErrorMask");
                                }
                                break;
                            case ObjectType.Group:
                                if (!(field is GroupType group)) break;
                                if (!group.TryGetSpecificationAsObject("T", out var subObj)) continue;
                                using (var args = new ArgsWrapper(fg,
                                    $"tasks.Add({field.Name}.Write_XmlFolder<{subObj.Name}, {subObj.Mask(MaskType.Error)}>",
                                    suffixLine: ")"))
                                {
                                    args.Add($"dir: new DirectoryPath(Path.Combine(dir.Path, \"{field.Name}\"))");
                                    args.Add($"errMaskFunc: errMaskFunc");
                                    args.Add($"index: (int){field.IndexEnumName}");
                                    args.Add("doMasks: doMasks");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    fg.AppendLine("await Task.WhenAll(tasks);");
                }
                fg.AppendLine("catch (Exception ex)");
                fg.AppendLine("when (doMasks)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("errMaskFunc().Overall = ex;");
                }
                fg.AppendLine("return errMaskRet;");
            }
        }

        private async Task GenerateForRecord(ObjectGeneration obj, FileGeneration fg)
        {
            if (!obj.IsTopClass) return;
            using (var args = new FunctionWrapper(fg,
                $"public{await obj.FunctionOverride()}void Write_Xml_Folder"))
            {
                args.Add("string path");
                args.Add($"out {obj.Mask(MaskType.Error)} errorMask");
                args.Add($"bool doMasks = true");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    "Write_Xml"))
                {
                    args.Add("path: path");
                    args.Add("errorMask: out errorMask");
                    args.Add("doMasks: doMasks");
                }
            }
        }
    }
}
