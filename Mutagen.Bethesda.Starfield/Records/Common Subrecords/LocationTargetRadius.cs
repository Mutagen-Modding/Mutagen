using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System.Diagnostics;

namespace Mutagen.Bethesda.Starfield;

// ToDo
// Copy paste risk

public partial class LocationTargetRadius
{
    public ALocationTarget Target { get; set; } = new LocationFallback();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IALocationTargetGetter ILocationTargetRadiusGetter.Target => Target;

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
        Target = 10,
        TargetLocation = 11,
        NearSelf = 12,
        NearEditorLocationCell = 13,
        AliasForCollection = 14
    }
}
    
partial class LocationTargetRadiusBinaryCreateTranslation
{
    public static ALocationTarget GetLocationTarget(MutagenFrame frame)
    {
        var type = (LocationTargetRadius.LocationType)frame.ReadInt32();
        switch (type)
        {
            case LocationTargetRadius.LocationType.NearReference:
                return new LocationTarget()
                {
                    Link = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<IPlacedGetter>()
                };
            case LocationTargetRadius.LocationType.InCell:
                return new LocationCell()
                {
                    Link = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<ICellGetter>()
                };
            case LocationTargetRadius.LocationType.ObjectID:
                return new LocationObjectId()
                {
                    Link = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<IObjectIdGetter>()
                };
            case LocationTargetRadius.LocationType.ObjectType:
                return new LocationObjectType()
                {
                    Type = (TargetObjectType)frame.ReadInt32()
                };
            case LocationTargetRadius.LocationType.LinkedReference:
                return new LocationKeyword()
                {
                    Link = FormKeyBinaryTranslation.Instance.Parse(frame).ToLink<IKeywordGetter>()
                };
            default:
                return new LocationFallback()
                {
                    Type = type,
                    Data = frame.ReadInt32()
                };
        }
    }

    public static partial void FillBinaryTargetCustom(MutagenFrame frame, ILocationTargetRadius item)
    {
        item.Target = GetLocationTarget(frame);
    }
}

partial class LocationTargetRadiusBinaryWriteTranslation
{
    public static partial void WriteBinaryTargetCustom(MutagenWriter writer, ILocationTargetRadiusGetter item)
    {
        var target = item.Target;
        switch (target)
        {
            case LocationTarget reference:
                writer.Write((int)LocationTargetRadius.LocationType.NearReference);
                FormKeyBinaryTranslation.Instance.Write(writer, reference.Link);
                break;
            case LocationCell cell:
                writer.Write((int)LocationTargetRadius.LocationType.InCell);
                FormKeyBinaryTranslation.Instance.Write(writer, cell.Link);
                break;
            case LocationObjectId id:
                writer.Write((int)LocationTargetRadius.LocationType.ObjectID);
                FormKeyBinaryTranslation.Instance.Write(writer, id.Link);
                break;
            case LocationObjectType type:
                writer.Write((int)LocationTargetRadius.LocationType.ObjectType);
                writer.Write((int)type.Type);
                break;
            case LocationKeyword keyw:
                writer.Write((int)LocationTargetRadius.LocationType.LinkedReference);
                FormKeyBinaryTranslation.Instance.Write(writer, keyw.Link);
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

partial class LocationTargetRadiusBinaryOverlay
{
    #region Target
    public partial IALocationTargetGetter GetTargetCustom(int location);
    public IALocationTargetGetter Target => GetTargetCustom(location: 0x0);
    #endregion
    
    public partial IALocationTargetGetter GetTargetCustom(int location)
    {
        return LocationTargetRadiusBinaryCreateTranslation.GetLocationTarget(
            new MutagenFrame(
                new MutagenMemoryReadStream(_structData.Slice(location), _package.MetaData)));
    }
}