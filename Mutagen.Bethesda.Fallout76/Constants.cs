using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76;

public class Constants
{
    public static readonly ModKey SeventySix = new ModKey("SeventySix", type: ModType.Master);
    public static readonly ModKey NW = new ModKey("NW", type: ModType.Master);
    public static readonly IFormLinkGetter<IFallout76MajorRecordGetter> Player = new FormLink<IFallout76MajorRecordGetter>(new FormKey(SeventySix, id: 0x14));
}