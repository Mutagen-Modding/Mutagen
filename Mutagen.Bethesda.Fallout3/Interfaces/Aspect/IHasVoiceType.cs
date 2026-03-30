using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public interface IHasVoiceType : IHasVoiceTypeGetter, IFallout3MajorRecordInternal
{
    IFormLinkNullable<IVoiceTypeGetter> Voice { get; }
}

public interface IHasVoiceTypeGetter : IFallout3MajorRecordGetter
{
    IFormLinkNullableGetter<IVoiceTypeGetter> Voice { get; }
}
