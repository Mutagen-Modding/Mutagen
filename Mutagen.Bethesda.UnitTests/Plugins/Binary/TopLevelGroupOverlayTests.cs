using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Shouldly;
using Xunit;
using TempFile = Noggog.IO.TempFile;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary;

public class TopLevelGroupOverlayTests
{
    [Fact]
    public void Overlay_ATopLevelGroupReadTwice_IsTheSameGroup()
    {
        using var tmp = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath, suffix: ".esp");
        var mod = new Fallout4Mod(ModKey.FromNameAndExtension("TopLevelGroupOverlay.esp"), Fallout4Release.Fallout4);
        mod.Globals.Add(new GlobalFloat(mod.GetNextFormKey(), Fallout4Release.Fallout4));
        var path = new ModPath(mod.ModKey, tmp.File.Path);
        mod.BeginWrite.ToPath(path).WithNoLoadOrder().NoModKeySync().Write();

        using var overlay = Fallout4Mod.CreateFromBinaryOverlay(path, Fallout4Release.Fallout4);
        var first = overlay.Globals;

        overlay.Globals.ShouldBeSameAs(first);
    }
}
