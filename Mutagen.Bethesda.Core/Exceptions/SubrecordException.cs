using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class SubrecordException : RecordException
    {
        public RecordType Subrecord;

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

        public override string ToString()
        {
            if (EditorID == null)
            {
                return $"{nameof(SubrecordException)} {ModKey} => {FormKey} => {Subrecord}: {this.Message} {this.InnerException}";
            }
            else
            {
                return $"{nameof(SubrecordException)} {ModKey} => {EditorID} ({FormKey}) => {Subrecord}: {this.Message} {this.InnerException}";
            }
        }
    }
}
