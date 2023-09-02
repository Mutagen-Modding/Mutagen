using Noggog;

namespace Mutagen.Bethesda.Starfield;

public interface IPositionRotation : IPositionRotationGetter
{
    new P3Float Position { get; set; }
    new P3Float Rotation { get; set; }
}

public interface IPositionRotationGetter
{
    P3Float Position { get; }
    P3Float Rotation { get; }
}