namespace Mutagen.Bethesda.Environments.DI
{
    public interface IGameReleaseContext
    {
        GameRelease Release { get; }
    }

    public record GameReleaseInjection(GameRelease Release)
        : IGameReleaseContext;
}