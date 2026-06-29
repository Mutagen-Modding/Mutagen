using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout3;

public partial class PackageLocation
{
    public APackageLocation Location { get; set; } = new PackageLocationFallback();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IAPackageLocationGetter IPackageLocationGetter.Location => Location;
}

partial class PackageLocationBinaryCreateTranslation
{
    public static partial void FillBinaryLocationCustom(MutagenFrame frame, IPackageLocation item)
    {
        item.Location = APackageLocation.Create(frame);
    }
}

partial class PackageLocationBinaryWriteTranslation
{
    public static partial void WriteBinaryLocationCustom(MutagenWriter writer, IPackageLocationGetter item)
    {
        APackageLocation.Write(writer, item.Location);
    }
}

partial class PackageLocationBinaryOverlay
{
    public partial IAPackageLocationGetter GetLocationCustom(int location);
    public IAPackageLocationGetter Location => GetLocationCustom(location: 0x0);

    public partial IAPackageLocationGetter GetLocationCustom(int location)
    {
        return APackageLocation.Create(
            new MutagenFrame(
                new MutagenMemoryReadStream(_structData.Slice(location), _package.MetaData)));
    }
}
