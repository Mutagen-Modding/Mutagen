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
                dataFolder: testingSettings.PassthroughSettings.DataFolder,
                loadOrder: loadOrder,
                importer: async (filePath, modKey) => TryGet<OblivionMod>.Succeed(OblivionMod.Create_Binary(filePath.Path, modKey)));

            Assert.NotNull(FormIDLinkTesterHelper.CreatedLinks);
            Assert.DoesNotContain(FormIDLinkTesterHelper.CreatedLinks, l => l == null);

#if DEBUG
            var failedLinks = FormIDLinkTesterHelper.CreatedLinks
                .Where(l => !l.AttemptedLink)
                .ToArray();

            Assert.Empty(failedLinks);
#endif

            var unlinked = FormIDLinkTesterHelper.CreatedLinks
                .Where(l => !l.Linked)
                .Where(l => l.FormKey != FormKey.NULL)
                .Where(l => (!unlinkedIgnoreTest?.Invoke(l)) ?? true)
                .ToArray();

            Assert.Empty(unlinked);
        }

        public static async Task Oblivion_Modlist(TestingSettings testingSettings)
        {
            var obliv = new ModKey("Oblivion", master: true);
            var knights = new ModKey("Knights", master: false);
            List<ModKey> loadOrder = new List<ModKey>()
            {
                obliv,
                knights,
            };
            Dictionary<Type, HashSet<FormKey>> knownUnlinked = new Dictionary<Type, HashSet<FormKey>>();
            knownUnlinked.TryCreateValue(typeof(Region)).Add(new FormKey(obliv, 0x078856));
            knownUnlinked.TryCreateValue(typeof(Script)).Add(new FormKey(knights, 0x020AE4));
            knownUnlinked.TryCreateValue(typeof(Script)).Add(new FormKey(knights, 0x020AE2));
            knownUnlinked.TryCreateValue(typeof(Weather)).Add(new FormKey(knights, 0x010B5F));
            await ModList_Test(
                testingSettings,
                loadOrder,
                unlinkedIgnoreTest: (link) =>
                {
                    if (link.FormKey == Mutagen.Bethesda.Oblivion.Constants.Player) return true;
                    if (knownUnlinked.TryGetValue(link.TargetType, out var formIDs)
                        && formIDs.Contains(link.FormKey))
                    {
                        return true;
                    }
                    return false;
                });
        }
    }
}
