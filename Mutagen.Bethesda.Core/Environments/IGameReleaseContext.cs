namespace Mutagen.Bethesda.Environments
{
    public interface IGameReleaseContext
    {
        GameRelease Release { get; }
    }

    public record GameReleaseInjection(GameRelease Release)
        : IGameReleaseContext;
}