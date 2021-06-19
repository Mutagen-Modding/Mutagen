using System;
using System.Reactive.Concurrency;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
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
            Inject = fixture;
        }

        public void Dispose()
        {
        }
    }
}