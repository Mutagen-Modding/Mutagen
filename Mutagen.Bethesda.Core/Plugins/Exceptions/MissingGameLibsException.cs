namespace Mutagen.Bethesda.Plugins.Exceptions;

public class MissingGameLibsException : Exception
{
    public GameCategory Category { get; }

    public MissingGameLibsException(GameCategory category)
    {
        Category = category;
    }
}
