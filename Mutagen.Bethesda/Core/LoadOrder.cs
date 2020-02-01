using Loqui;
using Mutagen.Bethesda.Oblivion;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public class LoadOrder
    {
        public static bool TryGetPluginsFile(out FilePath path)
        {
            path = new FilePath(
                Path.Combine(
                    Environment.GetEnvironmentVariable("LocalAppData"),
                    "Oblivion/Plugins.txt"));
            return path.Exists;
        }

        public static bool TryCreateLoadOrder(
            FilePath pluginListPath,
            DirectoryPath dataPath,
            out List<ModKey> modList)
        {
            List<(ModKey ModKey, DateTime Write)> list = new List<(ModKey ModKey, DateTime Write)>();
            modList = new List<ModKey>();
            foreach (var item in File.ReadAllLines(pluginListPath.Path))
            {
                var str = item;
                var commentIndex = str.IndexOf('#');
                if (commentIndex != -1)
                {
                    str = str.Substring(0, commentIndex);
                }
                if (string.IsNullOrWhiteSpace(str)) continue;
                str = str.Trim();
                if (!ModKey.TryFactory(str, out var key)) return false;
                FilePath file = new FilePath(
                    Path.Combine(dataPath.Path, str));
                if (!file.Exists) return false;
                list.Add((key, file.Info.LastWriteTime));
            }
            modList.AddRange(list
                .OrderBy(i => i.Write)
                .Select(i => i.ModKey));
            return true;
        }

        public static bool TryGetUsualLoadOrder(DirectoryPath dataPath, [MaybeNullWhen(false)]out List<ModKey> loadOrder)
        {
            if (!TryGetPluginsFile(out var path))
            {
                loadOrder = default!;
                return false;
            }
            return TryCreateLoadOrder(path, dataPath, out loadOrder);
        }
    }
}
