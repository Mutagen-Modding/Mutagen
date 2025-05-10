using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public interface IHasVoiceType : IHasVoiceTypeGetter, ISkyrimMajorRecordInternal
{
    IFormLinkNullable<IVoiceTypeGetter> Voice { get; }
}

/// <summary>
/// Common interface for all records that have a voice type.
/// </summary>
public interface IHasVoiceTypeGetter : ISkyrimMajorRecordGetter
{
    IFormLinkNullableGetter<IVoiceTypeGetter> Voice { get; }
}
