using Mutagen.Bethesda.Plugins.Implicit;
using Noggog;

// Intentionally not in Implicit
namespace Mutagen.Bethesda.Plugins;

public static class Implicits
{
    public static readonly ImplicitBaseMasters BaseMasters = ImplicitBaseMasters.Instance;
    public static readonly ImplicitListings Listings = ImplicitListings.Instance;
    public static readonly ImplicitRecordFormKeys RecordFormKeys = ImplicitRecordFormKeys.Instance;

    private static readonly ImplicitRegistration Oblivion;
    private static readonly ImplicitRegistration SkyrimLE;
    private static readonly ImplicitRegistration EnderalLE;
    private static readonly ImplicitRegistration SkyrimSE;
    private static readonly ImplicitRegistration EnderalSE;
    private static readonly ImplicitRegistration SkyrimVR;
    private static readonly ImplicitRegistration Fallout4;
    private static readonly ImplicitRegistration Fallout4VR;
    private static readonly ImplicitRegistration Starfield;

    static Implicits()
    {
        #region Oblivion
        var oblivion = ModKey.FromFileName("Oblivion.esm");
        var oblivionBaseMasters = new List<ModKey>()
        {
            oblivion,
            "Knights.esp",
            "DLCShiveringIsles.esp",
            "DLCBattlehornCastle.esp",
            "DLCHorseArmor.esp",
            "DLCOrrery.esp",
            "DLCFrostcrag.esp",
            "DLCThievesDen.esp",
            "DLCMehrunesRazor.esp",
            "DLCVileLair.esp",
            "DLCSpellTomes.esp",
        };
        Oblivion = new ImplicitRegistration(
            GameRelease.Oblivion,
            BaseMasters: new ImplicitModKeyCollection(oblivionBaseMasters),
            Listings: new ImplicitModKeyCollection(oblivion.AsEnumerable()),
            RecordFormKeys: Array.Empty<FormKey>());
        #endregion

        #region Skyrim
        var skyrimModKey = ModKey.FromNameAndExtension("Skyrim.esm");
        var skyrimBaseMasters = new ImplicitModKeyCollection(new ModKey[]
        {
            skyrimModKey,
            "Update.esm",
            "Dawnguard.esm",
            "HearthFires.esm",
            "Dragonborn.esm",
        });
        var enderal = ModKey.FromFileName("Enderal - Forgotten Stories.esm");
        SkyrimLE = new ImplicitRegistration(
            GameRelease.SkyrimLE,
            BaseMasters: skyrimBaseMasters,
            Listings: skyrimBaseMasters,
            RecordFormKeys: new HashSet<FormKey>()
            {
                // Actor Value Information
                skyrimModKey.MakeFormKey(0x3F5),
                skyrimModKey.MakeFormKey(0x5E0),
                skyrimModKey.MakeFormKey(0x5E1),
                skyrimModKey.MakeFormKey(0x5E6),
                skyrimModKey.MakeFormKey(0x5EA),
                skyrimModKey.MakeFormKey(0x5EE),
                skyrimModKey.MakeFormKey(0x5EF),
                skyrimModKey.MakeFormKey(0x5FC),
                skyrimModKey.MakeFormKey(0x60B),
                skyrimModKey.MakeFormKey(0x62F),
                skyrimModKey.MakeFormKey(0x63C),
                skyrimModKey.MakeFormKey(0x644),
                skyrimModKey.MakeFormKey(0x647),
                skyrimModKey.MakeFormKey(0x648),
                skyrimModKey.MakeFormKey(0x649),

                // Body Part Data
                skyrimModKey.MakeFormKey(0x1C),

                // Eyes
                skyrimModKey.MakeFormKey(0x1A),

                // Globals
                skyrimModKey.MakeFormKey(0x63),

                // Image Space Adapter
                skyrimModKey.MakeFormKey(0x164),
                skyrimModKey.MakeFormKey(0x166),

                // Impact Data Set
                skyrimModKey.MakeFormKey(0x276),

                // Player Reference
                skyrimModKey.MakeFormKey(0x14),

                // Texture Set
                skyrimModKey.MakeFormKey(0x28),
            });
        EnderalLE = SkyrimLE with { BaseMasters = new ImplicitModKeyCollection(SkyrimLE.Listings.And(enderal)) };
        SkyrimSE = SkyrimLE with { GameRelease = GameRelease.SkyrimSE };
        EnderalSE = SkyrimSE with { BaseMasters = new ImplicitModKeyCollection(SkyrimSE.Listings.And(enderal)) };
        SkyrimVR = SkyrimSE with
        {
            GameRelease = GameRelease.SkyrimVR,
            BaseMasters = new ImplicitModKeyCollection(SkyrimSE.BaseMasters.And("SkyrimVR.esm")),
            Listings = new ImplicitModKeyCollection(SkyrimSE.Listings.And("SkyrimVR.esm")),
        };
        #endregion

        #region Fallout4
        var falloutBaseMasters = new HashSet<ModKey>()
        {
            "Fallout4.esm",
            "DLCRobot.esm",
            "DLCworkshop01.esm",
            "DLCCoast.esm",
            "DLCworkshop02.esm",
            "DLCworkshop03.esm",
            "DLCNukaWorld.esm",
        };
        Fallout4 = new ImplicitRegistration(
            GameRelease.Fallout4,
            BaseMasters: new ImplicitModKeyCollection(falloutBaseMasters),
            Listings: new ImplicitModKeyCollection(falloutBaseMasters),
            RecordFormKeys: new HashSet<FormKey>()
            {
                // ToDo
            });
        Fallout4VR = Fallout4 with
        {
            GameRelease = GameRelease.Fallout4VR,
            BaseMasters = new ImplicitModKeyCollection(Fallout4.BaseMasters.And("Fallout4_VR.esm")),
            Listings = new ImplicitModKeyCollection(Fallout4.Listings.And("Fallout4_VR.esm")),
        };
        #endregion

        #region Starfield
        var starfieldBaseMasters = new HashSet<ModKey>()
        {
            "Starfield.esm",
        };
        Starfield = new ImplicitRegistration(
            GameRelease.Starfield,
            BaseMasters: new ImplicitModKeyCollection(starfieldBaseMasters),
            Listings: new ImplicitModKeyCollection(starfieldBaseMasters),
            RecordFormKeys: new HashSet<FormKey>()
            {
                // ToDo
            });
        #endregion
    }

    public static ImplicitRegistration Get(GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => Oblivion,
            GameRelease.SkyrimLE => SkyrimLE,
            GameRelease.EnderalLE => EnderalLE,
            GameRelease.SkyrimSE => SkyrimSE,
            GameRelease.SkyrimSEGog => SkyrimSE,
            GameRelease.EnderalSE => EnderalSE,
            GameRelease.SkyrimVR => SkyrimVR,
            GameRelease.Fallout4 => Fallout4,
            GameRelease.Fallout4VR => Fallout4,
            GameRelease.Starfield => Starfield,
            _ => throw new NotImplementedException(),
        };
    }
}