using System.IO.Abstractions;
using Autofac;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Environments.DI;
using Noggog.Autofac;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class ContainerTests
    {
        [Fact]
        public void EnsureAutofacValid()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MutagenModule>();
            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterType<GameReleasePlaceholder>().As<IGameReleaseContext>();
            var cont = builder.Build();
            cont.ValidateRegistrations(evaluateUsages: false);
        }
    }
}