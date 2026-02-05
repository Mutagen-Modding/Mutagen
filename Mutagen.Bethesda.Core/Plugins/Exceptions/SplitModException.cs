namespace Mutagen.Bethesda.Plugins.Exceptions;

/// <summary>
/// Exception thrown when split mod files have issues
/// </summary>
public class SplitModException : Exception
{
    public SplitModException(string message) : base(message) { }
    public SplitModException(string message, Exception innerException) : base(message, innerException) { }
}