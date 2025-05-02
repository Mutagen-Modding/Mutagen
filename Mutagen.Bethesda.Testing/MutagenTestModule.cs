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
    private readonly Module[] _modules;

    public MutagenTestModule(
        GameRelease release,
        IFileSystem? fileSystem,
        Module[] modules)
    {
        _release = release;
        _fileSystem = fileSystem;
        _modules = modules;
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
        
        foreach (var module in _modules)
        {
            builder.RegisterModule(module);
        }
    }
}