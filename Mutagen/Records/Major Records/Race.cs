using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Binary;
using Mutagen.Internals;
using Loqui;

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
            var nam0 = HeaderTranslation.GetNextSubRecordType(frame, out var contentLength);
            if (!nam0.Equals(Race_Registration.NAM0_HEADER))
            {
                throw new ArgumentException($"Expected {Race_Registration.NAM0_HEADER}");
            }
            frame.Position += Constants.SUBRECORD_LENGTH + contentLength;
            ListBinaryTranslation<FacePart, MaskItem<Exception, FacePart_ErrorMask>>.Instance.ParseRepeatedItem(
                frame: frame,
                doMasks: doMasks,
                fieldIndex: fieldIndex,
                triggeringRecord: FacePart_Registration.TRIGGERING_RECORD_TYPE,
                objType: ObjectType.Subrecord,
                errorMask: errorMask,
                transl: (MutagenFrame r, bool listDoMasks, out MaskItem<Exception, FacePart_ErrorMask> listSubMask) =>
                {
                    return LoquiBinaryTranslation<FacePart, FacePart_ErrorMask>.Instance.Parse(
                        frame: r.Spawn(snapToFinalPosition: false),
                        doMasks: listDoMasks,
                        errorMask: out listSubMask);
                }
                );
        }

        static partial void WriteBinary_FaceData_Custom(
            MutagenWriter writer, 
            IRaceGetter item, 
            bool doMasks, 
            int fieldIndex,
            Func<Race_ErrorMask> errorMask)
        {
            using (HeaderExport.ExportHeader(writer, Race_Registration.NAM0_HEADER, ObjectType.Subrecord))
            {
            }
            Mutagen.Binary.ListBinaryTranslation<FacePart, MaskItem<Exception, FacePart_ErrorMask>>.Instance.Write(
                writer: writer,
                item: item.FaceData,
                fieldIndex: fieldIndex,
                doMasks: doMasks,
                errorMask: errorMask,
                transl: (FacePart subItem, bool listDoMasks, out MaskItem<Exception, FacePart_ErrorMask> listSubMask) =>
                {
                    LoquiBinaryTranslation<FacePart, FacePart_ErrorMask>.Instance.Write(
                        writer: writer,
                        item: subItem,
                        doMasks: doMasks,
                        errorMask: out listSubMask);
                }
                );
        }
    }
}
