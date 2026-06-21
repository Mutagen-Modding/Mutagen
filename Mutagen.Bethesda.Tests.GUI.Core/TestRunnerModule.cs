using Autofac;
using Noggog.Reactive;
using Noggog.UI;

namespace Mutagen.Bethesda.Tests.GUI;

public class TestRunnerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<SchedulerProvider>().As<ISchedulerProvider>().SingleInstance();
        builder.RegisterType<MainVM>().AsSelf();
    }
}
