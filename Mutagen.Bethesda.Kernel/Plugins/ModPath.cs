using System;
using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Plugins
{
    public record ModPath : IModKeyed
    {
        public static readonly ModPath Empty = new ModPath(ModKey.Null, string.Empty);
        
        public ModKey ModKey { get; }
        public FilePath Path { get; }

        public ModPath(ModKey modKey, FilePath path)
        {
            ModKey = modKey;
            Path = path;
        }

        /// <summary>
        /// Constructs a ModPath from a string
        /// </summary>
        /// <param name="path">FilePath to convert from</param>
        /// <exception cref="ArgumentException">Throws if path file name was not convertable to a ModKey</exception>
        public ModPath(FilePath path)
        {
            ModKey = ModKey.FromFileName(path.Name);
            Path = path;
        }

        /// <summary>
        /// Constructs a ModPath from a string
        /// </summary>
        /// <param name="path">String to convert from</param>
        /// <exception cref="ArgumentException">Throws if path file name was not convertable to a ModKey</exception>
        public ModPath(string path)
        {
            ModKey = ModKey.FromFileName(System.IO.Path.GetFileName(path));
            Path = path;
        }

        /// <summary>
        /// Constructs a ModPath from a string
        /// </summary>
        /// <param name="path">String to convert from</param>
        /// <exception cref="ArgumentException">Throws if path file name was not convertable to a ModKey</exception>
        public static ModPath FromPath(FilePath path) => new(path);

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
