using StrongInject;

namespace Mutagen.Bethesda.Fonts.DI;

[Register<GetFontConfigListing, IGetFontConfigListing>]
[Register<GetFontConfig, IGetFontConfig>]
[Register<FontProvider, IFontProvider>]
internal class FontModule
{
    
}
