using Loqui;
using Loqui.Generation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Noggog;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda.Generation.Modules
{
    /// <summary>
    /// One of the harder parts to offer via natural intellisense is informing all the viable options
    /// a IMajorRecordGetter generic might allow, for example.
    /// 
    /// This module is in charge of generating extension methods and other tricks to help the compiler 
    /// help the user know the options available to them.
    /// </summary>
    public class TypeOptionSolidifier : GenerationModule
    {
        public override async Task FinalizeGeneration(ProtocolGeneration proto)
        {
            if (proto.Protocol.Namespace.Equals("Bethesda")) return;
            bool generate = false;
            FileGeneration fg = new FileGeneration();

            var modObj = proto.ObjectGenerationsByID.Values.FirstOrDefault(o => o.GetObjectType() == ObjectType.Mod);

            fg.AppendLine("using System.Collections.Generic;");
            fg.AppendLine("using Mutagen.Bethesda.Plugins.Cache;");
            fg.AppendLine("using Mutagen.Bethesda.Plugins.Order;");
            fg.AppendLine("using Mutagen.Bethesda.Plugins.Order.Internals;");
            fg.AppendLine();
            using (var n = new NamespaceWrapper(fg, proto.DefaultNamespace))
            {
                using (var c = new ClassWrapper(fg, "TypeOptionSolidifierMixIns"))
                {
                    c.Static = true;
                }
                using (new BraceWrapper(fg))
                {
                    using (new RegionWrapper(fg, "Normal"))
                    {
                        foreach (var obj in proto.ObjectGenerationsByName
                            .OrderBy(x => x.Key)
                            .Select(x => x.Value))
                        {
                            if (!await obj.IsMajorRecord()) continue;

                            var topLevel = modObj.Fields.Any(x =>
                            {
                                if (x is not GroupType grup) return false;
                                var grupTarget = grup.GetGroupTarget();
                                if (grupTarget == obj) return true;
                                return obj.BaseClassTrail().Any(b => b == grupTarget);
                            });
                            var topLevelStr = topLevel ? "TopLevel" : string.Empty;

                            using (var comment = new CommentWrapper(fg))
                            {
                                comment.Summary.AppendLine($"Scope a load order query to {obj.Name}");
                                comment.Parameters.GetOrAdd("listings").AppendLine("ModListings to query");
                                comment.Return.AppendLine($"A typed object to do further queries on {obj.Name}");
                            }
                            using (var args = new FunctionWrapper(fg,
                                $"public static {topLevelStr}TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}> {obj.Name}"))
                            {
                                args.Add($"this IEnumerable<IModListing<I{proto.Protocol.Namespace}ModGetter>> listings");
                            }
                            using (new BraceWrapper(fg))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return new {topLevelStr}TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>"))
                                {
                                    args.Add($"(bool includeDeletedRecords) => listings.WinningOverrides<{obj.Interface(getter: true)}>(includeDeletedRecords: includeDeletedRecords)");
                                    args.Add($"({nameof(ILinkCache)} linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>(linkCache, includeDeletedRecords: includeDeletedRecords)");
                                }
                            }
                            fg.AppendLine();

                            using (var comment = new CommentWrapper(fg))
                            {
                                comment.Summary.AppendLine($"Scope a load order query to {obj.Name}");
                                comment.Parameters.GetOrAdd("mods").AppendLine("Mods to query");
                                comment.Return.AppendLine($"A typed object to do further queries on {obj.Name}");
                            }
                            using (var args = new FunctionWrapper(fg,
                                $"public static {topLevelStr}TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}> {obj.Name}"))
                            {
                                args.Add($"this IEnumerable<I{proto.Protocol.Namespace}ModGetter> mods");
                            }
                            using (new BraceWrapper(fg))
                            {
                                using (var args = new ArgsWrapper(fg,
                                    $"return new {topLevelStr}TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>"))
                                {
                                    args.Add($"(bool includeDeletedRecords) => mods.WinningOverrides<{obj.Interface(getter: true)}>(includeDeletedRecords: includeDeletedRecords)");
                                    args.Add($"({nameof(ILinkCache)} linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>(linkCache, includeDeletedRecords: includeDeletedRecords)");
                                }
                            }
                            fg.AppendLine();
                            generate = true;
                        }
                    }

                    using (new RegionWrapper(fg, "Link Interfaces"))
                    {
                        if (LinkInterfaceModule.ObjectMappings.TryGetValue(proto.Protocol, out var interfs))
                        {
                            foreach (var interf in interfs)
                            {
                                var getter = $"{interf.Key}Getter";
                                using (var comment = new CommentWrapper(fg))
                                {
                                    comment.Summary.AppendLine($"Scope a load order query to {interf.Key}");
                                    comment.Parameters.GetOrAdd("listings").AppendLine("ModListings to query");
                                    comment.Return.AppendLine($"A typed object to do further queries on {interf.Key}");
                                }
                                using (var args = new FunctionWrapper(fg,
                                    $"public static TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}> {interf.Key}"))
                                {
                                    args.Add($"this IEnumerable<IModListing<I{proto.Protocol.Namespace}ModGetter>> listings");
                                }
                                using (new BraceWrapper(fg))
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        $"return new TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}>"))
                                    {
                                        args.Add($"(bool includeDeletedRecords) => listings.WinningOverrides<{getter}>(includeDeletedRecords: includeDeletedRecords)");
                                        args.Add($"({nameof(ILinkCache)} linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}>(linkCache, includeDeletedRecords: includeDeletedRecords)");
                                    }
                                }
                                fg.AppendLine();

                                using (var comment = new CommentWrapper(fg))
                                {
                                    comment.Summary.AppendLine($"Scope a load order query to {interf.Key}");
                                    comment.Parameters.GetOrAdd("mods").AppendLine("Mods to query");
                                    comment.Return.AppendLine($"A typed object to do further queries on {interf.Key}");
                                }
                                using (var args = new FunctionWrapper(fg,
                                    $"public static TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}> {interf.Key}"))
                                {
                                    args.Add($"this IEnumerable<I{proto.Protocol.Namespace}ModGetter> mods");
                                }
                                using (new BraceWrapper(fg))
                                {
                                    using (var args = new ArgsWrapper(fg,
                                        $"return new TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}>"))
                                    {
                                        args.Add($"(bool includeDeletedRecords) => mods.WinningOverrides<{getter}>(includeDeletedRecords: includeDeletedRecords)");
                                        args.Add($"({nameof(ILinkCache)} linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}>(linkCache, includeDeletedRecords: includeDeletedRecords)");
                                    }
                                }
                                fg.AppendLine();
                            }
                        }
                    }
                }
            }

            if (!generate) return;
            var path = Path.Combine(proto.DefFileLocation.FullName, $"TypeSolidifier{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
            fg.Generate(path);
            proto.GeneratedFiles.Add(path, ProjItemType.Compile);
        }
    }
}
