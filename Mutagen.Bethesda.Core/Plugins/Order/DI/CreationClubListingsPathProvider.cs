using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface ICreationClubListingsPathProvider
    {
        /// <summary>
        /// Returns expected location of the creation club load order file
        /// </summary>
        /// <returns>Expected path to creation club load order file</returns>
        FilePath? Path { get; }
    }

    public class CreationClubListingsPathProvider : ICreationClubListingsPathProvider
    {
        private readonly IGameCategoryContext _categoryContext;
        private readonly ICreationClubEnabledProvider _isUsed;
        private readonly IDataDirectoryContext _dataDirectory;

        public CreationClubListingsPathProvider(
            IGameCategoryContext categoryContext,
            ICreationClubEnabledProvider isUsed,
            IDataDirectoryContext dataDirectory)
        {
            _categoryContext = categoryContext;
            _isUsed = isUsed;
            _dataDirectory = dataDirectory;
        }

        /// <inheritdoc />
        public FilePath? Path
        {
            get
            {
                if (_isUsed.Used)
                {
                    return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_dataDirectory.Path)!, $"{_categoryContext.Category}.ccc");
                }

                return null;
            }
        }
    }

    public record CreationClubListingsPathInjection(FilePath? Path) : ICreationClubListingsPathProvider;
}