using System.IO.Abstractions;
using Autofac;
using AutoFixture;
using Mutagen.Bethesda.Environments.DI;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MutagenContainerCustomization : ICustomization
{
    private readonly GameRelease _release;
    private readonly Module[] _modules;

    public MutagenContainerCustomization(
        GameRelease release,
        params Module[] modules)
    {
        _release = release;
        _modules = modules;
    }

    public void Customize(IFixture fixture)
    {
        fixture.Customize(new ContainerAutoDataCustomization(
            new MutagenTestModule(
                _release,
                fixture.Create<IFileSystem>(),
                [
                    new EnvironmentModule(
                        fixture.Create<IGameDirectoryProvider>(),
                        fixture.Create<IDataDirectoryProvider>()),
                    .._modules
                ])));
    }

    private class EnvironmentModule : Module
    {
        private readonly IGameDirectoryProvider _gameDirectory;
        private readonly IDataDirectoryProvider _dataDirectory;

        public EnvironmentModule(
            IGameDirectoryProvider gameDirectory,
            IDataDirectoryProvider dataDirectory)
        {
            _gameDirectory = gameDirectory;
            _dataDirectory = dataDirectory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_gameDirectory).As<IGameDirectoryProvider>();
            builder.RegisterInstance(_dataDirectory).As<IDataDirectoryProvider>();
        }
    }
}
