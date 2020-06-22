using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class APackageTarget
    {
        public enum Type
        {
            SpecificReference,
            ObjectID,
            ObjectType,
            LinkedReference,
            RefAlias,
            Unknown,
            Self,
        }

        public static APackageTarget CreateFromBinary(MutagenFrame frame)
        {
            APackageTarget target = ((APackageTarget.Type)frame.ReadUInt32()) switch
            {
                APackageTarget.Type.SpecificReference => new PackageTargetSpecificReference()
                {
                    Reference = FormKeyBinaryTranslation.Instance.Parse(frame)
                },
                APackageTarget.Type.ObjectID => new PackageTargetObjectID()
                {
                    Reference = FormKeyBinaryTranslation.Instance.Parse(frame)
                },
                APackageTarget.Type.ObjectType => new PackageTargetObjectType()
                {
                    Type = (TargetObjectType)frame.ReadInt32()
                },
                APackageTarget.Type.LinkedReference => new PackageTargetReference()
                {
                    Reference = FormKeyBinaryTranslation.Instance.Parse(frame)
                },
                APackageTarget.Type.RefAlias => new PackageTargetAlias()
                {
                    Alias = frame.ReadInt32()
                },
                APackageTarget.Type.Unknown => new PackageTargetUnknown()
                {
                    Data = frame.ReadInt32()
                },
                APackageTarget.Type.Self => new PackageTargetSelf()
                {
                    Data = frame.ReadInt32()
                },
                _ => throw new NotImplementedException(),
            };
            target.CountOrDistance = frame.ReadInt32();
            return target;
        }
    }

    namespace Internals
    {
        public partial class APackageTargetBinaryCreateTranslation
        {
        }

        public partial class APackageTargetBinaryWriteTranslation
        {
            public static void WriteCustom(MutagenWriter writer, IAPackageTargetGetter item)
            {
                switch (item)
                {
                    case PackageTargetSpecificReference r:
                        writer.Write((int)APackageTarget.Type.SpecificReference);
                        FormKeyBinaryTranslation.Instance.Write(writer, r.Reference.FormKey);
                        break;
                    case PackageTargetObjectID r:
                        writer.Write((int)APackageTarget.Type.ObjectID);
                        FormKeyBinaryTranslation.Instance.Write(writer, r.Reference.FormKey);
                        break;
                    case PackageTargetObjectType r:
                        writer.Write((int)APackageTarget.Type.ObjectType);
                        writer.Write((int)r.Type);
                        break;
                    case PackageTargetReference r:
                        writer.Write((int)APackageTarget.Type.LinkedReference);
                        FormKeyBinaryTranslation.Instance.Write(writer, r.Reference.FormKey);
                        break;
                    case PackageTargetAlias r:
                        writer.Write((int)APackageTarget.Type.RefAlias);
                        writer.Write(r.Alias);
                        break;
                    case PackageTargetUnknown r:
                        writer.Write((int)APackageTarget.Type.Unknown);
                        writer.Write(r.Data);
                        break;
                    case PackageTargetSelf r:
                        writer.Write((int)APackageTarget.Type.Self);
                        writer.Write(r.Data);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                writer.Write(item.CountOrDistance);
            }
        }

        public partial class APackageTargetBinaryOverlay
        {
            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                throw new NotImplementedException();
            }
        }
    }
}
