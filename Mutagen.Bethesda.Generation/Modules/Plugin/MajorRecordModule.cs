using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.RecordTypeMapping;

namespace Mutagen.Bethesda.Generation.Modules.Plugin
{
    public class MajorRecordModule : GenerationModule
    {
        public override async Task LoadWrapup(ObjectGeneration obj)
        {
            await base.LoadWrapup(obj);
            if (!await obj.IsMajorRecord()) return;
            obj.BasicCtorPermission = CtorPermissionLevel.@protected;
            if (!obj.Abstract)
            {
                obj.Attributes.Add($"[{nameof(AssociatedRecordTypesAttribute)}(Mutagen.Bethesda.{obj.GetObjectData().GameCategory}.Internals.{nameof(RecordTypeInts)}.{obj.GetObjectData().RecordType})]", LoquiInterfaceType.IGetter);
            }
        }

        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInClass(obj, fg);
            if (!await obj.IsMajorRecord()) return;
            using (var args = new FunctionWrapper(fg,
                $"public {obj.Name}"))
            {
                args.Add($"{nameof(FormKey)} formKey");
                if (obj.GetObjectData().HasMultipleReleases)
                {
                    args.Add($"{obj.GetObjectData().GameCategory}Release gameRelease");
                }
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("this.FormKey = formKey;");
                if (obj.GetObjectData().HasMultipleReleases)
                {
                    fg.AppendLine("this.FormVersion = gameRelease.ToGameRelease().GetDefaultFormVersion()!.Value;");
                }
                fg.AppendLine("CustomCtor();");
            }
            fg.AppendLine();

            // Used for reflection based construction
            using (var args = new FunctionWrapper(fg,
                $"private {obj.Name}"))
            {
                args.Add($"{nameof(FormKey)} formKey");
                args.Add($"{nameof(GameRelease)} gameRelease");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("this.FormKey = formKey;");
                if (obj.GetObjectData().GameCategory?.HasFormVersion() ?? false)
                {
                    fg.AppendLine("this.FormVersion = gameRelease.GetDefaultFormVersion()!.Value;");
                }
                fg.AppendLine("CustomCtor();");
            }
            fg.AppendLine();

            if (obj.GetObjectData().GameCategory?.HasFormVersion() ?? false)
            {
                using (var args = new FunctionWrapper(fg,
                    $"internal {obj.Name}"))
                {
                    args.Add($"{nameof(FormKey)} formKey");
                    args.Add($"ushort formVersion");
                }
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine("this.FormKey = formKey;");
                    fg.AppendLine("this.FormVersion = formVersion;");
                    fg.AppendLine("CustomCtor();");
                }
                fg.AppendLine();
            }

            fg.AppendLine($"public {obj.Name}(I{obj.GetObjectData().GameCategory}Mod mod)");
            using (new DepthWrapper(fg))
            {
                using (var args = new FunctionWrapper(fg, ": this"))
                {
                    args.Add($"mod.{nameof(IMod.GetNextFormKey)}()");
                    if (obj.GetObjectData().HasMultipleReleases)
                    {
                        args.Add($"mod.{obj.GetObjectData().GameCategory}Release");
                    }
                }
            }
            using (new BraceWrapper(fg))
            {
            }
            fg.AppendLine();

            fg.AppendLine($"public {obj.Name}(I{obj.GetObjectData().GameCategory}Mod mod, string editorID)");
            using (new DepthWrapper(fg))
            {
                using (var args = new FunctionWrapper(fg, ": this"))
                {
                    args.Add($"mod.{nameof(IMod.GetNextFormKey)}(editorID)");
                    if (obj.GetObjectData().HasMultipleReleases)
                    {
                        args.Add($"mod.{obj.GetObjectData().GameCategory}Release");
                    }
                }
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("this.EditorID = editorID;");
            }
            fg.AppendLine();

            fg.AppendLine($"public override string ToString()");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"return MajorRecordPrinter<{obj.Name}>.ToString(this);");
            }
            fg.AppendLine();
        }

        public static async Task<Case> HasMajorRecordsInTree(ObjectGeneration obj, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (await HasMajorRecords(obj, includeBaseClass: includeBaseClass, includeSelf: false, specifications: specifications) == Case.Yes) return Case.Yes;
            // If no, check subclasses  
            foreach (var inheritingObject in await obj.InheritingObjects())
            {
                if (await HasMajorRecordsInTree(inheritingObject, includeBaseClass: false, specifications: specifications) == Case.Yes) return Case.Yes;
            }

            return Case.No;
        }

        public static async Task<Case> HasMajorRecords(LoquiType loqui, bool includeBaseClass, GenericSpecification specifications = null, bool includeSelf = true)
        {
            if (loqui.TargetObjectGeneration != null)
            {
                if (includeSelf && await loqui.TargetObjectGeneration.IsMajorRecord()) return Case.Yes;
                return await MajorRecordModule.HasMajorRecordsInTree(loqui.TargetObjectGeneration, includeBaseClass, loqui.GenericSpecification);
            }
            else if (specifications != null)
            {
                foreach (var target in specifications.Specifications.Values)
                {
                    if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                    var specObj = loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[key];
                    if (await specObj.IsMajorRecord()) return Case.Yes;
                    return await MajorRecordModule.HasMajorRecordsInTree(specObj, includeBaseClass);
                }
            }
            else if (loqui.RefType == LoquiType.LoquiRefType.Interface)
            {
                // ToDo  
                // Quick hack.  Real solution should use reflection to investigate the interface  
                return includeSelf ? Case.Yes : Case.No;
            }
            return Case.Maybe;
        }

        public static async Task<Case> HasMajorRecords(ObjectGeneration obj, bool includeBaseClass, bool includeSelf, GenericSpecification specifications = null)
        {
            if (obj.Name.EndsWith("ListGroup")) return Case.Yes;
            foreach (var field in obj.IterateFields(includeBaseClass: includeBaseClass))
            {
                if (field is LoquiType loqui)
                {
                    if (includeSelf
                        && loqui.TargetObjectGeneration != null
                        && await loqui.TargetObjectGeneration.IsMajorRecord())
                    {
                        return Case.Yes;
                    }
                    if (await HasMajorRecords(loqui, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
                }
                else if (field is ContainerType cont)
                {
                    if (cont.SubTypeGeneration is LoquiType contLoqui)
                    {
                        if (await HasMajorRecords(contLoqui, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType valLoqui)
                    {
                        if (await HasMajorRecords(valLoqui, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
                    }
                    if (dict.KeyTypeGen is LoquiType keyLoqui)
                    {
                        if (await HasMajorRecords(keyLoqui, includeBaseClass, specifications) == Case.Yes) return Case.Yes;
                    }
                }
            }
            return Case.No;
        }

        public static async IAsyncEnumerable<ObjectGeneration> IterateMajorRecords(LoquiType loqui, bool includeBaseClass, GenericSpecification specifications = null)
        {
            if (specifications?.Specifications.Count > 0)
            {
                foreach (var target in specifications.Specifications.Values)
                {
                    if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                    var specObj = loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[key];
                    if (await specObj.IsMajorRecord()) yield return specObj;
                    await foreach (var item in IterateMajorRecords(specObj, includeBaseClass, includeSelf: true, loqui.GenericSpecification))
                    {
                        yield return item;
                    }
                }
            }
            else if (loqui.TargetObjectGeneration != null)
            {
                if (await loqui.TargetObjectGeneration.IsMajorRecord()) yield return loqui.TargetObjectGeneration;
                await foreach (var item in IterateMajorRecords(loqui.TargetObjectGeneration, includeBaseClass, includeSelf: true, loqui.GenericSpecification))
                {
                    yield return item;
                }
            }
            else if (loqui.RefType == LoquiType.LoquiRefType.Interface)
            {
                // Must be a link interface 
                if (!LinkInterfaceModule.ObjectMappings[loqui.ObjectGen.ProtoGen.Protocol].TryGetValue(loqui.SetterInterface, out var mappings))
                {
                    throw new ArgumentException();
                }
                foreach (var obj in mappings)
                {
                    yield return obj;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static async IAsyncEnumerable<ObjectGeneration> IterateMajorRecords(ObjectGeneration obj, bool includeBaseClass, bool includeSelf, GenericSpecification specifications = null)
        {
            foreach (var field in obj.IterateFields(includeBaseClass: includeBaseClass))
            {
                if (field is LoquiType loqui)
                {
                    if (includeSelf
                        && loqui.TargetObjectGeneration != null
                        && await loqui.TargetObjectGeneration.IsMajorRecord())
                    {
                        yield return loqui.TargetObjectGeneration;
                    }
                    await foreach (var item in IterateMajorRecords(loqui, includeBaseClass, specifications))
                    {
                        yield return item;
                    }
                }
                else if (field is ContainerType cont)
                {
                    if (cont.SubTypeGeneration is LoquiType contLoqui)
                    {
                        await foreach (var item in IterateMajorRecords(contLoqui, includeBaseClass, specifications))
                        {
                            yield return item;
                        }
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType valLoqui)
                    {
                        await foreach (var item in IterateMajorRecords(valLoqui, includeBaseClass, specifications))
                        {
                            yield return item;
                        }
                    }
                    if (dict.KeyTypeGen is LoquiType keyLoqui)
                    {
                        await foreach (var item in IterateMajorRecords(keyLoqui, includeBaseClass, specifications))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        public static async Task<Dictionary<ObjectGeneration, HashSet<TypeGeneration>>> FindDeepRecords(ObjectGeneration obj)
        {
            var deepRecordMapping = new Dictionary<ObjectGeneration, HashSet<TypeGeneration>>();
            foreach (var field in obj.IterateFields())
            {
                if (field is LoquiType loqui)
                {
                    var groupType = field as GroupType;
                    await foreach (var deepObj in MajorRecordModule.IterateMajorRecords(loqui, includeBaseClass: true))
                    {
                        if (groupType != null
                            && groupType.GetGroupTarget() == deepObj)
                        {
                            continue;
                        }
                        if (loqui.TargetObjectGeneration == deepObj) continue;
                        deepRecordMapping.GetOrAdd(deepObj).Add(field);
                    }
                }
                else if (field is ContainerType cont)
                {
                    if (!(cont.SubTypeGeneration is LoquiType subLoqui)) continue;
                    await foreach (var deepObj in MajorRecordModule.IterateMajorRecords(subLoqui, includeBaseClass: true))
                    {
                        if (subLoqui.TargetObjectGeneration == deepObj) continue;
                        deepRecordMapping.GetOrAdd(deepObj).Add(field);
                    }
                }
            }
            return deepRecordMapping;
        }

        public override async IAsyncEnumerable<string> RequiredUsingStatements(ObjectGeneration obj)
        {
            if (await obj.IsMajorRecord())
            {
                yield return "Mutagen.Bethesda.Plugins.Utility";
                yield return "Mutagen.Bethesda.Plugins.RecordTypeMapping";
            }
        }
    }
}
