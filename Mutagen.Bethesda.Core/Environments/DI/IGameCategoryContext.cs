namespace Mutagen.Bethesda.Environments.DI;

public interface IGameCategoryContext
{
    GameCategory Category { get; }
}

public sealed class GameCategoryContext : IGameCategoryContext
{
    private readonly IGameReleaseContext _releaseContext;

    public GameCategoryContext(IGameReleaseContext releaseContext)
    {
        _releaseContext = releaseContext;
    }

    public GameCategory Category => _releaseContext.Release.ToCategory();
}

public record GameCategoryInjection(GameCategory Category) : IGameCategoryContext;