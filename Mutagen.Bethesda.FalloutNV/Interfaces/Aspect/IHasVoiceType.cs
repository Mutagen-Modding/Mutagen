using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FalloutNV;

/// <summary>
/// Common interface for all records that have a voice type.
/// </summary>
public interface IHasVoiceType : IHasVoiceTypeGetter, IFalloutNVMajorRecordInternal
{
    IFormLinkNullable<IVoiceTypeGetter> Voice { get; }
}

/// <summary>
/// Common interface for all records that have a voice type.
/// </summary>
public interface IHasVoiceTypeGetter : IFalloutNVMajorRecordGetter
{
    IFormLinkNullableGetter<IVoiceTypeGetter> Voice { get; }
}
