using System;

namespace Mutagen.Bethesda.Plugins.Aspects
{
    public interface IWeightValue : IWeightValueGetter
    {
        new uint Value { get; set; }
        new float Weight { get; set; }
    }

    public interface IWeightValueGetter
    {
        uint Value { get; }
        float Weight { get; }
    }
}
