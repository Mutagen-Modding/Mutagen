using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout3;

public partial class PackageTarget
{
    public APackageTarget Target { get; set; } = new PackageTargetFallback();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IAPackageTargetGetter IPackageTargetGetter.Target => Target;
}

partial class PackageTargetBinaryCreateTranslation
{
    public static partial void FillBinaryTargetCustom(MutagenFrame frame, IPackageTarget item)
    {
        item.Target = APackageTarget.Create(frame);
    }
}

partial class PackageTargetBinaryWriteTranslation
{
    public static partial void WriteBinaryTargetCustom(MutagenWriter writer, IPackageTargetGetter item)
    {
        APackageTarget.Write(writer, item.Target);
    }
}

partial class PackageTargetBinaryOverlay
{
    public partial IAPackageTargetGetter GetTargetCustom(int location);
    public IAPackageTargetGetter Target => GetTargetCustom(location: 0x0);

    public partial IAPackageTargetGetter GetTargetCustom(int location)
    {
        return APackageTarget.Create(
            new MutagenFrame(
                new MutagenMemoryReadStream(_structData.Slice(location), _package.MetaData)));
    }
}
