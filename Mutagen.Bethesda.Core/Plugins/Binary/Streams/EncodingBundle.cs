using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

public record EncodingBundle(IMutagenEncoding NonTranslated, IMutagenEncoding NonLocalized);