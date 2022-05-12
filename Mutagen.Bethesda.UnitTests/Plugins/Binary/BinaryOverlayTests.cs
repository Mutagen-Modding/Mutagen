using System.IO.Abstractions.TestingHelpers;
using Mutagen.Bethesda.Skyrim;
using Noggog.Testing.IO;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary;

public class BinaryOverlayTests
{
    [Fact]
    public void DisposedIfException()
    {
        var fs = new MockFileSystem();
        fs.Directory.CreateDirectory($"{PathingUtil.DrivePrefix}SomeFolder");
        var modPath = Path.Combine($"{PathingUtil.DrivePrefix}SomeFolder", "Test.esp");
        try
        {
            fs.File.WriteAllText(modPath, "DERP");
            var mod = SkyrimMod.CreateFromBinaryOverlay(modPath, SkyrimRelease.SkyrimLE, fileSystem: fs);
        }
        catch (ArgumentException)
        {
        }
        // Assert that file is released from wrapper's internal stream
        fs.File.Delete(modPath);
    }
}