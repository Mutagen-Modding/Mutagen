using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.StructureMap;
using Mutagen.Bethesda.WPF.Plugins.Order.Implementations;
using StructureMap;

namespace Mutagen.Bethesda.WPF.TestDisplay
{
    public class Register : Registry
    {
        public Register()
        {
            IncludeRegistry<MutagenRegister>();
            For<IFileSystem>().Use<FileSystem>();
            For<MainVM>();
            For<FileSyncedLoadOrderVM>();
            For<IGameReleaseContext>().Use(x => new GameReleaseInjection(GameRelease.SkyrimSE)).Singleton();
        }
    }
}