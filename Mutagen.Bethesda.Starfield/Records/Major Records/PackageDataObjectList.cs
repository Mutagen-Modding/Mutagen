using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Aspects;

namespace Mutagen.Bethesda.Starfield;

partial class PackageDataObjectListBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string INamedRequiredGetter.Name => this.Name ?? string.Empty;
    
    public float? Data => throw new NotImplementedException();
}