using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public class Constants
{
    public static readonly ModKey Fallout3 = new ModKey("Fallout3", type: ModType.Master);
    public static readonly ModKey Anchorage = new ModKey("Anchorage", type: ModType.Master);
    public static readonly ModKey ThePitt = new ModKey("ThePitt", type: ModType.Master);
    public static readonly ModKey BrokenSteel = new ModKey("BrokenSteel", type: ModType.Master);
    public static readonly ModKey PointLookout = new ModKey("PointLookout", type: ModType.Master);
    public static readonly ModKey Zeta = new ModKey("Zeta", type: ModType.Master);
    public static readonly IFormLinkGetter<IFallout3MajorRecordGetter> Player = new FormLink<IFallout3MajorRecordGetter>(new FormKey(Fallout3, id: 0x14));
}
