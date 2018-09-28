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
        public static async Task ModList_Test(
            TestingSettings testingSettings,
            List<ModKey> loadOrder,
            Func<ILink, bool> unlinkedIgnoreTest = null)
        {
            ModList<OblivionMod> modList = new ModList<OblivionMod>();
            FormIDLinkTesterHelper.Active = true;
            await modList.Import(
                dataFolder: testingSettings.DataFolder,
                loadOrder: loadOrder,
                importer: async (filePath) => TryGet<OblivionMod>.Succeed(OblivionMod.Create_Binary(filePath.Path)));

            Assert.NotNull(FormIDLinkTesterHelper.CreatedLinks);
            Assert.DoesNotContain(FormIDLinkTesterHelper.CreatedLinks, l => l == null);

            var failedLinks = FormIDLinkTesterHelper.CreatedLinks
                .Where(l => !l.AttemptedLink)
                .ToArray();

            Assert.Empty(failedLinks);

            var unlinked = FormIDLinkTesterHelper.CreatedLinks
                .Where(l => !l.Linked)
                .Where(l => l.FormID != FormID.NULL)
                .Where(l => (!unlinkedIgnoreTest?.Invoke(l)) ?? true)
                .ToArray();

            Assert.Empty(unlinked);
        }

        public static async Task Oblivion_Modlist(TestingSettings testingSettings)
        {
            List<ModKey> loadOrder = new List<ModKey>()
            {
                new ModKey("Oblivion", master: true),
                new ModKey("Knights", master: false)
            };
            Dictionary<Type, HashSet<FormID>> knownUnlinked = new Dictionary<Type, HashSet<FormID>>();
            knownUnlinked.TryCreateValue(typeof(Region)).Add(FormID.Factory(0x00078856));
            await ModList_Test(
                testingSettings,
                loadOrder,
                unlinkedIgnoreTest: (link) =>
                {
                    if (link.FormID == Mutagen.Bethesda.Oblivion.Constants.Player) return true;
                    if (knownUnlinked.TryGetValue(link.TargetType, out var formIDs)
                        && formIDs.Contains(link.FormID))
                    {
                        return true;
                    }
                    return false;
                });
        }
    }
}
