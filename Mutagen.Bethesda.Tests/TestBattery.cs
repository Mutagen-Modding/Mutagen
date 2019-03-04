using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public static class TestBattery
    {
        public static Task RunTests(TestingSettings settings)
        {
            return Task.WhenAll(
                GetTests(settings: settings)
                .Select((t) => Task.Run(() => t)));
        }

        public static IEnumerable<Task> GetTests(TestingSettings settings)
        {
            var oblivPassthrough = new Oblivion_Passthrough_Test(settings.PassthroughSettings, settings.OblivionESM);
            if ((settings.PassthroughSettings?.TestNormal ?? false)
                || (settings.PassthroughSettings?.TestObservable ?? false))
            {
                if (settings.OblivionESM.Do)
                {
                    yield return oblivPassthrough.BinaryPassthroughTest();
                }
                foreach (var passthrough in settings.OtherPassthroughsEnumerable)
                {
                    if (!passthrough.Do) continue;
                    yield return new Oblivion_Passthrough_Test(settings.PassthroughSettings, passthrough).BinaryPassthroughTest();
                }
            }

            if (settings.TestGroupMasks)
            {
                yield return OblivionESM_Passthrough_Tests.OblivionESM_GroupMask_Import(settings.PassthroughSettings, settings.OblivionESM);
                yield return OblivionESM_Passthrough_Tests.OblivionESM_GroupMask_Export(settings.PassthroughSettings, settings.OblivionESM);
            }
            if (settings.PassthroughSettings?.TestFolder ?? false)
            {
                yield return OblivionESM_Passthrough_Tests.OblivionESM_Folder_Reimport(settings.PassthroughSettings, settings.OblivionESM, oblivPassthrough);
            }
            if (settings.TestModList)
            {
                yield return ModList_Tests.Oblivion_Modlist(settings);
            }
            if (settings.TestFlattenedMod)
            {
                yield return FlattenedMod_Tests.Oblivion_FlattenMod(settings);
            }
        }
    }
}
