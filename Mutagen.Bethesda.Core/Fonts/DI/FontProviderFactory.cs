using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Fonts.DI;

public interface IFontProviderFactory
{
    IFontProvider Create(Language language);
}

public class FontProviderFactory(IGetFontConfig getFontConfig) : IFontProviderFactory
{
    public IFontProvider Create(Language language)
    {
        return new FontProvider(language, getFontConfig);
    }
}
