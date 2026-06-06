using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

public partial class APackageTarget
{
    // Shared union read/write for the PTDT/PTD2 "Target" 8-byte slot
    // (Type itU32 discriminator + 4-byte value), per wbPxDTLocationDecider.
    public static APackageTarget Create(MutagenFrame frame)
    {
        var type = (Package.TargetType)frame.ReadInt32();
        return type switch
        {
            Package.TargetType.SpecificReference => new PackageTargetReference()
            {
                Reference = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<IPlacedGetter>()
            },
            Package.TargetType.ObjectId => new PackageTargetObjectId()
            {
                Reference = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<IPackageTargetObjectGetter>()
            },
            Package.TargetType.ObjectType => new PackageTargetObjectType()
            {
                ObjectType = (Package.ObjectType)frame.ReadInt32()
            },
            // xEdit models the Linked Reference branch's 4-byte value as Unused; preserve the raw bits.
            Package.TargetType.LinkedReference => new PackageTargetLinkedReference()
            {
                Unused = frame.ReadInt32()
            },
            _ => new PackageTargetFallback()
            {
                Type = type,
                Data = frame.ReadInt32()
            },
        };
    }

    public static void Write(MutagenWriter writer, IAPackageTargetGetter item)
    {
        switch (item)
        {
            case IPackageTargetReferenceGetter r:
                writer.Write((int)Package.TargetType.SpecificReference);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Reference);
                break;
            case IPackageTargetObjectIdGetter r:
                writer.Write((int)Package.TargetType.ObjectId);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Reference);
                break;
            case IPackageTargetObjectTypeGetter r:
                writer.Write((int)Package.TargetType.ObjectType);
                writer.Write((int)r.ObjectType);
                break;
            case IPackageTargetLinkedReferenceGetter r:
                writer.Write((int)Package.TargetType.LinkedReference);
                writer.Write(r.Unused);
                break;
            case IPackageTargetFallbackGetter f:
                writer.Write((int)f.Type);
                writer.Write(f.Data);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
