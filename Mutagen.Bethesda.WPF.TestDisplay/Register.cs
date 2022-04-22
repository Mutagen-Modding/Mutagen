using System.IO.Abstractions;
using Autofac;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.WPF.Plugins.Order.Implementations;

namespace Mutagen.Bethesda.WPF.TestDisplay;

public class MainModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<MutagenModule>();
        builder.RegisterType<FileSystem>().As<IFileSystem>();
        builder.RegisterType<MainVM>().AsSelf();
        builder.RegisterType<FileSyncedLoadOrderVM>().AsSelf();
        builder.RegisterInstance(new GameReleaseInjection(GameRelease.SkyrimSE)).As<IGameReleaseContext>();
    }
}