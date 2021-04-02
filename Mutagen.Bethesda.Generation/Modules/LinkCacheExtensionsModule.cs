using Loqui;
using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules
{
    public class LinkCacheExtensionsModule : GenerationModule
    {
        public override async Task FinalizeGeneration(ProtocolGeneration proto)
        {
            if (proto.Protocol.Namespace.Equals("All")
                || proto.Protocol.Namespace.Equals("Bethesda")) return;
            FileGeneration fg = new FileGeneration();

            fg.AppendLine("using System.Collections.Generic;");

            fg.AppendLine();
            using (var n = new NamespaceWrapper(fg, proto.DefaultNamespace))
            {
                var setterName = $"I{proto.Protocol.Namespace}Mod";
                var getterName = $"I{proto.Protocol.Namespace}ModGetter";
                var generic = $"<{setterName}, {getterName}>";
                using (var c = new ClassWrapper(fg, "LinkCacheMixIns"))
                {
                    c.Static = true;
                }
                using (new BraceWrapper(fg))
                {
                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a Link Cache using a single mod as its link target. <br/>");
                        comment.Summary.AppendLine($"Modification of the target Mod is not safe.  Internal caches can become incorrect if ");
                        comment.Summary.AppendLine($"modifications occur on content already cached.");
                        comment.Parameters.GetOrAdd("mod").AppendLine("Mod to construct the package relative to");
                        comment.Return.AppendLine($"LinkPackage attached to given mod");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static ImmutableModLinkCache{generic} ToImmutableLinkCache"))
                    {
                        args.Add($"this {getterName} mod");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return mod.ToImmutableLinkCache{generic}();");
                    }
                    fg.AppendLine();

                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but");
                        comment.Summary.AppendLine($"this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to");
                        comment.Summary.AppendLine($"be modified afterwards, use ImmutableModLinkCache instead.<br/>");
                        comment.Parameters.GetOrAdd("mod").AppendLine("Mod to construct the package relative to");
                        comment.Return.AppendLine($"LinkPackage attached to given mod");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static MutableModLinkCache{generic} ToMutableLinkCache"))
                    {
                        args.Add($"this {getterName} mod");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return mod.ToMutableLinkCache{generic}();");
                    }
                    fg.AppendLine();

                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a new linking package relative to a load order.<br/>");
                        comment.Summary.AppendLine($"Will resolve links to the highest overriding mod containing the record being sought. <br/>");
                        comment.Summary.AppendLine($"Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become");
                        comment.Summary.AppendLine($"incorrect if modifications occur on content already cached.");
                        comment.Parameters.GetOrAdd("loadOrder").AppendLine("LoadOrder to construct the package relative to");
                        comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static ImmutableLoadOrderLinkCache{generic} ToImmutableLinkCache"))
                    {
                        args.Add($"this LoadOrder<{getterName}> loadOrder");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return loadOrder.ToImmutableLinkCache{generic}();");
                    }
                    fg.AppendLine();

                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a new linking package relative to a load order.<br/>");
                        comment.Summary.AppendLine($"Will resolve links to the highest overriding mod containing the record being sought. <br/>");
                        comment.Summary.AppendLine($"Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become");
                        comment.Summary.AppendLine($"incorrect if modifications occur on content already cached.");
                        comment.Parameters.GetOrAdd("loadOrder").AppendLine("LoadOrder to construct the package relative to");
                        comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static ImmutableLoadOrderLinkCache{generic} ToImmutableLinkCache"))
                    {
                        args.Add($"this LoadOrder<IModListing<{getterName}>> loadOrder");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return loadOrder.ToImmutableLinkCache{generic}();");
                    }
                    fg.AppendLine();

                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a new linking package relative to a load order.<br/>");
                        comment.Summary.AppendLine($"Will resolve links to the highest overriding mod containing the record being sought. <br/>");
                        comment.Summary.AppendLine($"Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become");
                        comment.Summary.AppendLine($"incorrect if modifications occur on content already cached.");
                        comment.Parameters.GetOrAdd("loadOrder").AppendLine("LoadOrder to construct the package relative to");
                        comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static ImmutableLoadOrderLinkCache{generic} ToImmutableLinkCache"))
                    {
                        args.Add($"this IEnumerable<IModListing<{getterName}>> loadOrder");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return loadOrder.ToImmutableLinkCache{generic}();");
                    }
                    fg.AppendLine();

                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a new linking package relative to a load order.<br/>");
                        comment.Summary.AppendLine($"Will resolve links to the highest overriding mod containing the record being sought. <br/>");
                        comment.Summary.AppendLine($"Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become");
                        comment.Summary.AppendLine($"incorrect if modifications occur on content already cached.");
                        comment.Parameters.GetOrAdd("loadOrder").AppendLine("LoadOrder to construct the package relative to");
                        comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static ImmutableLoadOrderLinkCache{generic} ToImmutableLinkCache"))
                    {
                        args.Add($"this IEnumerable<{getterName}> loadOrder");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return loadOrder.ToImmutableLinkCache{generic}();");
                    }
                    fg.AppendLine();

                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a mutable load order link cache by combining an existing immutable load order cache,");
                        comment.Summary.AppendLine($"plus a set of mods to be put at the end of the load order and allow to be mutable.");
                        comment.Parameters.GetOrAdd("immutableBaseCache").AppendLine("LoadOrderCache to use as the immutable base");
                        comment.Parameters.GetOrAdd("mutableMods").AppendLine("Set of mods to place at the end of the load order, which are allowed to be modified afterwards");
                        comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static MutableLoadOrderLinkCache{generic} ToMutableLinkCache"))
                    {
                        args.Add($"this LoadOrder<{getterName}> immutableBaseCache");
                        args.Add($"params {setterName}[] mutableMods");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return immutableBaseCache.ToMutableLinkCache{generic}(mutableMods);");
                    }
                    fg.AppendLine();

                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a mutable load order link cache by combining an existing immutable load order cache,");
                        comment.Summary.AppendLine($"plus a set of mods to be put at the end of the load order and allow to be mutable.");
                        comment.Parameters.GetOrAdd("immutableBaseCache").AppendLine("LoadOrderCache to use as the immutable base");
                        comment.Parameters.GetOrAdd("mutableMods").AppendLine("Set of mods to place at the end of the load order, which are allowed to be modified afterwards");
                        comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static MutableLoadOrderLinkCache{generic} ToMutableLinkCache"))
                    {
                        args.Add($"this LoadOrder<IModListing<{getterName}>> immutableBaseCache");
                        args.Add($"params {setterName}[] mutableMods");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return immutableBaseCache.ToMutableLinkCache{generic}(mutableMods);");
                    }
                    fg.AppendLine();

                    using (var comment = new CommentWrapper(fg))
                    {
                        comment.Summary.AppendLine($"Creates a mutable load order link cache by combining an existing immutable load order cache,");
                        comment.Summary.AppendLine($"plus a set of mods to be put at the end of the load order and allow to be mutable.");
                        comment.Parameters.GetOrAdd("immutableBaseCache").AppendLine("LoadOrderCache to use as the immutable base");
                        comment.Parameters.GetOrAdd("mutableMods").AppendLine("Set of mods to place at the end of the load order, which are allowed to be modified afterwards");
                        comment.Return.AppendLine($"LinkPackage attached to given LoadOrder");
                    }
                    using (var args = new FunctionWrapper(fg,
                        $"public static MutableLoadOrderLinkCache{generic} ToMutableLinkCache"))
                    {
                        args.Add($"this IEnumerable<{getterName}> immutableBaseCache");
                        args.Add($"params {setterName}[] mutableMods");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"return immutableBaseCache.ToMutableLinkCache{generic}(mutableMods);");
                    }
                    fg.AppendLine();
                }
            }

            var path = Path.Combine(proto.DefFileLocation.FullName, $"LinkCacheMixIns{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
            fg.Generate(path);
            proto.GeneratedFiles.Add(path, ProjItemType.Compile);
        }
    }
}
