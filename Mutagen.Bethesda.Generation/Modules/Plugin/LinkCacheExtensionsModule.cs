using Loqui.Generation;
using Noggog;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class LinkCacheExtensionsModule : GenerationModule
{
    public override async Task FinalizeGeneration(ProtocolGeneration proto)
    {
        if (proto.Protocol.Namespace.Equals("All")
            || proto.Protocol.Namespace.Equals("Bethesda")) return;
        StructuredStringBuilder sb = new StructuredStringBuilder();

        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Order;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;");

        sb.AppendLine();
        using (sb.Namespace(proto.DefaultNamespace, fileScoped: false))
        {
            var setterName = $"I{proto.Protocol.Namespace}Mod";
            var getterName = $"I{proto.Protocol.Namespace}ModGetter";
            var generic = $"<{setterName}, {getterName}>";
            using (var c = sb.Class("LinkCacheMixIns"))
            {
                c.Static = true;
            }
            using (sb.CurlyBrace())
            {
                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a Link Cache using a single mod as its link target. <br/>");
                    comment.Summary.AppendLine($"Modification of the target Mod is not safe.  Internal caches can become incorrect if ");
                    comment.Summary.AppendLine($"modifications occur on content already cached.");
                    comment.Parameters.GetOrAdd("mod").AppendLine("Mod to construct the package relative to");
                    comment.Return.AppendLine($"LinkPackage attached to given mod");
                }
                using (var args = sb.Function(
                           $"public static ImmutableModLinkCache{generic} ToImmutableLinkCache"))
                {
                    args.Add($"this {getterName} mod");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return mod.ToImmutableLinkCache{generic}();");
                }
                sb.AppendLine();

                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but");
                    comment.Summary.AppendLine($"this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to");
                    comment.Summary.AppendLine($"be modified afterwards, use ImmutableModLinkCache instead.<br/>");
                    comment.Parameters.GetOrAdd("mod").AppendLine("Mod to construct the package relative to");
                    comment.Return.AppendLine($"LinkPackage attached to given mod");
                }
                using (var args = sb.Function(
                           $"public static MutableModLinkCache{generic} ToMutableLinkCache"))
                {
                    args.Add($"this {getterName} mod");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return mod.ToMutableLinkCache{generic}();");
                }
                sb.AppendLine();

                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a new linking package relative to a load order.<br/>");
                    comment.Summary.AppendLine($"Will resolve links to the highest overriding mod containing the record being sought. <br/>");
                    comment.Summary.AppendLine($"Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become");
                    comment.Summary.AppendLine($"incorrect if modifications occur on content already cached.");
                    comment.Parameters.GetOrAdd("loadOrder").AppendLine("LoadOrder to construct the package relative to");
                    comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                }
                using (var args = sb.Function(
                           $"public static ImmutableLoadOrderLinkCache{generic} ToImmutableLinkCache"))
                {
                    args.Add($"this ILoadOrderGetter<{getterName}> loadOrder");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return loadOrder.ToImmutableLinkCache{generic}();");
                }
                sb.AppendLine();

                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a new linking package relative to a load order.<br/>");
                    comment.Summary.AppendLine($"Will resolve links to the highest overriding mod containing the record being sought. <br/>");
                    comment.Summary.AppendLine($"Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become");
                    comment.Summary.AppendLine($"incorrect if modifications occur on content already cached.");
                    comment.Parameters.GetOrAdd("loadOrder").AppendLine("LoadOrder to construct the package relative to");
                    comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                }
                using (var args = sb.Function(
                           $"public static ImmutableLoadOrderLinkCache{generic} ToImmutableLinkCache"))
                {
                    args.Add($"this ILoadOrderGetter<IModListingGetter<{getterName}>> loadOrder");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return loadOrder.ToImmutableLinkCache{generic}();");
                }
                sb.AppendLine();

                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a new linking package relative to a load order.<br/>");
                    comment.Summary.AppendLine($"Will resolve links to the highest overriding mod containing the record being sought. <br/>");
                    comment.Summary.AppendLine($"Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become");
                    comment.Summary.AppendLine($"incorrect if modifications occur on content already cached.");
                    comment.Parameters.GetOrAdd("loadOrder").AppendLine("LoadOrder to construct the package relative to");
                    comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                }
                using (var args = sb.Function(
                           $"public static ImmutableLoadOrderLinkCache{generic} ToImmutableLinkCache"))
                {
                    args.Add($"this IEnumerable<IModListingGetter<{getterName}>> loadOrder");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return loadOrder.ToImmutableLinkCache{generic}();");
                }
                sb.AppendLine();

                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a new linking package relative to a load order.<br/>");
                    comment.Summary.AppendLine($"Will resolve links to the highest overriding mod containing the record being sought. <br/>");
                    comment.Summary.AppendLine($"Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become");
                    comment.Summary.AppendLine($"incorrect if modifications occur on content already cached.");
                    comment.Parameters.GetOrAdd("loadOrder").AppendLine("LoadOrder to construct the package relative to");
                    comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                }
                using (var args = sb.Function(
                           $"public static ImmutableLoadOrderLinkCache{generic} ToImmutableLinkCache"))
                {
                    args.Add($"this IEnumerable<{getterName}> loadOrder");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return loadOrder.ToImmutableLinkCache{generic}();");
                }
                sb.AppendLine();

                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a mutable load order link cache by combining an existing immutable load order cache,");
                    comment.Summary.AppendLine($"plus a set of mods to be put at the end of the load order and allow to be mutable.");
                    comment.Parameters.GetOrAdd("immutableBaseCache").AppendLine("LoadOrderCache to use as the immutable base");
                    comment.Parameters.GetOrAdd("mutableMods").AppendLine("Set of mods to place at the end of the load order, which are allowed to be modified afterwards");
                    comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                }
                using (var args = sb.Function(
                           $"public static MutableLoadOrderLinkCache{generic} ToMutableLinkCache"))
                {
                    args.Add($"this ILoadOrderGetter<{getterName}> immutableBaseCache");
                    args.Add($"params {setterName}[] mutableMods");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return immutableBaseCache.ToMutableLinkCache{generic}(mutableMods);");
                }
                sb.AppendLine();

                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a mutable load order link cache by combining an existing immutable load order cache,");
                    comment.Summary.AppendLine($"plus a set of mods to be put at the end of the load order and allow to be mutable.");
                    comment.Parameters.GetOrAdd("immutableBaseCache").AppendLine("LoadOrderCache to use as the immutable base");
                    comment.Parameters.GetOrAdd("mutableMods").AppendLine("Set of mods to place at the end of the load order, which are allowed to be modified afterwards");
                    comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                }
                using (var args = sb.Function(
                           $"public static MutableLoadOrderLinkCache{generic} ToMutableLinkCache"))
                {
                    args.Add($"this ILoadOrderGetter<IModListingGetter<{getterName}>> immutableBaseCache");
                    args.Add($"params {setterName}[] mutableMods");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return immutableBaseCache.ToMutableLinkCache{generic}(mutableMods);");
                }
                sb.AppendLine();

                using (var comment = sb.Comment())
                {
                    comment.Summary.AppendLine($"Creates a mutable load order link cache by combining an existing immutable load order cache,");
                    comment.Summary.AppendLine($"plus a set of mods to be put at the end of the load order and allow to be mutable.");
                    comment.Parameters.GetOrAdd("immutableBaseCache").AppendLine("LoadOrderCache to use as the immutable base");
                    comment.Parameters.GetOrAdd("mutableMods").AppendLine("Set of mods to place at the end of the load order, which are allowed to be modified afterwards");
                    comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                }
                using (var args = sb.Function(
                           $"public static MutableLoadOrderLinkCache{generic} ToMutableLinkCache"))
                {
                    args.Add($"this IEnumerable<{getterName}> immutableBaseCache");
                    args.Add($"params {setterName}[] mutableMods");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine($"return immutableBaseCache.ToMutableLinkCache{generic}(mutableMods);");
                }
                sb.AppendLine();
            }
        }

        var path = Path.Combine(proto.DefFileLocation.FullName, $"LinkCacheMixIns{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        sb.Generate(path);
        proto.GeneratedFiles.Add(path, ProjItemType.Compile);
    }
}