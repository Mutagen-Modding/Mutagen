using Mutagen.Bethesda.Plugins;
using Newtonsoft.Json;
using Noggog;

namespace Mutagen.Bethesda.Json;

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
            if (!objectType.Name.Contains("FormLink"))
            {
                throw new ArgumentException();
            }

            if (IsNullableLink(objectType))
            {
                return Activator.CreateInstance(
                    typeof(FormLinkNullable<>).MakeGenericType(objectType.GenericTypeArguments[0]));
            }
            else
            {
                return Activator.CreateInstance(
                    typeof(FormLink<>).MakeGenericType(objectType.GenericTypeArguments[0]),
                    FormKey.Null);
            }
        }
        else
        {
            var str = obj.ToString();
                
            if (objectType == typeof(FormKey))
            {
                if (str.IsNullOrWhitespace())
                {
                    return FormKey.Null;
                }
                else
                {
                    return FormKey.Factory(str);
                }
            }
            else if (objectType == typeof(FormKey?))
            {
                if (str.IsNullOrWhitespace())
                {
                    return default(FormKey?);
                }
                else
                {
                    return FormKey.Factory(str);
                }
            }

            if (!objectType.Name.Contains("FormLink"))
            {
                throw new ArgumentException();
            }

            FormKey key;
            if (str.IsNullOrWhitespace())
            {
                key = FormKey.Null;
            }
            else
            {
                key = FormKey.Factory(str);
            }
                
            if (IsNullableLink(objectType))
            {
                return Activator.CreateInstance(
                    typeof(FormLinkNullable<>).MakeGenericType(objectType.GenericTypeArguments[0]),
                    key);
            }
            else
            {
                return Activator.CreateInstance(
                    typeof(FormLink<>).MakeGenericType(objectType.GenericTypeArguments[0]),
                    key);
            }
        }
    }

    private bool IsNullableLink(Type type)
    {
        return type.Name.AsSpan()[..^2].EndsWith("Nullable", StringComparison.Ordinal);
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
                writer.WriteValue(fl.FormKeyNullable?.ToString());
                break;
            default:
                throw new ArgumentException();
        }
    }
}