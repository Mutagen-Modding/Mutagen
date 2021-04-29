using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System;
using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class LocationTargetRadius
    {
        public ALocationTarget Target { get; set; } = null!;
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


    // FO4 sigReferences 
    //  'NULL', 'PLYR', 'ACHR', 'REFR', 'PGRE', 'PHZD','PMIS', 'PARW', 'PBAR', 'PBEA', 'PCON', 'PFLA'
    // skyrim: 
    //   NULL, DOOR, PLYR, ACHR, REFR, PGRE, PHZD, PMIS, PARW, PBAR, PBEA, PCON, PFLA
    //--> new in FO4: 
    //--> not in FO4: DOOR

    //FO4 ObjectId references: 
    // wbFormIDCkNoReach('Object ID', [NULL, ACTI, DOOR, STAT, MSTT, FURN, SPEL, NPC_, CONT, ARMO, AMMO, MISC, WEAP, OMOD, BOOK, NOTE, KEYM, ALCH, INGR, LIGH, FACT, FLST, IDLM, TXST, PROJ]),
    //Skyrim: 
    // wbFormIDCkNoReach('Object ID', [NULL, ACTI, DOOR, STAT, MSTT, FURN, SPEL, SCRL, NPC_, CONT, ARMO, AMMO, MISC, WEAP, BOOK, KEYM, ALCH, INGR, LIGH, FACT, FLST, IDLM, SHOU]),
    //--> new in FO4: OMOD, NOTE, TXST, PROJ
    //--> not in FO4: SCRL, SHOU




    namespace Internals
    {
        public partial class LocationTargetRadiusBinaryCreateTranslation
        {
            public static ALocationTarget GetLocationTarget(MutagenFrame frame)
            {
                var type = (LocationTargetRadius.LocationType)frame.ReadInt32();
                switch (type)
                {
                    case LocationTargetRadius.LocationType.NearReference:
                        return new LocationTarget()
                        {
                            Link = FormKeyBinaryTranslation.Instance.Parse(frame).AsLink<ILocationTargetableGetter>()
                        };
                    case LocationTargetRadius.LocationType.InCell:
                        return new LocationCell()
                        {
                            Link = FormKeyBinaryTranslation.Instance.Parse(frame).AsLink<ICellGetter>()
                        };
                    case LocationTargetRadius.LocationType.ObjectID:
                        return new LocationObjectId()
                        {
                            Link = FormKeyBinaryTranslation.Instance.Parse(frame).AsLink<IObjectIdGetter>()
                        };
                    case LocationTargetRadius.LocationType.ObjectType:
                        return new LocationObjectType()
                        {
                            Type = (TargetObjectType)frame.ReadInt32()
                        };
                    case LocationTargetRadius.LocationType.LinkedReference:
                        return new LocationKeyword()
                        {
                            Link = FormKeyBinaryTranslation.Instance.Parse(frame).AsLink<IKeywordGetter>()
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

        public partial class LocationTargetRadiusBinaryWriteTranslation
        {
            public static partial void WriteBinaryTargetCustom(MutagenWriter writer, ILocationTargetRadiusGetter item)
            {
                var target = item.Target;
                switch (target)
                {
                    case LocationTarget reference:
                        writer.Write((int)LocationTargetRadius.LocationType.NearReference);
                        FormKeyBinaryTranslation.Instance.Write(writer, reference.Link.FormKey);
                        break;
                    case LocationCell cell:
                        writer.Write((int)LocationTargetRadius.LocationType.InCell);
                        FormKeyBinaryTranslation.Instance.Write(writer, cell.Link.FormKey);
                        break;
                    case LocationObjectId id:
                        writer.Write((int)LocationTargetRadius.LocationType.ObjectID);
                        FormKeyBinaryTranslation.Instance.Write(writer, id.Link.FormKey);
                        break;
                    case LocationObjectType type:
                        writer.Write((int)LocationTargetRadius.LocationType.ObjectType);
                        writer.Write((int)type.Type);
                        break;
                    case LocationKeyword keyw:
                        writer.Write((int)LocationTargetRadius.LocationType.LinkedReference);
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

        public partial class LocationTargetRadiusBinaryOverlay
        {
            IALocationTargetGetter GetTargetCustom(int location)
            {
                return LocationTargetRadiusBinaryCreateTranslation.GetLocationTarget(
                    new MutagenFrame(
                        new MutagenMemoryReadStream(_data.Slice(location), _package.MetaData)));
            }
        }
    }
}
