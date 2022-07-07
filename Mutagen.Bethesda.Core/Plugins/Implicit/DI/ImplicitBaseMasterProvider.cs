using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Implicit.DI;

public interface IImplicitBaseMasterProvider
{
    IReadOnlyList<ModKey> BaseMasters { get; }
}

public sealed class ImplicitBaseMasterProvider : IImplicitBaseMasterProvider
{
    private readonly IGameReleaseContext _gameRelease;

    public ImplicitBaseMasterProvider(
        IGameReleaseContext gameRelease)
    {
        _gameRelease = gameRelease;
    }
        
    public IReadOnlyList<ModKey> BaseMasters => Implicits.Get(_gameRelease.Release).BaseMasters;
}