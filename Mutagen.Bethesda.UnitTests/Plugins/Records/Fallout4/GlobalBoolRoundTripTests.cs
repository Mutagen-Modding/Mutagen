using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Shouldly;
using Xunit;
using TempFile = Noggog.IO.TempFile;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

// Regression coverage for https://github.com/Mutagen-Modding/Mutagen/issues/538:
// GlobalBool's binary writer wrote its FLTV subrecord as a raw 1-byte bool instead
// of the 4-byte float every other Global subtype (and both readers) expect, so a
// GlobalBool written by Mutagen's own writer could not be read back by Mutagen's
// own reader.
public class GlobalBoolRoundTripTests
{
    private static Fallout4Mod WriteModWithGlobalBool(TempFile tmp, bool value, out FormKey formKey)
    {
        var mod = new Fallout4Mod(ModKey.FromNameAndExtension("GlobalBoolRoundTrip.esp"), Fallout4Release.Fallout4);
        var glob = new GlobalBool(mod.GetNextFormKey(), Fallout4Release.Fallout4)
        {
            Data = value,
            EditorID = "GlobalBoolRoundTrip",
        };
        mod.Globals.Add(glob);
        formKey = glob.FormKey;

        var path = new ModPath(mod.ModKey, tmp.File.Path);
        mod.BeginWrite
            .ToPath(path)
            .WithNoLoadOrder()
            .NoModKeySync()
            .Write();
        return mod;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WriteThenReadBinary_RoundTripsData(bool value)
    {
        using var tmp = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath, suffix: ".esp");
        var written = WriteModWithGlobalBool(tmp, value, out var formKey);
        var path = new ModPath(written.ModKey, tmp.File.Path);

        var reloaded = Fallout4Mod.CreateFromBinary(path, Fallout4Release.Fallout4);
        var reloadedGlob = (GlobalBool)reloaded.Globals.Records.First(g => g.FormKey == formKey);

        reloadedGlob.Data.ShouldBe(value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WriteThenReadBinaryOverlay_RoundTripsData(bool value)
    {
        using var tmp = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath, suffix: ".esp");
        var written = WriteModWithGlobalBool(tmp, value, out var formKey);
        var path = new ModPath(written.ModKey, tmp.File.Path);

        using var reloaded = Fallout4Mod.CreateFromBinaryOverlay(path, Fallout4Release.Fallout4);
        var reloadedGlob = (IGlobalBoolGetter)reloaded.Globals.Records.First(g => g.FormKey == formKey);

        reloadedGlob.Data.ShouldBe(value);
    }
}
