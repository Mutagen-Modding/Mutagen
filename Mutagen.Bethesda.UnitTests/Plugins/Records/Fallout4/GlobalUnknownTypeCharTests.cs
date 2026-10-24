using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class GlobalUnknownTypeCharTests
{
    private static void Write(IFileSystem fileSystem, ModPath modPath, Fallout4Mod mod)
    {
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .NoModKeySync()
            .WithFileSystem(fileSystem)
            .Write();
    }

    private static void ShouldRereadAsAnUnknownGlobal(
        IFileSystem fileSystem,
        ModPath modPath,
        Fallout4Mod mod,
        char typeChar,
        float data)
    {
        mod.Globals.Add(new GlobalUnknown(mod)
        {
            TypeChar = typeChar,
            Data = data,
        });
        Write(fileSystem, modPath, mod);

        using var overlay = Fallout4Mod.Create(mod.Fallout4Release)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();
        var mutable = Fallout4Mod.Create(mod.Fallout4Release)
            .FromPath(modPath)
            .Mutable()
            .WithFileSystem(fileSystem)
            .Construct();

        ShouldBeUnknown(overlay.Globals.ShouldHaveSingleItem());
        ShouldBeUnknown(mutable.Globals.ShouldHaveSingleItem());

        void ShouldBeUnknown(IGlobalGetter global)
        {
            global.TypeChar.ShouldBe(typeChar);
            global.ShouldBeAssignableTo<IGlobalUnknownGetter>().ShouldNotBeNull().Data.ShouldBe(data);
        }
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void UnrecognizedTypeChar_RereadsAsAnUnknownGlobal(
        IFileSystem fileSystem,
        ModPath modPath,
        Fallout4Mod mod,
        float data)
    {
        ShouldRereadAsAnUnknownGlobal(fileSystem, modPath, mod, 'x', data);
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void NullTypeChar_RereadsAsAnUnknownGlobal(
        IFileSystem fileSystem,
        ModPath modPath,
        Fallout4Mod mod,
        float data)
    {
        ShouldRereadAsAnUnknownGlobal(fileSystem, modPath, mod, '\0', data);
    }

    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void NoTypeChar_RereadsAsAFloatGlobal(
        IFileSystem fileSystem,
        ModPath modPath,
        Fallout4Mod mod,
        float data)
    {
        mod.Globals.Add(new GlobalFloat(mod)
        {
            Data = data,
        });
        Write(fileSystem, modPath, mod);

        using var overlay = Fallout4Mod.Create(mod.Fallout4Release)
            .FromPath(modPath)
            .WithFileSystem(fileSystem)
            .Construct();

        var global = overlay.Globals.ShouldHaveSingleItem().ShouldBeAssignableTo<IGlobalFloatGetter>().ShouldNotBeNull();
        global.OutputChar.ShouldBeFalse();
        global.Data.ShouldBe(data);
    }
}
