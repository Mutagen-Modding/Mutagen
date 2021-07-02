using System;
using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Plugins
{
    public record ModPath
    {
        public static readonly ModPath Empty = new ModPath(ModKey.Null, string.Empty);
        
        public ModKey ModKey { get; }
        public FilePath Path { get; }

        public ModPath(ModKey modKey, FilePath path)
        {
            ModKey = modKey;
            Path = path;
        }

        public static ModPath FromPath(FilePath path)
        {
            var modKey = ModKey.FromFileName(path.Name);
            return new ModPath(modKey, path);
        }

        public static bool TryFromPath(FilePath path, [MaybeNullWhen(false)] out ModPath modPath)
        {
            if (!ModKey.TryFromFileName(path.Name, out var modKey))
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

        public static implicit operator ModPath(FilePath filePath)
        {
            return FromPath(filePath);
        }

        public static implicit operator string(ModPath p)
        {
            return p.Path.Path;
        }

        public static implicit operator FilePath(ModPath p)
        {
            return p.Path;
        }

        public static implicit operator ModKey(ModPath p)
        {
            return p.ModKey;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Path.Path))
            {
                return ModKey.ToString();
            }
            else
            {
                return $"{ModKey} => {Path}";
            }
        }
    }
}
