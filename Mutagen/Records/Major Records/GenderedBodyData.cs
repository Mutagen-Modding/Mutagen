using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Binary;
using Mutagen.Internals;
using Noggog.Notifying;

namespace Mutagen
{
    public partial class GenderedBodyData
    {
        static partial void FillBinary_Female_Custom(
            MutagenFrame frame,
            IGenderedBodyDataGetter item, 
            bool doMasks,
            int fieldIndex,
            Func<GenderedBodyData_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }

        static partial void FillBinary_Male_Custom(
            MutagenFrame frame,
            IGenderedBodyDataGetter item,
            bool doMasks, 
            int fieldIndex,
            Func<GenderedBodyData_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }

        static partial void WriteBinary_Female_Custom(
            MutagenWriter writer,
            IGenderedBodyDataGetter item, 
            bool doMasks,
            int fieldIndex, 
            Func<GenderedBodyData_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }

        static partial void WriteBinary_Male_Custom(
            MutagenWriter writer,
            IGenderedBodyDataGetter item,
            bool doMasks, 
            int fieldIndex, 
            Func<GenderedBodyData_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }
    }
}
