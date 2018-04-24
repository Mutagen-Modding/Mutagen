using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Worldspace
    {
        [Flags]
        public enum Flag
        {
            SmallWorld = 0x01,
            CantFastTravel = 0x02,
            OblivionWorldspace = 0x04,
            NoLODWater = 0x10,
        }

        static partial void FillBinary_OffsetLength_Custom(MutagenFrame frame, Worldspace item, Func<Worldspace_ErrorMask> errorMask)
        {
            if (!HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var xLen).Type.Equals("XXXX")
                || xLen != 4)
            {
                throw new ArgumentException();
            }
            var contentLen = frame.Reader.ReadInt32();
            if (!HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var oLen).Type.Equals("OFST")
                || oLen != 0)
            {
                throw new ArgumentException();
            }
            item.OffsetData = frame.Reader.ReadBytes(contentLen);
        }

        static partial void WriteBinary_OffsetLength_Custom(MutagenWriter writer, Worldspace item, Func<Worldspace_ErrorMask> errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, Worldspace_Registration.XXXX_HEADER))
            {
                writer.Write(item.OffsetData.Length);
            }
            writer.Write(Worldspace_Registration.OFST_HEADER.Type);
            writer.WriteZeros(2);
            writer.Write(item.OffsetData);
        }

        static partial void CustomBinaryEnd_Import(MutagenFrame frame, Worldspace obj, Func<Worldspace_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }

        static partial void CustomBinaryEnd_Export(MutagenWriter writer, Worldspace obj, Func<Worldspace_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }
    }
}
