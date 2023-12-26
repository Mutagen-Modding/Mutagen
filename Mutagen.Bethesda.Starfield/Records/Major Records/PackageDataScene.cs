using System.Diagnostics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;

namespace Mutagen.Bethesda.Starfield;

partial class PackageDataSceneBinaryOverlay
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string INamedRequiredGetter.Name => this.Name ?? string.Empty;
    
    public IFormLinkNullableGetter<ISceneGetter> Scene => throw new NotImplementedException();
}