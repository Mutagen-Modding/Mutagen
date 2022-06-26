using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout4;

public partial class PackageDataTarget
{
    public enum Types
    {
        Target,
        SingleRef,
    }

    #region Target
    public APackageTarget Target { get; set; } = null!;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IAPackageTargetGetter IPackageDataTargetGetter.Target => Target;
    #endregion

    partial void CustomCtor()
    {
        Target = new PackageTargetObjectType();
    }

    public PackageDataTarget(APackageTarget target)
    {
        Target = target;
    }
}

partial class PackageDataTargetBinaryOverlay
{
    public PackageDataTarget.Types Type => throw new NotImplementedException();

    public IAPackageTargetGetter Target => throw new NotImplementedException();
}
