using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

/// <summary>
/// Common interface for all records that have a voice type.
/// </summary>
public interface IHasVoiceType : IHasVoiceTypeGetter, IFallout3MajorRecordInternal
{
    IFormLinkNullable<IVoiceTypeGetter> Voice { get; }
}

/// <summary>
/// Common interface for all records that have a voice type.
/// </summary>
public interface IHasVoiceTypeGetter : IFallout3MajorRecordGetter
{
    IFormLinkNullableGetter<IVoiceTypeGetter> Voice { get; }
}
