using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Json
{
    public class ModKeyJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(ModKey)) return true;
            if (objectType == typeof(ModKey?)) return true;
            return false;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var obj = reader.Value;
            if (obj == null)
            {
                if (objectType == typeof(ModKey))
                {
                    return ModKey.Null;
                }
                if (objectType == typeof(ModKey?))
                {
                    return null;
                }
                throw new ArgumentException();
            }
            return ModKey.FromNameAndExtension(obj.ToString());
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null) return;
            if (value is not ModKey modKey)
            {
                throw new ArgumentException();
            }
            writer.WriteValue(modKey.FileName);
        }
    }
}
