using System;
using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Records.DI
{
    public interface IModImporter
    {
        TMod Import<TMod>(ModPath modPath)
            where TMod : IModGetter;

        IModGetter Import(ModPath modPath);
    }
    
    public interface IModImporter<TMod>
        where TMod : IModGetter
    {
        TMod Import(ModPath modPath);
    }

    public class ModImporter : IModImporter
    {
        private readonly IFileSystem _fileSystem;
        private readonly IGameReleaseContext _gameRelease;

        public ModImporter(
            IFileSystem fileSystem,
            IGameReleaseContext gameRelease)
        {
            _fileSystem = fileSystem;
            _gameRelease = gameRelease;
        }

        public TMod Import<TMod>(ModPath modPath)
            where TMod : IModGetter
        {
            return ModInstantiator<TMod>.Importer(modPath, _gameRelease.Release, _fileSystem);
        }

        public IModGetter Import(ModPath modPath)
        {
            return ModInstantiator.Importer(modPath, _gameRelease.Release, _fileSystem);
        }
    }

    public class ModImporter<TMod> : IModImporter<TMod>
        where TMod : IModGetter
    {
        private readonly IFileSystem _fileSystem;
        private readonly IGameReleaseContext _gameRelease;

        public ModImporter(
            IFileSystem fileSystem,
            IGameReleaseContext gameRelease)
        {
            _fileSystem = fileSystem;
            _gameRelease = gameRelease;
        }

        public TMod Import(ModPath modPath)
        {
            return ModInstantiator<TMod>.Importer(modPath, _gameRelease.Release, _fileSystem);
        }
    }

    public class ModImporterWrapper<TMod> : IModImporter<TMod>
        where TMod : IModGetter
    {
        private readonly Func<ModPath, TMod> _factory;

        public ModImporterWrapper(Func<ModPath, TMod> factory)
        {
            _factory = factory;
        }

        public TMod Import(ModPath modPath) => _factory(modPath);
    }
}