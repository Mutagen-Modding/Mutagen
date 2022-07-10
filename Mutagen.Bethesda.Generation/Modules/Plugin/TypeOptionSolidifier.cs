using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Noggog;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog.IO;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;
using ObjectType = Mutagen.Bethesda.Plugins.Meta.ObjectType;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

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
        StructuredStringBuilder sb = new StructuredStringBuilder();

        var modObj = proto.ObjectGenerationsByID.Values.FirstOrDefault(o => o.GetObjectType() == ObjectType.Mod);

        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Cache;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Order;");
        sb.AppendLine();
        using (var n = sb.Namespace(proto.DefaultNamespace, fileScoped: false))
        {
            using (var c = sb.Class("TypeOptionSolidifierMixIns"))
            {
                c.Static = true;
            }
            using (sb.CurlyBrace())
            {
                using (sb.Region("Normal"))
                {
                    foreach (var obj in proto.ObjectGenerationsByName
                                 .OrderBy(x => x.Key)
                                 .Select(x => x.Value))
                    {
                        if (!await obj.IsMajorRecord()) continue;
                        if (!obj.BaseClass.Name.EndsWith("MajorRecord")) continue;

                        var topLevel = modObj.Fields.Any(x =>
                        {
                            if (x is not GroupType grup) return false;
                            var grupTarget = grup.GetGroupTarget();
                            if (grupTarget == obj) return true;
                            return obj.BaseClassTrail().Any(b => b == grupTarget);
                        });
                        var topLevelStr = topLevel ? "TopLevel" : string.Empty;

                        using (var comment = sb.Comment())
                        {
                            comment.Summary.AppendLine($"Scope a load order query to {obj.Name}");
                            comment.Parameters.GetOrAdd("listings").AppendLine("ModListings to query");
                            comment.Return.AppendLine($"A typed object to do further queries on {obj.Name}");
                        }
                        using (var args = sb.Function(
                                   $"public static {topLevelStr}TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}> {obj.Name}"))
                        {
                            args.Add($"this IEnumerable<IModListingGetter<I{proto.Protocol.Namespace}ModGetter>> listings");
                        }
                        using (sb.CurlyBrace())
                        {
                            using (var args = sb.Call(
                                       $"return new {topLevelStr}TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>"))
                            {
                                args.Add($"(bool includeDeletedRecords) => listings.WinningOverrides<{obj.Interface(getter: true)}>(includeDeletedRecords: includeDeletedRecords)");
                                args.Add($"({nameof(ILinkCache)} linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>(linkCache, includeDeletedRecords: includeDeletedRecords)");
                            }
                        }
                        sb.AppendLine();

                        using (var comment = sb.Comment())
                        {
                            comment.Summary.AppendLine($"Scope a load order query to {obj.Name}");
                            comment.Parameters.GetOrAdd("mods").AppendLine("Mods to query");
                            comment.Return.AppendLine($"A typed object to do further queries on {obj.Name}");
                        }
                        using (var args = sb.Function(
                                   $"public static {topLevelStr}TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}> {obj.Name}"))
                        {
                            args.Add($"this IEnumerable<I{proto.Protocol.Namespace}ModGetter> mods");
                        }
                        using (sb.CurlyBrace())
                        {
                            using (var args = sb.Call(
                                       $"return new {topLevelStr}TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>"))
                            {
                                args.Add($"(bool includeDeletedRecords) => mods.WinningOverrides<{obj.Interface(getter: true)}>(includeDeletedRecords: includeDeletedRecords)");
                                args.Add($"({nameof(ILinkCache)} linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {obj.Interface(getter: false)}, {obj.Interface(getter: true)}>(linkCache, includeDeletedRecords: includeDeletedRecords)");
                            }
                        }
                        sb.AppendLine();
                        generate = true;
                    }
                }

                using (sb.Region("Link Interfaces"))
                {
                    if (LinkInterfaceModule.ObjectMappings.TryGetValue(proto.Protocol, out var interfs))
                    {
                        foreach (var interf in interfs)
                        {
                            var getter = $"{interf.Key}Getter";
                            using (var comment = sb.Comment())
                            {
                                comment.Summary.AppendLine($"Scope a load order query to {interf.Key}");
                                comment.Parameters.GetOrAdd("listings").AppendLine("ModListings to query");
                                comment.Return.AppendLine($"A typed object to do further queries on {interf.Key}");
                            }
                            using (var args = sb.Function(
                                       $"public static TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}> {interf.Key}"))
                            {
                                args.Add($"this IEnumerable<IModListingGetter<I{proto.Protocol.Namespace}ModGetter>> listings");
                            }
                            using (sb.CurlyBrace())
                            {
                                using (var args = sb.Call(
                                           $"return new TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}>"))
                                {
                                    args.Add($"(bool includeDeletedRecords) => listings.WinningOverrides<{getter}>(includeDeletedRecords: includeDeletedRecords)");
                                    args.Add($"({nameof(ILinkCache)} linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}>(linkCache, includeDeletedRecords: includeDeletedRecords)");
                                }
                            }
                            sb.AppendLine();

                            using (var comment = sb.Comment())
                            {
                                comment.Summary.AppendLine($"Scope a load order query to {interf.Key}");
                                comment.Parameters.GetOrAdd("mods").AppendLine("Mods to query");
                                comment.Return.AppendLine($"A typed object to do further queries on {interf.Key}");
                            }
                            using (var args = sb.Function(
                                       $"public static TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}> {interf.Key}"))
                            {
                                args.Add($"this IEnumerable<I{proto.Protocol.Namespace}ModGetter> mods");
                            }
                            using (sb.CurlyBrace())
                            {
                                using (var args = sb.Call(
                                           $"return new TypedLoadOrderAccess<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}>"))
                                {
                                    args.Add($"(bool includeDeletedRecords) => mods.WinningOverrides<{getter}>(includeDeletedRecords: includeDeletedRecords)");
                                    args.Add($"({nameof(ILinkCache)} linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<I{proto.Protocol.Namespace}Mod, I{proto.Protocol.Namespace}ModGetter, {interf.Key}, {getter}>(linkCache, includeDeletedRecords: includeDeletedRecords)");
                                }
                            }
                            sb.AppendLine();
                        }
                    }
                }
            }
        }

        if (!generate) return;
        var path = Path.Combine(proto.DefFileLocation.FullName, $"TypeSolidifier{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        ExportStringToFile exportStringToFile = new();
        exportStringToFile.ExportToFile(path, sb.GetString());
        proto.GeneratedFiles.Add(path, ProjItemType.Compile);
    }
}