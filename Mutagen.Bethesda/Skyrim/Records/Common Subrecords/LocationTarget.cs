using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class LocationTarget
    {
        public ALocationTarget Target { get; set; } = null!;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IALocationTargetGetter ILocationTargetGetter.Target => Target;

        public enum LocationType
        {
            NearReference = 0,
            InCell = 1,
            NearPackageStart = 2,
            NearEditorLocation = 3,
            ObjectID = 4,
            ObjectType = 5,
            LinkedReference = 6,
            AtPackageLocation = 7,
            AliasForReference = 8,
            AliasForLocation = 9,
            NearSelf = 12
        }
    }

    namespace Internals
    {
        public partial class LocationTargetBinaryCreateTranslation
        {
            public static ALocationTarget GetLocationTarget(MutagenFrame frame)
            {
                var type = (LocationTarget.LocationType)frame.ReadInt32();
                switch (type)
                {
                    case LocationTarget.LocationType.NearReference:
                        return new LocationReference()
                        {
                            Link = FormKeyBinaryTranslation.Instance.Parse(frame)
                        };
                    case LocationTarget.LocationType.InCell:
                        return new LocationCell()
                        {
                            Link = FormKeyBinaryTranslation.Instance.Parse(frame)
                        };
                    case LocationTarget.LocationType.ObjectID:
                        return new LocationObjectId()
                        {
                            Link = FormKeyBinaryTranslation.Instance.Parse(frame)
                        };
                    case LocationTarget.LocationType.ObjectType:
                        return new LocationObjectType()
                        {
                            Type = (TargetObjectType)frame.ReadInt32()
                        };
                    case LocationTarget.LocationType.LinkedReference:
                        return new LocationKeyword()
                        {
                            Link = FormKeyBinaryTranslation.Instance.Parse(frame)
                        };
                    default:
                        return new LocationFallback()
                        {
                            Type = type,
                            Data = frame.ReadInt32()
                        };
                }
            }

            static partial void FillBinaryTargetCustom(MutagenFrame frame, ILocationTarget item)
            {
                item.Target = GetLocationTarget(frame);
            }
        }

        public partial class LocationTargetBinaryWriteTranslation
        {
            static partial void WriteBinaryTargetCustom(MutagenWriter writer, ILocationTargetGetter item)
            {
                var target = item.Target;
                switch (target)
                {
                    case LocationReference reference:
                        writer.Write((int)LocationTarget.LocationType.NearReference);
                        FormKeyBinaryTranslation.Instance.Write(writer, reference.Link.FormKey);
                        break;
                    case LocationCell cell:
                        writer.Write((int)LocationTarget.LocationType.InCell);
                        FormKeyBinaryTranslation.Instance.Write(writer, cell.Link.FormKey);
                        break;
                    case LocationObjectId id:
                        writer.Write((int)LocationTarget.LocationType.ObjectID);
                        FormKeyBinaryTranslation.Instance.Write(writer, id.Link.FormKey);
                        break;
                    case LocationObjectType type:
                        writer.Write((int)LocationTarget.LocationType.ObjectType);
                        writer.Write((int)type.Type);
                        break;
                    case LocationKeyword keyw:
                        writer.Write((int)LocationTarget.LocationType.LinkedReference);
                        FormKeyBinaryTranslation.Instance.Write(writer, keyw.Link.FormKey);
                        break;
                    case LocationFallback fallback:
                        writer.Write((int)fallback.Type);
                        writer.Write(fallback.Data);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public partial class LocationTargetBinaryOverlay
        {
            IALocationTargetGetter GetTargetCustom(int location)
            {
                return LocationTargetBinaryCreateTranslation.GetLocationTarget(
                    new MutagenFrame(
                        new MutagenMemoryReadStream(_data.Slice(location), _package.MetaData)));
            }
        }
    }
}
