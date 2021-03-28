using System;
using JetBrains.Annotations;

namespace Mutagen.Bethesda.Core.Pex.Exceptions
{
    [PublicAPI]
    public class PexParsingException : Exception
    {
        public PexParsingException(string msg) : base(msg) { }
        public PexParsingException(string msg, Exception e) : base(msg, e) { }
    }
}
