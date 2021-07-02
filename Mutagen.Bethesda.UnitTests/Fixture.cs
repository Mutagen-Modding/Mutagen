using System;
using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments;
using Noggog.Reactive;
using NSubstitute;

namespace Mutagen.Bethesda.UnitTests
{
    public class Fixture : IDisposable
    {
        public ISpecimenBuilder Inject { get; }

        public Fixture()
        {
            var fixture = new AutoFixture.Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var scheduler = Substitute.For<ISchedulerProvider>();
            scheduler.TaskPool.Returns(Scheduler.CurrentThread);
            fixture.Register(() => scheduler);
            fixture.Register(() => new DataDirectoryInjection("C:/SomeFolder"));
            Inject = fixture;
        }

        public void Dispose()
        {
        }
    }
}