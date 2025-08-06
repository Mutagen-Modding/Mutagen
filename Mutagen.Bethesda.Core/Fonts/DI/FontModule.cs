using StrongInject;

namespace Mutagen.Bethesda.Fonts.DI;

[Register<GetFontConfigListing, IGetFontConfigListing>]
[Register<GetFontConfig, IGetFontConfig>]
[Register<FontProvider, IFontProvider>]
[Register<FontProviderFactory, IFontProviderFactory>]
internal class FontModule
{
    
}
