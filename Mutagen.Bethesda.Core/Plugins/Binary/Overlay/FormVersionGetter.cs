using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal sealed class FormVersionGetter : IFormVersionGetter
{
    public ushort? FormVersion { get; set; }
}