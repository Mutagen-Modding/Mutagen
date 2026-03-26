using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76;

public interface IHasVoiceType : IHasVoiceTypeGetter, IFallout76MajorRecordInternal
{
    IFormLinkNullable<IVoiceTypeGetter> Voice { get; }
}

/// <summary>
/// Common interface for all records that have a voice type.
/// </summary>
public interface IHasVoiceTypeGetter : IFallout76MajorRecordGetter
{
    IFormLinkNullableGetter<IVoiceTypeGetter> Voice { get; }
}
