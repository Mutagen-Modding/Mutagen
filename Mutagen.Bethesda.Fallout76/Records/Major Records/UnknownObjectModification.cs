using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Fallout76;

partial class UnknownObjectModificationBinaryOverlay
{
    public RecordType ModificationType { get; set; }
    public IReadOnlyList<IAObjectModPropertyGetter<AObjectModification.NoneProperty>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<AObjectModification.NoneProperty>>();
}
