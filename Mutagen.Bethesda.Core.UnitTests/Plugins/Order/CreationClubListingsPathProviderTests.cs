﻿using AutoFixture.Xunit2;
using Shouldly;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class CreationClubListingsPathProviderTests
{
    [Theory, MutagenAutoData]
    public void NotUsed(
        [Frozen]ICreationClubEnabledProvider enabledProvider,
        CreationClubListingsPathProvider sut)
    {
        enabledProvider.Used.Returns(false);
        sut.Path
            .ShouldBeNull();
    }

    [Theory, MutagenAutoData]
    public void Typical(
        [Frozen]IGameDirectoryProvider gameDir,
        [Frozen]IGameCategoryContext gameCategoryContext,
        [Frozen]ICreationClubEnabledProvider enabledProvider,
        CreationClubListingsPathProvider sut)
    {
        enabledProvider.Used.Returns(true);
        foreach (var category in Enums<GameCategory>.Values)
        {
            gameCategoryContext.Category.Returns(category);
            sut.Path
                .ShouldBe(new FilePath(Path.Combine(gameDir.Path!, $"{category}.ccc")));
        }
    }
}