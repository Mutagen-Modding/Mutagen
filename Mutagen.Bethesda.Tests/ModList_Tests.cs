using Mutagen.Bethesda.Oblivion;
using Noggog.Utility;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public static class ModList_Tests
    {
        public static async Task Oblivion_Modlist(TestingSettings testingSettings)
        {
            List<ModKey> loadOrder = new List<ModKey>()
            {
                new ModKey("Oblivion", master: true),
                new ModKey("Knights", master: false)
            };
            ModList<OblivionMod> modList = new ModList<OblivionMod>();
            await modList.Import(
                dataFolder: testingSettings.DataFolder,
                loadOrder: loadOrder,
                importer: async (filePath) => TryGet<OblivionMod>.Succeed(OblivionMod.Create_Binary(filePath.Path)));
        }
    }
}
