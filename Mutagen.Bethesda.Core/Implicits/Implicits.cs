using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public record ImplicitRegistration(
        GameRelease GameRelease,
        ICollection<ModKey> BaseMasters,
        ICollection<ModKey> Listings,
        ICollection<FormKey> RecordFormKeys);

    public static class Implicits
    {
        public static readonly ImplicitBaseMasters BaseMasters = ImplicitBaseMasters.Instance;
        public static readonly ImplicitListings Listings = ImplicitListings.Instance;
        public static readonly ImplicitRecordFormKeys RecordFormKeys = ImplicitRecordFormKeys.Instance;

        private readonly static ImplicitRegistration Oblivion;
        private readonly static ImplicitRegistration SkyrimLE;
        private readonly static ImplicitRegistration SkyrimSE;
        private readonly static ImplicitRegistration SkyrimVR;
        private readonly static ImplicitRegistration Fallout4;

        static Implicits()
        {
            #region Oblivion
            var oblivionBaseMasters = new HashSet<ModKey>()
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
               BaseMasters: oblivionBaseMasters,
               Listings: oblivionBaseMasters,
               RecordFormKeys: Array.Empty<FormKey>());
            #endregion

            #region Skyrim
            var skyrimModKey = ModKey.FromNameAndExtension("Skyrim.esm");
            var skyrimBaseMasters = new HashSet<ModKey>()
            {
                skyrimModKey,
                "Update.esm",
                "Dawnguard.esm",
                "HearthFires.esm",
                "Dragonborn.esm",
            };
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
            SkyrimSE = SkyrimLE with { GameRelease = GameRelease.SkyrimSE };
            SkyrimVR = SkyrimSE with
            {
                GameRelease = GameRelease.SkyrimVR,
                BaseMasters = new HashSet<ModKey>(SkyrimSE.BaseMasters.And("SkyrimVR.esm")),
                Listings = new HashSet<ModKey>(SkyrimSE.Listings.And("SkyrimVR.esm")),
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
               BaseMasters: falloutBaseMasters,
               Listings: falloutBaseMasters,
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
                GameRelease.SkyrimSE => SkyrimSE,
                GameRelease.SkyrimVR => SkyrimVR,
                GameRelease.Fallout4 => Fallout4,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
