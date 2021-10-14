using Mutagen.Bethesda.Plugins;
using Newtonsoft.Json;
using System;

namespace Mutagen.Bethesda.Json
{
    public class FormKeyJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(FormKey)) return true;
            if (objectType == typeof(FormKey?)) return true;
            if (!objectType.IsGenericType) return false;
            if (typeof(IFormLinkGetter).IsAssignableFrom(objectType)) return true;
            return false;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var obj = reader.Value;
            if (obj == null)
            {
                if (objectType == typeof(FormKey))
                {
                    return FormKey.Null;
                }
                if (objectType == typeof(FormKey?))
                {
                    return null;
                }
                throw new NotImplementedException();
            }
            else
            {
                if (objectType == typeof(FormKey)
                    || objectType == typeof(FormKey?))
                {
                    return FormKey.Factory(obj.ToString());
                }
                if (!objectType.Name.Contains("FormLink"))
                {
                    throw new ArgumentException();
                }

                if (objectType.Name.AsSpan()[..^2].EndsWith("Nullable", StringComparison.Ordinal))
                {
                    return Activator.CreateInstance(
                        typeof(FormLinkNullable<>).MakeGenericType(objectType.GenericTypeArguments[0]),
                        FormKey.Factory(obj.ToString()));
                }
                else
                {
                    return Activator.CreateInstance(
                        typeof(FormLink<>).MakeGenericType(objectType.GenericTypeArguments[0]),
                        FormKey.Factory(obj.ToString()));
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null) return;
            switch (value)
            {
                case FormKey fk:
                    writer.WriteValue(fk.ToString());
                    break;
                case IFormLinkGetter fl:
                    writer.WriteValue(fl.FormKey.ToString());
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
