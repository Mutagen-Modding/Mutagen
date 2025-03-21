using Loqui.Generation;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Noggog.IO;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class GameCategoryExtensionsModule : GenerationModule
{
    public override async Task FinalizeGeneration(ProtocolGeneration proto)
    {
        await base.FinalizeGeneration(proto);
        if (proto.Protocol.Namespace != "Bethesda") return;

        StructuredStringBuilder sb = new StructuredStringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using Mutagen.Bethesda.Plugins.Records;");
        sb.AppendLine();

        using (sb.Namespace("Mutagen.Bethesda", fileScoped: false))
        {
            using (var cl = sb.Class("GameCategoryHelper"))
            {
                cl.Partial = true;
                cl.Static = true;
            }
            using (sb.CurlyBrace())
            {
                using (var args = sb.Function(
                           $"public static {nameof(GameCategory)} FromModType<TMod>"))
                {
                    args.Wheres.Add($"where TMod : {nameof(IModGetter)}");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("return TryFromModType<TMod>() ?? throw new ArgumentException($\"Unknown game type for: {typeof(TMod).Name}\");");
                }
                sb.AppendLine();
                    
                using (var args = sb.Function(
                           $"public static {nameof(GameCategory)}? TryFromModType<TMod>"))
                {
                    args.Wheres.Add($"where TMod : {nameof(IModGetter)}");
                }
                using (sb.CurlyBrace())
                {
                    sb.AppendLine("switch (typeof(TMod).Name)");
                    using (sb.CurlyBrace())
                    {
                        foreach (var cat in Enums<GameCategory>.Values)
                        {
                            sb.AppendLine($"case \"I{cat}Mod\":");
                            sb.AppendLine($"case \"I{cat}ModGetter\":");
                            using (sb.IncreaseDepth())
                            {
                                sb.AppendLine($"return {nameof(GameCategory)}.{cat};");
                            }
                        }

                        sb.AppendLine("default:");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("return null;");
                        }
                    }
                }
            }
        }

        var path = Path.Combine(proto.DefFileLocation.FullName, "../Extensions", $"GameCategoryHelper{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        ExportStringToFile exportStringToFile = new(IFileSystemExt.DefaultFilesystem);
        exportStringToFile.ExportToFile(path, sb.GetString());
        proto.GeneratedFiles.Add(path, ProjItemType.Compile);
    }
}