using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public class MissingModException : Exception
    {
        public ModPath ModPath => ModPaths[0];
        public IReadOnlyList<ModPath> ModPaths { get; }

        public MissingModException(ModPath path)
        {
            ModPaths = path.AsEnumerable().ToArray();
        }

        public MissingModException(ModPath path, string? message)
            : base(message)
        {
            ModPaths = path.AsEnumerable().ToArray();
        }

        public MissingModException(IEnumerable<ModPath> paths)
        {
            ModPaths = paths.ToArray();
        }

        public MissingModException(IEnumerable<ModPath> path, string? message)
            : base(message)
        {
            ModPaths = path.ToArray();
        }

        public MissingModException(ModKey key)
        {
            ModPaths = new ModPath(key, string.Empty).AsEnumerable().ToArray();
        }

        public MissingModException(IEnumerable<ModKey> keys)
        {
            ModPaths = keys.Select(key => new ModPath(key, string.Empty)).ToArray();
        }

        public MissingModException(ModKey key, string? message)
            : base(message)
        {
            ModPaths = new ModPath(key, string.Empty).AsEnumerable().ToArray();
        }

        public MissingModException(IEnumerable<ModKey> keys, string? message)
            : base(message)
        {
            ModPaths = keys.Select(key => new ModPath(key, string.Empty)).ToArray();
        }

        public override string ToString()
        {
            if (ModPaths.Count == 1)
            {
                return $"{nameof(MissingModException)} {ModPath}: {this.Message} {this.InnerException}{this.StackTrace}";
            }
            else
            {
                return $"{nameof(MissingModException)} {ModPath} (+{ModPaths.Count - 1}): {this.Message} {this.InnerException}{this.StackTrace}";
            }
        }
    }
}
