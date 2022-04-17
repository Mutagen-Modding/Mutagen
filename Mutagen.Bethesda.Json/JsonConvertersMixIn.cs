using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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