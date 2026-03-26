using System;

namespace Mutagen.Bethesda.Fallout76;

partial class ObjectModification
{
    public enum ModFormType
    {
        Armor = 0,
        NonPlayerCharacter = 1,
        Weapon = 2,
        MovableStatic = 3,
        None = 4,
    }
}

partial class ObjectModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<AObjectModification.NoneProperty>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<AObjectModification.NoneProperty>>();
}
