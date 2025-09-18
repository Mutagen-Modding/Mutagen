using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Starfield;

partial class APackageTarget
{
    public enum Type
    {
        SpecificReference,
        ObjectID,
        ObjectType,
        LinkedReference,
        RefAlias,
        InterruptData,
        Self,
        Keyword
    }

    public static APackageTarget CreateFromBinary(MutagenFrame frame)
    {
        var i = frame.ReadInt32();
        APackageTarget target = ((APackageTarget.Type)i) switch
        {
            APackageTarget.Type.SpecificReference => new PackageTargetSpecificReference()
            {
                Reference = new FormLink<IPlacedGetter>(FormKeyBinaryTranslation.Instance.Parse(frame))
            },
            APackageTarget.Type.ObjectID => new PackageTargetObjectID()
            {
                Reference = new FormLink<IObjectIdGetter>(FormKeyBinaryTranslation.Instance.Parse(frame))
            },
            APackageTarget.Type.ObjectType => new PackageTargetObjectType()
            {
                Type = (TargetObjectType)frame.ReadInt32()
            },
            APackageTarget.Type.LinkedReference => new PackageTargetLinkedReference()
            {
                Keyword = new FormLink<IKeywordGetter>(FormKeyBinaryTranslation.Instance.Parse(frame))
            },
            APackageTarget.Type.RefAlias => new PackageTargetAlias()
            {
                Alias = frame.ReadInt32()
            },
            APackageTarget.Type.InterruptData => new PackageTargetInterruptData()
            {
                Data = frame.ReadUInt32()
            },
            APackageTarget.Type.Self => new PackageTargetSelf()
            {
                Data = frame.ReadInt32()
            },
            APackageTarget.Type.Keyword => new PackageTargetKeyword()
            {
                Keyword = new FormLink<IKeywordGetter>(FormKeyBinaryTranslation.Instance.Parse(frame))
            },
            _ => new PackageTargetUnknown()
            {
                Data = frame.ReadInt32(),
                Type = i
            },
        };
        target.CountOrDistance = frame.ReadInt32();
        return target;
    }
}

partial class APackageTargetBinaryCreateTranslation
{
    public static partial void FillBinaryDataParseCustom(MutagenFrame frame, IAPackageTarget item)
    {
    }
}

partial class APackageTargetBinaryWriteTranslation
{
    public static void WriteCustom(MutagenWriter writer, IAPackageTargetGetter item)
    {
        switch (item)
        {
            case PackageTargetSpecificReference r:
                writer.Write((int)APackageTarget.Type.SpecificReference);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Reference);
                break;
            case PackageTargetObjectID r:
                writer.Write((int)APackageTarget.Type.ObjectID);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Reference);
                break;
            case PackageTargetObjectType r:
                writer.Write((int)APackageTarget.Type.ObjectType);
                writer.Write((int)r.Type);
                break;
            case PackageTargetLinkedReference r:
                writer.Write((int)APackageTarget.Type.LinkedReference);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Keyword);
                break;
            case PackageTargetAlias r:
                writer.Write((int)APackageTarget.Type.RefAlias);
                writer.Write(r.Alias);
                break;
            case PackageTargetInterruptData r:
                writer.Write((int)APackageTarget.Type.InterruptData);
                writer.Write(r.Data);
                break;
            case PackageTargetSelf r:
                writer.Write((int)APackageTarget.Type.Self);
                writer.Write(r.Data);
                break;
            case PackageTargetKeyword r:
                writer.Write((int)APackageTarget.Type.Keyword);
                FormKeyBinaryTranslation.Instance.Write(writer, r.Keyword);
                break;
            case PackageTargetUnknown r:
                writer.Write(r.Type);
                writer.Write(r.Data);
                break;
            default:
                throw new NotImplementedException();
        }
        writer.Write(item.CountOrDistance);
    }

    public static partial void WriteBinaryDataParseCustom(MutagenWriter writer, IAPackageTargetGetter item)
    {
    }
}

partial class APackageTargetBinaryOverlay
{
    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        throw new NotImplementedException();
    }
}
