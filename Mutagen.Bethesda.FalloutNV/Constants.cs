using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FalloutNV;

public class Constants
{
    public static readonly ModKey FalloutNV = new ModKey("FalloutNV", type: ModType.Master);
    public static readonly ModKey CaravanPack = new ModKey("CaravanPack", type: ModType.Master);
    public static readonly ModKey ClassicPack = new ModKey("ClassicPack", type: ModType.Master);
    public static readonly ModKey DeadMoney = new ModKey("DeadMoney", type: ModType.Master);
    public static readonly ModKey GunRunnersArsenal = new ModKey("GunRunnersArsenal", type: ModType.Master);
    public static readonly ModKey HonestHearts = new ModKey("HonestHearts", type: ModType.Master);
    public static readonly ModKey LonesomeRoad = new ModKey("LonesomeRoad", type: ModType.Master);
    public static readonly ModKey MercenaryPack = new ModKey("MercenaryPack", type: ModType.Master);
    public static readonly ModKey OldWorldBlues = new ModKey("OldWorldBlues", type: ModType.Master);
    public static readonly ModKey TribalPack = new ModKey("TribalPack", type: ModType.Master);
    public static readonly IFormLinkGetter<IFalloutNVMajorRecordGetter> Player = new FormLink<IFalloutNVMajorRecordGetter>(new FormKey(FalloutNV, id: 0x14));
}
