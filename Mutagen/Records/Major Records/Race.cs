using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Binary;
using Mutagen.Internals;

namespace Mutagen
{
    public partial class Race
    {
        //ToDo
        //Make markers private api

        [Flags]
        public enum Flag
        {
            Playable = 1,
        }

        public enum FaceIndex
        {
            Head = 0,
            EarMale = 1,
            EarFemale = 2,
            Mouth = 3,
            TeethLower = 4,
            TeethUpper = 5,
            Tongue = 6,
            EyeLeft = 7,
            EyeRight = 8,
        }

        public enum BodyIndex
        {
            UpperBody = 0,
            LowerBody = 1,
            Hand = 2,
            Foot = 3,
            Tail = 4,
        }

        static partial void FillBinary_FaceData_Custom(
            MutagenFrame frame, 
            IRaceGetter item,
            bool doMasks,
            int fieldIndex, 
            Func<Race_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }

        static partial void WriteBinary_FaceData_Custom(
            MutagenWriter writer, 
            IRaceGetter item, 
            bool doMasks, 
            int fieldIndex,
            Func<Race_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }
    }
}
