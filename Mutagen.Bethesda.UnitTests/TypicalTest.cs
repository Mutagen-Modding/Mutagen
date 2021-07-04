using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Noggog.Reactive;
using NSubstitute;

namespace Mutagen.Bethesda.UnitTests
{
    public class TypicalTest
    {
        public ISpecimenBuilder Fixture { get; }

        public TypicalTest()
        {
            var fixture = new AutoFixture.Fixture();
            fixture.Customize(new AutoNSubstituteCustomization()
            {
                ConfigureMembers = true
            });
            var scheduler = Substitute.For<ISchedulerProvider>();
            scheduler.TaskPool.Returns(Scheduler.CurrentThread);
            fixture.Register(() => scheduler);
            fixture.Register<IDataDirectoryProvider>(() => new DataDirectoryInjection("C:/SomeFolder"));
            Fixture = fixture;
        }
    }
}