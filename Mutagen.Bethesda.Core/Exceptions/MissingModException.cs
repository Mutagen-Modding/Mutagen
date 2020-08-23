using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class MissingModException : Exception
    {
        public ModPath ModPath { get; }

        public MissingModException(ModPath path)
        {
            ModPath = path;
        }

        public MissingModException(ModPath path, string message)
            : base(message)
        {
            ModPath = path;
        }

        public MissingModException(ModKey key)
        {
            ModPath = new ModPath(key, string.Empty);
        }

        public MissingModException(ModKey key, string message) 
            : base(message)
        {
            ModPath = new ModPath(key, string.Empty);
        }
    }
}
