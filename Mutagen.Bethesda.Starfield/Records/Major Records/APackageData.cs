using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Aspects;

namespace Mutagen.Bethesda.Starfield;

partial class APackageData
{
    [Flags]
    public enum Flag
    {
        Public = 0x01
    }
}

partial class APackageDataBinaryOverlay
{
    public string? Name => throw new NotImplementedException();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string INamedRequiredGetter.Name => this.Name ?? string.Empty;
    
    public APackageData.Flag? Flags => throw new NotImplementedException();
}