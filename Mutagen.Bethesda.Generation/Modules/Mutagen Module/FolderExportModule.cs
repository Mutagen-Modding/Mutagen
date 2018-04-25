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
        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            if (obj.GetObjectType() != ObjectType.Mod) return;
            using (var args = new FunctionWrapper(fg,
                $"public void Write_XmlFolder"))
            {
                args.Add("DirectoryPath dir");
                args.Add($"out {obj.Mask(MaskType.Error)} errorMask");
                args.Add("bool doMasks = true");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"{obj.Mask(MaskType.Error)} errMaskRet = null;");
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
                                    $"this.{field.Name}.Write_XML"))
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
                                //fg.AppendLine($"var {field.Name}Dir = new DirectoryPath(Path.Combine(dir.Path, \"{field.Name}\"));");
                                //fg.AppendLine($"{field.Name}Dir.Create();");
                                //fg.AppendLine($"int {field.Name}Counter = 0;");
                                //fg.AppendLine($"foreach (var item in this.{field.Name}.Items.Values)");
                                //using (new BraceWrapper(fg))
                                //{
                                //    using (var args = new ArgsWrapper(fg,
                                //        $"item.Write_XML"))
                                //    {
                                //        args.Add($"path: Path.Combine({field.Name}Dir.Path, $\"{{{field.Name}Counter++}} - {{item.FormID.IDString()}} - {{item.EditorID}}.xml\")");
                                //        args.Add($"errorMask: out var {field.Name}ErrorMask");
                                //        args.Add($"doMasks: doMasks");
                                //    }
                                //    using (var args = new ArgsWrapper(fg,
                                //        $"ErrorMask.HandleErrorMaskAddition"))
                                //    {
                                //        args.Add($"creator: errMaskFunc");
                                //        args.Add($"index: (int){field.IndexEnumName}");
                                //        args.Add($"errMaskObj: {field.Name}ErrorMask");
                                //    }
                                //}
                                if (!group.TryGetSpecificationAsObject("T", out var subObj)) continue;
                                using (var args = new ArgsWrapper(fg,
                                    $"{field.Name}.Write_XmlFolder<{subObj.Mask(MaskType.Error)}>"))
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
                }
                fg.AppendLine("catch (Exception ex)");
                fg.AppendLine("when (doMasks)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("errMaskFunc().Overall = ex;");
                }
                fg.AppendLine("errorMask = errMaskRet;");
            }
        }
    }
}
