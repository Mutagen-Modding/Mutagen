using Autofac;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Masters.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog.Autofac;
using Module = Autofac.Module;

namespace Mutagen.Bethesda.Autofac;

public class MutagenModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IArchiveReaderProvider).Assembly)
            .InNamespacesOf(
                typeof(IArchiveReaderProvider),
                typeof(IDataDirectoryLookup),
                typeof(IImplicitBaseMasterProvider),
                typeof(ILoadOrderWriter),
                typeof(IModActivator),
                typeof(IGameInstallProvider),
                typeof(IMasterReferenceReaderFactory))
            .NotInjection()
            .AsMatchingInterface();
        builder.RegisterGeneric(typeof(GameEnvironmentProvider<>))
            .As(typeof(IGameEnvironmentProvider<>));
        builder.RegisterGeneric(typeof(GameEnvironmentProvider<,>))
            .As(typeof(IGameEnvironmentProvider<,>));
        builder.RegisterGeneric(typeof(LoadOrderImporter<>))
            .As(typeof(ILoadOrderImporter<>));
        builder.RegisterGeneric(typeof(ModImporter<>))
            .As(typeof(IModImporter<>));
        builder.RegisterType<GameLocator>()
            .As<IGameDirectoryLookup>()
            .As<IDataDirectoryLookup>()
            .As<IGameInstallProvider>();
    }
}