using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using System;

namespace Mutagen.Bethesda.Plugins.Exceptions
{
    public class RecordException : Exception
    {
        public ModKey? ModKey { get; private set; }
        public FormKey? FormKey { get; private set; }
        public Type? RecordType { get; private set; }
        public string? EditorID { get; private set; }

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
        public static RecordException Enrich(Exception ex, IMajorRecordGetter? majorRec)
        {
            return Enrich(ex, majorRec?.FormKey, majorRec?.Registration.ClassType, majorRec?.EditorID);
        }

        public static RecordException Enrich(Exception ex, ModKey? modKey, IMajorRecordGetter? majorRec)
        {
            return Enrich(ex, majorRec?.FormKey, majorRec?.Registration.ClassType, majorRec?.EditorID, modKey);
        }

        public static RecordException Enrich(Exception ex, FormKey? formKey, Type? recordType, string? edid, ModKey? modKey = null)
        {
            if (ex is RecordException rec)
            {
                if (rec.ModKey == null && modKey != null)
                {
                    rec.ModKey = modKey;
                }
                if (rec.EditorID == null && edid != null)
                {
                    rec.EditorID = edid;
                }
                if (rec.FormKey == null && formKey != null)
                {
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

        #endregion

        #region Create
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
                    return $"{nameof(RecordException)} {ModKey} => {FormKey}: {this.Message} {this.InnerException}{this.StackTrace}";
                }
                else
                {
                    return $"{nameof(RecordException)} {ModKey} => {FormKey}<{RecordType.Name}>: {this.Message} {this.InnerException}{this.StackTrace}";
                }
            }
            else
            {
                if (RecordType == null)
                {
                    return $"{nameof(RecordException)} {ModKey} => {EditorID} ({FormKey}): {this.Message} {this.InnerException}{this.StackTrace}";
                }
                else
                {
                    return $"{nameof(RecordException)} {ModKey} => {EditorID} ({FormKey}<{RecordType.Name}>): {this.Message} {this.InnerException}{this.StackTrace}";
                }
            }
        }
    }
}
