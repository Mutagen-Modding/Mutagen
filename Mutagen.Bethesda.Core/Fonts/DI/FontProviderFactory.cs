using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Fonts.DI;

public interface IFontProviderFactory
{
    IFontProvider Create(Language language);
}

public class FontProviderFactory(IGetFontConfig getFontConfig) : IFontProviderFactory
{
    private readonly Dictionary<Language, IFontProvider> _cache = new();
    private readonly object _lock = new();

    public IFontProvider Create(Language language)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(language, out var provider)) return provider;

            provider = new FontProvider(language, getFontConfig);
            _cache[language] = provider;
            return provider;
        }
    }
}
