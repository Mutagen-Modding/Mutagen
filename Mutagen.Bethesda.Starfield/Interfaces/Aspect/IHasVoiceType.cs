using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

public interface IHasVoiceType : IHasVoiceTypeGetter, IStarfieldMajorRecordInternal
{
    IFormLinkNullable<IVoiceTypeGetter> Voice { get; }
}

/// <summary>
/// Common interface for all records that have a voice type.
/// </summary>
public interface IHasVoiceTypeGetter : IStarfieldMajorRecordGetter
{
    IFormLinkNullableGetter<IVoiceTypeGetter> Voice { get; }
}