using Microsoft.VisualBasic.CompilerServices;

namespace Mutagen.Bethesda.Environments.DI
{
    public interface IGameReleaseContext
    {
        GameRelease Release { get; }
    }

    public record GameReleaseInjection(GameRelease Release)
        : IGameReleaseContext;

    public class GameReleasePlaceholder : IGameReleaseContext
    {
        public GameRelease Release => throw new IncompleteInitialization();
    }
}