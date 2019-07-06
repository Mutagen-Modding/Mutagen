using Mutagen.Bethesda;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public enum ObjectType
    {
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
        public static sbyte GetOffset(this ObjectType objType, GameMode mode)
        {
            switch (objType)
            {
                case ObjectType.Subrecord:
                    return Constants.SUBRECORD_HEADER_OFFSET;
                case ObjectType.Record:
                    return MetaDataConstants.Get(mode).MajorConstants.LengthAfterLength;
                case ObjectType.Group:
                    return Constants.GRUP_HEADER_OFFSET;
                case ObjectType.Mod:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}