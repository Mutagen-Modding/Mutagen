using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

public partial class APackageLocation
{
    // Shared union read/write for the PLDT/PLD2 "Location" 8-byte slot
    // (Type itU32 discriminator + 4-byte value), per wbPxDTLocationDecider.
    public static APackageLocation Create(MutagenFrame frame)
    {
        var type = (Package.LocationType)frame.ReadInt32();
        return type switch
        {
            Package.LocationType.NearReference => new PackageLocationReference()
            {
                Reference = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<IPlacedGetter>()
            },
            Package.LocationType.InCell => new PackageLocationCell()
            {
                Cell = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<ICellGetter>()
            },
            Package.LocationType.ObjectId => new PackageLocationObjectId()
            {
                Reference = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<IPackageLocationObjectGetter>()
            },
            Package.LocationType.ObjectType => new PackageLocationObjectType()
            {
                ObjectType = (Package.ObjectType)frame.ReadInt32()
            },
            // xEdit models these branches' 4-byte value as Unused; preserve the raw bits.
            Package.LocationType.NearCurrentLocation => new PackageLocationNearCurrentLocation()
            {
                Unused = frame.ReadInt32()
            },
            Package.LocationType.NearEditorLocation => new PackageLocationNearEditorLocation()
            {
                Unused = frame.ReadInt32()
            },
            Package.LocationType.NearLinkedReference => new PackageLocationNearLinkedReference()
            {
                Unused = frame.ReadInt32()
            },
            Package.LocationType.AtPackageLocation => new PackageLocationAtPackageLocation()
            {
                Unused = frame.ReadInt32()
            },
            _ => new PackageLocationFallback()
            {
                Type = type,
                Data = frame.ReadInt32()
            },
        };
    }

    public static void Write(MutagenWriter writer, IAPackageLocationGetter item)
    {
        switch (item)
        {
            case IPackageLocationReferenceGetter r:
                writer.Write((int)Package.LocationType.NearReference);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Reference);
                break;
            case IPackageLocationCellGetter r:
                writer.Write((int)Package.LocationType.InCell);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Cell);
                break;
            case IPackageLocationObjectIdGetter r:
                writer.Write((int)Package.LocationType.ObjectId);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Reference);
                break;
            case IPackageLocationObjectTypeGetter r:
                writer.Write((int)Package.LocationType.ObjectType);
                writer.Write((int)r.ObjectType);
                break;
            case IPackageLocationNearCurrentLocationGetter r:
                writer.Write((int)Package.LocationType.NearCurrentLocation);
                writer.Write(r.Unused);
                break;
            case IPackageLocationNearEditorLocationGetter r:
                writer.Write((int)Package.LocationType.NearEditorLocation);
                writer.Write(r.Unused);
                break;
            case IPackageLocationNearLinkedReferenceGetter r:
                writer.Write((int)Package.LocationType.NearLinkedReference);
                writer.Write(r.Unused);
                break;
            case IPackageLocationAtPackageLocationGetter r:
                writer.Write((int)Package.LocationType.AtPackageLocation);
                writer.Write(r.Unused);
                break;
            case IPackageLocationFallbackGetter f:
                writer.Write((int)f.Type);
                writer.Write(f.Data);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
