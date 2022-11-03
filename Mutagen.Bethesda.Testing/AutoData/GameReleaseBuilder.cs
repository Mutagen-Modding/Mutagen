using AutoFixture.Kernel;
using Noggog;

namespace Mutagen.Bethesda.Testing.AutoData;

public class GameReleaseBuilder : ISpecimenBuilder
{
    private readonly GameRelease _release;

    public GameReleaseBuilder(GameRelease release)
    {
        _release = release;
    }
        
    public object Create(object request, ISpecimenContext context)
    {
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }

        if (request is not Type t) return new NoSpecimen();
        if (t == typeof(GameRelease))
        {
            return _release;
        }
        if (t == typeof(GameCategory))
        {
            return _release.ToCategory();
        }
        
        if (t.Name.EndsWith("Release"))
        {
            if (Enum.TryParse<GameCategory>(t.Name.TrimEnd("Release"), out var cata))
            {
                switch (cata)
                {
                    case GameCategory.Skyrim:
                        return Enum.Parse(t, "SkyrimSE");
                    case GameCategory.Fallout4:
                    case GameCategory.Oblivion:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        return new NoSpecimen();
    }
}