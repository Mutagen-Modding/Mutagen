using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Newtonsoft.Json;
using Noggog;

namespace Mutagen.Bethesda.Json;

public sealed class FormKeyJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(FormKey)) return true;
        if (objectType == typeof(FormKey?)) return true;
        if (objectType == typeof(FormLinkInformation)) return true;
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

            if (objectType.IsGenericType)
            {
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
            else
            {
                if (str.IsNullOrWhitespace() || str == "Null")
                {
                    return new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter));
                }
                
                var span = str.AsSpan();
                var startIndex = span.IndexOf('<');
                if (startIndex == -1)
                {
                    throw new ArgumentException();
                }

                var endIndex = span.IndexOf('>');
                if (endIndex == -1)
                {
                    throw new ArgumentException();
                }

                var typeName = span.Slice(startIndex + 1, endIndex - 1 - startIndex).ToString();

                var lastPeriod = typeName.LastIndexOf(".", StringComparison.Ordinal);
                if (lastPeriod != -1 && typeName[(lastPeriod + 1)..] == "MajorRecord")
                {
                    typeName = "Mutagen.Bethesda.Plugins.Records.MajorRecord";
                }
                else if (!typeName.StartsWith("Mutagen.Bethesda."))
                {
                    typeName = "Mutagen.Bethesda." + typeName;
                }
                var regis = LoquiRegistration.GetRegisterByFullName(typeName);
                if (regis == null)
                {
                    throw new ArgumentException($"Unknown object type: {typeName}");
                }

                return new FormLinkInformation(
                    FormKey.Factory(span.Slice(0, startIndex)),
                    regis.GetterType);
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
            case IFormLinkGetter fl when fl.GetType().IsGenericType:
                writer.WriteValue(fl.FormKeyNullable?.ToString());
                break;
            case IFormLinkIdentifier ident:
                writer.WriteValue(IFormLinkIdentifier.GetString(ident, simpleType: true));
                break;
            default:
                throw new ArgumentException();
        }
    }
}