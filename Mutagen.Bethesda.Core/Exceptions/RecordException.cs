using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class RecordException : Exception
    {
        public ModKey? ModKey { get; internal set; }
        public FormKey? FormKey { get; internal set; }
        public string? EditorID { get; internal set; }

        public RecordException(FormKey? formKey, ModKey? modKey, string? edid)
        {
            FormKey = formKey;
            ModKey = modKey;
            EditorID = edid;
        }

        public RecordException(FormKey? formKey, ModKey? modKey, string? edid, string message)
            : base(message)
        {
            FormKey = formKey;
            ModKey = modKey;
            EditorID = edid;
        }

        public RecordException(FormKey? formKey, ModKey? modKey, string? edid, string message, Exception innerException)
            : base(message, innerException)
        {
            FormKey = formKey;
            ModKey = modKey;
            EditorID = edid;
        }

        public RecordException(FormKey? formKey, ModKey? modKey, string? edid, Exception innerException) 
            : base(innerException.Message, innerException)
        {
            FormKey = formKey;
            ModKey = modKey;
            EditorID = edid;
        }

        public static RecordException Factory(Exception ex, IMajorRecordCommonGetter majorRec)
        {
            return Factory(ex, majorRec.FormKey, majorRec.EditorID);
        }

        public static RecordException Factory(Exception ex, ModKey? modKey, IMajorRecordCommonGetter majorRec)
        {
            return Factory(ex, majorRec.FormKey, majorRec.EditorID, modKey);
        }

        public static RecordException Factory(Exception ex, FormKey? formKey, string? edid, ModKey? modKey = null)
        {
            if (ex is RecordException rec)
            {
                if (modKey != null)
                {
                    rec.ModKey = modKey;
                }
                if (edid != null)
                {
                    rec.EditorID = edid;
                }
                if (formKey != null)
                {
                    rec.FormKey = formKey;
                }
                return rec;
            }
            return new RecordException(
                formKey: formKey,
                modKey: modKey,
                edid: edid,
                innerException: ex);
        }

        public static RecordException Factory(Exception ex, ModKey modKey)
        {
            if (ex is RecordException rec)
            {
                rec.ModKey = modKey;
                return rec;
            }
            return new RecordException(
                formKey: null,
                modKey: modKey,
                edid: null,
                innerException: ex);
        }

        public override string ToString()
        {
            if (EditorID == null)
            {
                return $"{nameof(RecordException)} {ModKey} => {FormKey}: {this.Message} {this.InnerException}{this.StackTrace}";
            }
            else
            {
                return $"{nameof(RecordException)} {ModKey} => {EditorID} ({FormKey}): {this.Message} {this.InnerException}{this.StackTrace}";
            }
        }
    }
}
