using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.IO;

namespace Mutagen.Bethesda
{
    public class ModPath : IEquatable<ModPath>
    {
        public readonly ModKey ModKey;
        public readonly string Path;
        public static readonly ModPath Empty = new ModPath(ModKey.Null, string.Empty);

        public ModPath(ModKey modKey, string path)
        {
            ModKey = modKey;
            Path = path;
        }

        public static ModPath FromPath(string path)
        {
            var modKey = ModKey.FromNameAndExtension(System.IO.Path.GetFileName(path));
            return new ModPath(modKey, path);
        }

        public static bool TryFromPath(string path, [MaybeNullWhen(false)] out ModPath modPath)
        {
            if (!ModKey.TryFactory(System.IO.Path.GetFileName(path), out var modKey))
            {
                modPath = default;
                return false;
            }
            modPath = new ModPath(modKey, path);
            return true;
        }

        public static implicit operator ModPath(string str)
        {
            return FromPath(str);
        }

        public static implicit operator string(ModPath p)
        {
            return p.Path;
        }

        public static implicit operator ModKey(ModPath p)
        {
            return p.ModKey;
        }

        public override string ToString()
        {
            return $"{ModKey} => {Path}";
        }

        public override bool Equals(object obj)
        {
            return obj is ModPath modPath && Equals(modPath);
        }

        public bool Equals(ModPath other)
        {
            if (!ModKey.Equals(other.ModKey)) return false;
            if (string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(ModKey);
            hash.Add(Path.GetHashCode(StringComparison.OrdinalIgnoreCase));
            return hash.ToHashCode();
        }
    }
}
