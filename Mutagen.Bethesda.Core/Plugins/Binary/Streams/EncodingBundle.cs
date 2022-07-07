using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

public sealed record EncodingBundle(IMutagenEncoding NonTranslated, IMutagenEncoding NonLocalized);