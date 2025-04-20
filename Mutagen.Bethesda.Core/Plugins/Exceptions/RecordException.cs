using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using Loqui;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Exceptions;

/// <summary>
/// An exception contains information about an associated Record
/// </summary>
public class RecordException : Exception
{
    public ModKey? ModKey { get; private set; }
    public FormKey? FormKey { get; private set; }
    public Type? RecordType { get; private set; }
    public string? EditorID { get; private set; }

    static RecordException()
    {
        Warmup.Init();
    }

    public RecordException(FormKey? formKey, Type? recordType, ModKey? modKey, string? edid)
    {
        FormKey = formKey;
        RecordType = recordType;
        ModKey = modKey;
        EditorID = edid;
    }

    public RecordException(FormKey? formKey, Type? recordType, ModKey? modKey, string? edid, string message)
        : base(message)
    {
        FormKey = formKey;
        RecordType = recordType;
        ModKey = modKey;
        EditorID = edid;
    }

    public RecordException(FormKey? formKey, Type? recordType, ModKey? modKey, string? edid, string message, Exception? innerException)
        : base(message, innerException)
    {
        FormKey = formKey;
        RecordType = recordType;
        ModKey = modKey;
        EditorID = edid;
    }

    public RecordException(FormKey? formKey, Type? recordType, ModKey? modKey, string? edid, Exception innerException)
        : base(innerException.Message, innerException)
    {
        FormKey = formKey;
        RecordType = recordType;
        ModKey = modKey;
        EditorID = edid;
    }

    #region Enrich
    /// <summary>
    /// Wraps an exception to associate it with a specific major record
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="majorRec">Major Record to pull information from</param>
    public static RecordException Enrich(Exception ex, IMajorRecordGetter? majorRec)
    {
        return Enrich(ex, majorRec?.FormKey, majorRec?.Registration.ClassType, majorRec?.EditorID);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="modKey">ModKey to mark as containing the record</param>
    /// <param name="majorRec">Major Record to pull information from</param>
    public static RecordException Enrich(Exception ex, ModKey? modKey, IMajorRecordGetter? majorRec)
    {
        return Enrich(ex, majorRec?.FormKey, majorRec?.Registration.ClassType, majorRec?.EditorID, modKey);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="formKey">FormKey to mark the exception to be associated with</param>
    /// <param name="recordType">C# Type that the record is</param>
    /// <param name="edid">EditorID to mark the exception to be associated with</param>
    /// <param name="modKey">ModKey to mark as containing the record</param>
    public static RecordException Enrich(Exception ex, FormKey? formKey, Type? recordType, string? edid = null, ModKey? modKey = null)
    {
        if (ex is RecordException rec)
        {
            if (rec.ModKey == null && modKey != null)
            {
                rec.ModKey = modKey;
            }

            if (rec.EditorID == null && rec.FormKey == null)
            {
                rec.EditorID = edid;
                rec.FormKey = formKey;
            }
            if (rec.RecordType == null && recordType != null)
            {
                rec.RecordType = recordType;
            }
            return rec;
        }
        return new RecordException(
            formKey: formKey,
            modKey: modKey,
            edid: edid,
            recordType: recordType,
            innerException: ex);
    }

    private static Type GetRecordType(Type t)
    {
        if (LoquiRegistration.TryGetRegister(t, out var regis))
        {
            return regis.ClassType;
        }

        return t;
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="formKey">FormKey to mark the exception to be associated with</param>
    /// <param name="edid">EditorID to mark the exception to be associated with</param>
    /// <param name="modKey">ModKey to mark as containing the record</param>
    public static RecordException Enrich<TMajor>(Exception ex, FormKey? formKey, string? edid, ModKey? modKey = null)
        where TMajor : IMajorRecordGetter
    {
        return Enrich(
            ex,
            formKey,
            GetRecordType(typeof(TMajor)),
            edid,
            modKey);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="modKey">ModKey to mark as containing the record</param>
    public static RecordException Enrich(Exception ex, ModKey modKey)
    {
        if (ex is RecordException rec)
        {
            if (rec.ModKey == null)
            {
                rec.ModKey = modKey;
            }
            return rec;
        }
        return new RecordException(
            formKey: null,
            modKey: modKey,
            edid: null,
            recordType: null,
            innerException: ex);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="majorRecordContext">ModContext to pull information from</param>
    public static RecordException Enrich<TMajor>(Exception ex, IModContext<TMajor> majorRecordContext)
        where TMajor : IMajorRecordGetter
    {
        return Enrich(ex, modKey: majorRecordContext.ModKey, majorRec: majorRecordContext.Record);
    }

    #endregion

    #region EnrichAndThrow
    /// <summary>
    /// Wraps an exception to associate it with a specific major record, and then throws it
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="majorRec">Major Record to pull information from</param>
    [DoesNotReturn]
    public static void EnrichAndThrow(Exception ex, IMajorRecordGetter? majorRec)
    {
        EnrichAndThrow(ex, majorRec?.FormKey, majorRec?.Registration.ClassType, majorRec?.EditorID);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record, and then throws it
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="modKey">ModKey to mark as containing the record</param>
    /// <param name="majorRec">Major Record to pull information from</param>
    [DoesNotReturn]
    public static void EnrichAndThrow(Exception ex, ModKey? modKey, IMajorRecordGetter? majorRec)
    {
        EnrichAndThrow(ex, majorRec?.FormKey, majorRec?.Registration.ClassType, majorRec?.EditorID, modKey);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record, and then throws it
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="formKey">FormKey to mark the exception to be associated with</param>
    /// <param name="recordType">C# Type that the record is</param>
    /// <param name="edid">EditorID to mark the exception to be associated with</param>
    /// <param name="modKey">ModKey to mark as containing the record</param>
    [DoesNotReturn]
    public static void EnrichAndThrow(Exception ex, FormKey? formKey, Type? recordType, string? edid = null, ModKey? modKey = null)
    {
        if (ex is RecordException rec)
        {
            if (rec.ModKey == null && modKey != null)
            {
                rec.ModKey = modKey;
            }

            if (rec.EditorID == null && rec.FormKey == null)
            {
                rec.EditorID = edid;
                rec.FormKey = formKey;
            }
            if (rec.RecordType == null && recordType != null)
            {
                rec.RecordType = recordType;
            }
            ExceptionDispatchInfo.Capture(rec).Throw();
        }
        throw new RecordException(
            formKey: formKey,
            modKey: modKey,
            edid: edid,
            recordType: recordType,
            innerException: ex);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record, and then throws it
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="formKey">FormKey to mark the exception to be associated with</param>
    /// <param name="edid">EditorID to mark the exception to be associated with</param>
    /// <param name="modKey">ModKey to mark as containing the record</param>
    [DoesNotReturn]
    public static void EnrichAndThrow<TMajor>(Exception ex, FormKey? formKey, string? edid, ModKey? modKey = null)
        where TMajor : IMajorRecordGetter
    {
        EnrichAndThrow(
            ex,
            formKey,
            GetRecordType(typeof(TMajor)),
            edid,
            modKey);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record, and then throws it
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="modKey">ModKey to mark as containing the record</param>
    [DoesNotReturn]
    public static void EnrichAndThrow(Exception ex, ModKey modKey)
    {
        if (ex is RecordException rec)
        {
            if (rec.ModKey == null)
            {
                rec.ModKey = modKey;
            }
            ExceptionDispatchInfo.Capture(rec).Throw();
        }
        throw new RecordException(
            formKey: null,
            modKey: modKey,
            edid: null,
            recordType: null,
            innerException: ex);
    }

    /// <summary>
    /// Wraps an exception to associate it with a specific major record, and then throws it
    /// </summary>
    /// <param name="ex">Exception to enrich</param>
    /// <param name="majorRecordContext">ModContext to pull information from</param>
    [DoesNotReturn]
    public static void EnrichAndThrow<TMajor>(Exception ex, IModContext<TMajor> majorRecordContext)
        where TMajor : IMajorRecordGetter
    {
        EnrichAndThrow(ex, modKey: majorRecordContext.ModKey, majorRec: majorRecordContext.Record);
    }

    #endregion

    #region Create
    /// <summary>
    /// Creates an exception associated with a specific major record
    /// </summary>
    public static RecordException Create(string message, IMajorRecordGetter majorRec, Exception? innerException = null)
    {
        return new RecordException(
            formKey: majorRec.FormKey,
            modKey: majorRec.FormKey.ModKey,
            edid: majorRec.EditorID,
            message: message,
            recordType: majorRec.Registration.ClassType,
            innerException: innerException);
    }

    /// <summary>
    /// Creates an exception associated with a specific major record
    /// </summary>
    public static RecordException Create(string message, ModKey? modKey, IMajorRecordGetter majorRec, Exception? innerException = null)
    {
        return new RecordException(
            formKey: majorRec.FormKey,
            modKey: modKey,
            edid: majorRec.EditorID,
            message: message,
            recordType: majorRec.Registration.ClassType,
            innerException: innerException);
    }

    /// <summary>
    /// Creates an exception associated with a specific major record
    /// </summary>
    public static RecordException Create(string message, FormKey? formKey, Type? recordType, string? edid, ModKey? modKey = null, Exception? innerException = null)
    {
        return new RecordException(
            formKey: formKey,
            modKey: modKey,
            edid: edid,
            message: message,
            recordType: recordType,
            innerException: innerException);
    }

    /// <summary>
    /// Creates an exception associated with a specific major record
    /// </summary>
    public static RecordException Create(string message, ModKey modKey, Exception? innerException = null)
    {
        return new RecordException(
            formKey: null,
            modKey: modKey,
            edid: null,
            recordType: null,
            message: message,
            innerException: innerException);
    }

    /// <summary>
    /// Creates an exception associated with a specific major record
    /// </summary>
    public static RecordException Create<TMajor>(string message, FormKey? formKey, string? edid, ModKey? modKey = null, Exception? innerException = null)
        where TMajor : IMajorRecordGetter
    {
        return Create(
            message: message,
            formKey: formKey,
            recordType: GetRecordType(typeof(TMajor)),
            modKey: modKey,
            edid: edid,
            innerException: innerException);
    }
    #endregion

    #region Deprectiated
    [Obsolete("Use Enrich instead")]
    public static RecordException Factory(Exception ex, IMajorRecordGetter majorRec)
    {
        return Enrich(ex, majorRec.FormKey, majorRec.Registration.ClassType, majorRec.EditorID);
    }

    [Obsolete("Use Enrich instead")]
    public static RecordException Factory(Exception ex, ModKey? modKey, IMajorRecordGetter majorRec)
    {
        return Enrich(ex, majorRec.FormKey, majorRec.Registration.ClassType, majorRec.EditorID, modKey);
    }

    [Obsolete("Use Enrich instead")]
    public static RecordException Factory(Exception ex, FormKey? formKey, string? edid, ModKey? modKey = null)
    {
        return Enrich(ex, formKey, recordType: null, edid, modKey);
    }

    [Obsolete("Use Enrich instead")]
    public static RecordException Factory(Exception ex, ModKey modKey)
    {
        return Enrich(ex, modKey);
    }

    [Obsolete("Use Create instead")]
    public static RecordException Factory(string message, IMajorRecordGetter majorRec, Exception? innerException = null)
    {
        return Create(message, majorRec, innerException);
    }

    [Obsolete("Use Create instead")]
    public static RecordException Factory(string message, ModKey? modKey, IMajorRecordGetter majorRec, Exception? innerException = null)
    {
        return Create(message, modKey, majorRec, innerException);
    }

    [Obsolete("Use Create instead")]
    public static RecordException Factory(string message, FormKey? formKey, string? edid, ModKey? modKey = null, Exception? innerException = null)
    {
        return Create(message, formKey, recordType: null, edid, modKey, innerException);
    }

    [Obsolete("Use Create instead")]
    public static RecordException Factory(string message, ModKey modKey, Exception? innerException = null)
    {
        return Create(message, modKey, innerException);
    }
    #endregion

    public override string ToString()
    {
        if (EditorID == null)
        {
            if (RecordType == null)
            {
                return $"{nameof(RecordException)} {ModKey} => {FormKey}: {Message} {InnerException}{StackTrace}";
            }
            else
            {
                return $"{nameof(RecordException)} {ModKey} => {FormKey}<{RecordType.Name}>: {Message} {InnerException}{StackTrace}";
            }
        }
        else
        {
            if (RecordType == null)
            {
                return $"{nameof(RecordException)} {ModKey} => {EditorID} ({FormKey}): {Message} {InnerException}{StackTrace}";
            }
            else
            {
                return $"{nameof(RecordException)} {ModKey} => {EditorID} ({FormKey}<{RecordType.Name}>): {Message} {InnerException}{StackTrace}";
            }
        }
    }
}