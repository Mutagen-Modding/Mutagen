using Autofac;

namespace Mutagen.Bethesda.Generation.Generator;

public class GeneratorAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(
                typeof(IGenerationConstructor).Assembly)
            .AsSelf()
            .AsImplementedInterfaces();
    }
}
