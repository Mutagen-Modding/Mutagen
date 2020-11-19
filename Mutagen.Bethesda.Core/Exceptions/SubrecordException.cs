using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class SubrecordException : RecordException
    {
        public RecordType Subrecord { get; internal set; }

        public SubrecordException(RecordType record, FormKey? formKey, ModKey? modKey, string? edid)
            : base(formKey, modKey, edid: edid)
        {
            Subrecord = record;
        }

        public SubrecordException(RecordType record, FormKey? formKey, ModKey? modKey, string? edid, string message) 
            : base(formKey, modKey, edid: edid, message: message)
        {
            Subrecord = record;
        }

        public SubrecordException(RecordType record, FormKey? formKey, ModKey? modKey, string? edid, Exception innerException) 
            : base(formKey, modKey, edid: edid, innerException)
        {
            Subrecord = record;
        }

        public SubrecordException(RecordType record, FormKey? formKey, ModKey? modKey, string? edid, string message, Exception innerException)
            : base(formKey, modKey, edid: edid, message: message, innerException: innerException)
        {
            Subrecord = record;
        }

        public static SubrecordException Factory(Exception ex, RecordType record)
        {
            return new SubrecordException(record, formKey: null, modKey: null, edid: null, innerException: ex);
        }

        public static SubrecordException FactoryPassthroughExisting(Exception ex, RecordType record)
        {
            if (ex is SubrecordException sub)
            {
                return sub;
            }
            return Factory(ex, record);
        }

        public override string ToString()
        {
            if (EditorID == null)
            {
                return $"{nameof(SubrecordException)} {ModKey} => {FormKey} => {Subrecord}: {this.Message} {this.InnerException}{this.StackTrace}";
            }
            else
            {
                return $"{nameof(SubrecordException)} {ModKey} => {EditorID} ({FormKey}) => {Subrecord}: {this.Message} {this.InnerException}{this.StackTrace}";
            }
        }
    }
}
