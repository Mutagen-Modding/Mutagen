using Mutagen.Bethesda.Plugins.Implicit;
using Noggog;
using System;
using System.Collections.Generic;

// Intentionally not in Implicit
namespace Mutagen.Bethesda.Plugins
{
    public static class Implicits
    {
        public static readonly ImplicitBaseMasters BaseMasters = ImplicitBaseMasters.Instance;
        public static readonly ImplicitListings Listings = ImplicitListings.Instance;
        public static readonly ImplicitRecordFormKeys RecordFormKeys = ImplicitRecordFormKeys.Instance;

        private readonly static ImplicitRegistration Oblivion;
        private readonly static ImplicitRegistration SkyrimLE;
        private readonly static ImplicitRegistration EnderalLE;
        private readonly static ImplicitRegistration SkyrimSE;
        private readonly static ImplicitRegistration EnderalSE;
        private readonly static ImplicitRegistration SkyrimVR;
        private readonly static ImplicitRegistration Fallout4;

        static Implicits()
        {
            #region Oblivion
            var oblivionBaseMasters = new List<ModKey>()
            {
                "Oblivion.esm",
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
               Listings: new ImplicitModKeyCollection(Array.Empty<ModKey>()),
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
                "Fallout4.esm"
            };
            Fallout4 = new ImplicitRegistration(
               GameRelease.Fallout4,
               BaseMasters: new ImplicitModKeyCollection(falloutBaseMasters),
               Listings: new ImplicitModKeyCollection(falloutBaseMasters),
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
                GameRelease.EnderalSE => EnderalSE,
                GameRelease.SkyrimVR => SkyrimVR,
                GameRelease.Fallout4 => Fallout4,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
