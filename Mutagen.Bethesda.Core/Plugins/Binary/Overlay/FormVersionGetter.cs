using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay
{
    public class FormVersionGetter : IFormVersionGetter
    {
        public ushort? FormVersion { get; set; }
    }
}
