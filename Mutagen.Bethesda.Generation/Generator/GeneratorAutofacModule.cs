using Autofac;
using Noggog.Autofac;
using Noggog.Autofac.Modules;

namespace Mutagen.Bethesda.Generation.Generator;

public class GeneratorAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.WithTypicalFilesystem();
        builder.RegisterModule<NoggogModule>();
        builder.RegisterAssemblyTypes(
                typeof(IGenerationConstructor).Assembly)
            .AsSelf()
            .AsImplementedInterfaces();
    }
}
