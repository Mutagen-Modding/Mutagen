using System.IO.Abstractions;
using Autofac;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Testing.Fakes;
using Noggog.Autofac;

namespace Mutagen.Bethesda.Testing;

public class MutagenTestModule : Module
{
    private readonly GameRelease _release;
    private readonly IFileSystem? _fileSystem;
    
    public MutagenTestModule(
        GameRelease release,
        IFileSystem? fileSystem)
    {
        _release = release;
        _fileSystem = fileSystem;
    }
    
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<MutagenModule>();
        builder.RegisterInstance(new GameReleaseInjection(_release))
            .AsImplementedInterfaces();
        if (_fileSystem != null)
        {
            builder.RegisterInstance(_fileSystem)
                .AsImplementedInterfaces();
        }
        
        builder.RegisterAssemblyTypes(typeof(ManualLoadOrderProvider).Assembly)
            .InNamespacesOf(
                typeof(ManualLoadOrderProvider))
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}