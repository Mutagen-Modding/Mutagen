using Loqui;
using Mutagen.Bethesda.Oblivion;
using Noggog;
using System;
using System.Collections.Generic;
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

        public static bool TryCreateLoadOrder(string path, out List<ModKey> modList)
        {
            modList = new List<ModKey>();
            foreach (var item in File.ReadAllLines(path))
            {
                var str = item;
                var commentIndex = str.IndexOf('#');
                if (commentIndex != -1)
                {
                    str = str.Substring(0, commentIndex);
                }
                if (string.IsNullOrWhiteSpace(str)) continue;
                str = str.Trim();
                if (!ModKey.TryFactory(str, out var key))
                {
                    return false;
                }
                modList.Add(key);
            }
            return true;
        }

        public static bool TryGetUsualLoadOrder(out List<ModKey> loadOrder)
        {
            if (!TryGetPluginsFile(out var path))
            {
                loadOrder = null;
                return false;
            }
            return TryCreateLoadOrder(path.Path, out loadOrder);
        }
    }
}
