namespace Mutagen.Bethesda.Plugins.Exceptions;

/// <summary>
/// Exception thrown when a LinkCache is required to resolve a cross-mod record type but is not available
/// </summary>
public class LinkCacheMissingException : Exception
{
    public FormKey FormKey { get; }
    public Type? RecordType { get; }

    public LinkCacheMissingException(FormKey formKey, Type? recordType, string message)
        : base(message)
    {
        FormKey = formKey;
        RecordType = recordType;
    }

    public LinkCacheMissingException(FormKey formKey, Type? recordType, string message, Exception? innerException)
        : base(message, innerException)
    {
        FormKey = formKey;
        RecordType = recordType;
    }

    public override string ToString()
    {
        if (RecordType != null)
        {
            return $"{nameof(LinkCacheMissingException)} {FormKey}<{RecordType.Name}>: {Message} {InnerException}{StackTrace}";
        }
        else
        {
            return $"{nameof(LinkCacheMissingException)} {FormKey}: {Message} {InnerException}{StackTrace}";
        }
    }
}
