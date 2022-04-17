using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using NSubstitute;

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
            
        return new NoSpecimen();
    }
}