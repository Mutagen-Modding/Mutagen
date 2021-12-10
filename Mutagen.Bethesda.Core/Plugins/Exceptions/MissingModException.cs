using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public class MissingModException : ModPathException
{
    public MissingModException(ModPath path, string? message = null, Exception? innerException = null) : base(path, message, innerException)
    {
    }

    public MissingModException(IEnumerable<ModPath> path, string? message = null, Exception? innerException = null) : base(path, message, innerException)
    {
    }

    public MissingModException(ModKey key, string? message = null, Exception? innerException = null) : base(key, message, innerException)
    {
    }

    public MissingModException(IEnumerable<ModKey> keys, string? message = null, Exception? innerException = null) : base(keys, message, innerException)
    {
    }
}