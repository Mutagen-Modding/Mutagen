using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class RecordException : AggregateException
    {
        public ModKey? ModKey { get; }
        public FormKey? FormKey { get; }

        public RecordException(FormKey? formKey, ModKey? modKey)
        {
            FormKey = formKey;
            ModKey = modKey;
        }

        public RecordException(FormKey? formKey, ModKey? modKey, string message) : base(message)
        {
            FormKey = formKey;
            ModKey = modKey;
        }

        public RecordException(FormKey? formKey, ModKey? modKey, string message, Exception innerException) : base(message, innerException)
        {
            FormKey = formKey;
            ModKey = modKey;
        }

        public RecordException(FormKey? formKey, ModKey? modKey, Exception innerException) : base(innerException.Message, innerException)
        {
            FormKey = formKey;
            ModKey = modKey;
        }

        public static RecordException Factory(Exception ex, FormKey? formKey)
        {
            if (ex is RecordException rec)
            {
                return new RecordException(
                    formKey: formKey,
                    modKey: rec.ModKey,
                    innerException: rec.InnerException);
            }
            return new RecordException(
                formKey: formKey,
                modKey: null,
                innerException: ex);
        }

        public static RecordException Factory(Exception ex, ModKey modKey)
        {
            if (ex is RecordException rec)
            {
                return new RecordException(
                    formKey: rec.FormKey,
                    modKey: modKey,
                    innerException: rec.InnerException);
            }
            return new RecordException(
                formKey: null,
                modKey: modKey,
                innerException: ex);
        }

        public override string ToString()
        {
            return $"{nameof(RecordException)} {ModKey} => {FormKey}: {this.Message} {this.InnerException}";
        }
    }
}
