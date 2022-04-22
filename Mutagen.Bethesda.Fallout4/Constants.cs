using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

public class Constants
{
    public static readonly ModKey Fallout4 = new ModKey("Fallout4", type: ModType.Master);
    public static readonly ModKey Robot = new ModKey("DLCRobot", type: ModType.Master);
    public static readonly ModKey Workshop1 = new ModKey("DLCworkshop01", type: ModType.Master);
    public static readonly ModKey Coast = new ModKey("DLCCoast", type: ModType.Master);
    public static readonly ModKey Workshop2 = new ModKey("DLCworkshop02", type: ModType.Master);
    public static readonly ModKey Workshop3 = new ModKey("DLCworkshop03", type: ModType.Master);
    public static readonly ModKey NukaWorld = new ModKey("DLCNukaWorld", type: ModType.Master);
    public static readonly IFormLinkGetter<IFallout4MajorRecordGetter> Player = new FormLink<IFallout4MajorRecordGetter>(new FormKey(Fallout4, id: 0x14));
}