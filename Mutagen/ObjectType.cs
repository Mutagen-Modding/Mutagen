using Mutagen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public enum ObjectType
    {
        Struct,
        Subrecord,
        Record,
        Group,
        Mod
    }
}

namespace System
{
    public static class ObjectTypeExt
    {
        public static ContentLength GetLengthLength(this ObjectType objType)
        {
            switch (objType)
            {
                case ObjectType.Struct:
                    return ContentLength.Zero;
                case ObjectType.Subrecord:
                    return Constants.SUBRECORD_LENGTHLENGTH;
                case ObjectType.Record:
                    return Constants.RECORD_LENGTHLENGTH;
                case ObjectType.Group:
                    return Constants.GRUP_LENGTHLENGTH;
                case ObjectType.Mod:
                default:
                    throw new NotImplementedException();
            }
        }

        public static ContentLength GetOffset(this ObjectType objType)
        {
            switch (objType)
            {
                case ObjectType.Struct:
                    return ContentLength.Zero;
                case ObjectType.Subrecord:
                    return Constants.SUBRECORD_HEADER_OFFSET;
                case ObjectType.Record:
                    return Constants.RECORD_HEADER_OFFSET;
                case ObjectType.Group:
                    return Constants.GRUP_HEADER_OFFSET;
                case ObjectType.Mod:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}