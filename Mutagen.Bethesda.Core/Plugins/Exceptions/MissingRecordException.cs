using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Plugins.Exceptions
{
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
                    return $"{nameof(MissingRecordException)} {FormKey?.ToString() ?? EditorID}<{Type}>: {this.Message} {this.InnerException}{this.StackTrace}";
                }
                else
                {
                    return $"{nameof(MissingRecordException)} {FormKey?.ToString() ?? EditorID}: {this.Message} {this.InnerException}{this.StackTrace}";
                }
            }
            else
            {
                return $"{nameof(MissingRecordException)} {FormKey?.ToString() ?? EditorID}<{Type} (+{Types.Length - 1})>: {this.Message} {this.InnerException}{this.StackTrace}";
            }
        }
    }
}
