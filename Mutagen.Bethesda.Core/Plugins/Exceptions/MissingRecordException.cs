namespace Mutagen.Bethesda.Plugins.Exceptions;

public class MissingRecordException : Exception
{
    public FormKey? FormKey { get; }
    public string? EditorID { get; }
    public Type? Type { get; }
    public Type[] Types { get; } = Array.Empty<Type>();

    public MissingRecordException(FormKey formKey, Type type)
    {
        FormKey = formKey;
        Type = type;
    }

    public MissingRecordException(IFormLinkIdentifier formLinkIdentifier)
    {
        FormKey = formLinkIdentifier.FormKey;
        Type = formLinkIdentifier.Type;
    }

    public MissingRecordException(string editorID, Type type)
    {
        EditorID = editorID;
        Type = type;
    }

    public MissingRecordException(FormKey formKey, Type[] types)
    {
        FormKey = formKey;
        Type = types.FirstOrDefault();
        Types = types;
    }

    public MissingRecordException(string editorID, Type[] types)
    {
        EditorID = editorID;
        Type = types.FirstOrDefault();
        Types = types;
    }

    public override string ToString()
    {
        if (Types.Length == 0)
        {
            if (Type != null)
            {
                return $"{nameof(MissingRecordException)} {FormKey?.ToString() ?? EditorID}<{Type}>: {Message} {InnerException}{StackTrace}";
            }
            else
            {
                return $"{nameof(MissingRecordException)} {FormKey?.ToString() ?? EditorID}: {Message} {InnerException}{StackTrace}";
            }
        }
        else
        {
            return $"{nameof(MissingRecordException)} {FormKey?.ToString() ?? EditorID}<{Type} (+{Types.Length - 1})>: {Message} {InnerException}{StackTrace}";
        }
    }
}