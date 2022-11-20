using Mutagen.Bethesda.Inis;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class EnumCompletenessTests
{
    #region GameRelease
    [Fact]
    public void ToCategoryCoverage()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            release.ToCategory();
        }
    }

    [Fact]
    public void HasEnabledMarkers()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            PluginListings.HasEnabledMarkers(release);
        }
    }

    [Fact]
    public void GameConstants()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            Mutagen.Bethesda.Plugins.Meta.GameConstants.Get(release);
        }
    }

    [Fact]

    public void DefaultFormVersion()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            release.GetDefaultFormVersion();
        }
    }
    #endregion

    #region GameCategory
    [Fact]
    public void HasFormVersion()
    {
        foreach (var cat in Enums<GameCategory>.Values)
        {
            cat.HasFormVersion();
        }
    }
    #endregion

    #region MyDocumentsString
    [Fact]
    public void MyDocumentsString()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            IniPathLookup.ToMyDocumentsString(release);
        }
    }
    #endregion

    #region ToIniName
    [Fact]
    public void ToIniName()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            IniPathLookup.ToIniName(release);
        }
    }
    #endregion
}