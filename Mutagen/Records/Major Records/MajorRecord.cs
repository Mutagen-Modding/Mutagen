using Mutagen.Binary;
using Mutagen.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public partial class MajorRecord
    {
        internal static void Fill_Binary(
            MutagenFrame frame,
            MajorRecord record,
            bool doMasks,
            out MajorRecord_ErrorMask errorMask)
        {
            MajorRecord_ErrorMask errMask = null;
            Func<MajorRecord_ErrorMask> errorMaskCreator = () =>
            {
                if (errMask == null)
                {
                    errMask = new MajorRecord_ErrorMask();
                }
                return errMask;
            };
            Fill_Binary_Structs(
                record,
                frame,
                errorMaskCreator);
            for (int i = 0; i < MajorRecord_Registration.NumTypedFields; i++)
            {
                Fill_Binary_RecordTypes(
                    record,
                    frame,
                    errorMaskCreator);
            }
            errorMask = errMask;
        }
    }
}
