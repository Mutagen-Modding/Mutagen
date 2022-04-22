using Newtonsoft.Json;

namespace Mutagen.Bethesda.Json;

public static class JsonConvertersMixIn
{
    public static readonly ModKeyJsonConverter ModKey = new();
    public static readonly FormKeyJsonConverter FormKey = new();

    public static void AddMutagenConverters(this JsonSerializerSettings settings)
    {
        settings.Converters.Add(ModKey);
        settings.Converters.Add(FormKey);
    }
}