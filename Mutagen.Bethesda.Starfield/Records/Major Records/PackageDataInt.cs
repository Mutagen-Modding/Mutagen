using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Aspects;

namespace Mutagen.Bethesda.Starfield;

partial class PackageDataIntBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string INamedRequiredGetter.Name => this.Name ?? string.Empty;
    
    public uint Data => throw new NotImplementedException();
}