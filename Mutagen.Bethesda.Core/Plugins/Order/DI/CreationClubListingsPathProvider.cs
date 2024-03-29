﻿using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ICreationClubListingsPathProvider
{
    /// <summary>
    /// Returns expected location of the creation club load order file
    /// </summary>
    /// <returns>Expected path to creation club load order file</returns>
    FilePath? Path { get; }
}

public sealed class CreationClubListingsPathProvider : ICreationClubListingsPathProvider
{
    public IGameCategoryContext CategoryContext { get; }
    public ICreationClubEnabledProvider IsUsed { get; }
    public IGameDirectoryProvider DirectoryProvider { get; }

    public CreationClubListingsPathProvider(
        IGameCategoryContext categoryContext,
        ICreationClubEnabledProvider isUsed,
        IGameDirectoryProvider gameDirectoryProvider)
    {
        CategoryContext = categoryContext;
        IsUsed = isUsed;
        DirectoryProvider = gameDirectoryProvider;
    }

    /// <inheritdoc />
    public FilePath? Path
    {
        get
        {
            if (IsUsed.Used)
            {
                var dir = DirectoryProvider.Path;
                if (dir == null) return null;

                return System.IO.Path.Combine(dir, $"{CategoryContext.Category}.ccc");
            }

            return null;
        }
    }
}

public record CreationClubListingsPathInjection(FilePath? Path) : ICreationClubListingsPathProvider;