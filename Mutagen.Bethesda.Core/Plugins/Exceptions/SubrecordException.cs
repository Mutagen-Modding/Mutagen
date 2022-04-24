namespace Mutagen.Bethesda.Plugins.Exceptions;

public class SubrecordException : RecordException
{
    public RecordType Subrecord { get; internal set; }

    public SubrecordException(RecordType subRecord, FormKey? formKey, Type? majorRecordType, ModKey? modKey, string? edid)
        : base(formKey, majorRecordType, modKey, edid: edid)
    {
        Subrecord = subRecord;
    }

    public SubrecordException(RecordType subRecord, FormKey? formKey, Type? majorRecordType, ModKey? modKey, string? edid, string message) 
        : base(formKey, majorRecordType, modKey, edid: edid, message: message)
    {
        Subrecord = subRecord;
    }

    public SubrecordException(RecordType subRecord, FormKey? formKey, Type? majorRecordType, ModKey? modKey, string? edid, Exception innerException) 
        : base(formKey, majorRecordType, modKey, edid: edid, innerException)
    {
        Subrecord = subRecord;
    }

    public SubrecordException(RecordType subRecord, FormKey? formKey, Type? majorRecordType, ModKey? modKey, string? edid, string message, Exception innerException)
        : base(formKey, majorRecordType, modKey, edid: edid, message: message, innerException: innerException)
    {
        Subrecord = subRecord;
    }

    public static SubrecordException Enrich(Exception ex, RecordType subRecord)
    {
        if (ex is SubrecordException sub)
        {
            return sub;
        }
        return new SubrecordException(subRecord, formKey: null, majorRecordType: null, modKey: null, edid: null, innerException: ex);
    }

    [Obsolete("Use Create instead")]
    public static SubrecordException Factory(Exception ex, RecordType subRecord)
    {
        return Enrich(ex, subRecord);
    }

    [Obsolete("Use Enrich instead")]
    public static SubrecordException FactoryPassthroughExisting(Exception ex, RecordType subRecord)
    {
        return Enrich(ex, subRecord);
    }
        
    public static SubrecordException Create(string message, RecordType recordType)
    {
        return new SubrecordException(recordType, default(FormKey?), default(Type?), default(ModKey?),
            default(string?), message: message);
    }

    public override string ToString()
    {
        if (EditorID == null)
        {
            if (RecordType == null)
            {
                return $"{nameof(SubrecordException)} {ModKey} => {FormKey} => {Subrecord}: {this.Message} {this.InnerException}{this.StackTrace}";
            }
            else
            {
                return $"{nameof(SubrecordException)} {ModKey} => {FormKey}<{RecordType.Name}> => {Subrecord}: {this.Message} {this.InnerException}{this.StackTrace}";
            }
        }
        else
        {
            if (RecordType == null)
            {
                return $"{nameof(SubrecordException)} {ModKey} => {EditorID} ({FormKey}) => {Subrecord}: {this.Message} {this.InnerException}{this.StackTrace}";
            }
            else
            {
                return $"{nameof(SubrecordException)} {ModKey} => {EditorID} ({FormKey}<{RecordType.Name}>) => {Subrecord}: {this.Message} {this.InnerException}{this.StackTrace}";
            }
        }
    }
}