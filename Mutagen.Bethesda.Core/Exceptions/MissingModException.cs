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
    }
}
