using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class AvailableMorphs_Registration
        {
            public static readonly RecordType MPAV_HEADER = new RecordType("MPAV");
        }

        public partial class AvailableMorphsBinaryOverlay
        {
            public int NoseNumber => 0;

            private int noseDataLocation;
            private int browDataLocation;
            private int eyeDataLocation;
            private int lipDataLocation;

            public int NoseIndex => 0;
            public ReadOnlyMemorySlice<byte> NoseData => HeaderTranslation.ExtractSubrecordMemory(_data.Slice(noseDataLocation), _package.Meta);
            public int BrowIndex => 1;
            public ReadOnlyMemorySlice<byte> BrowData => HeaderTranslation.ExtractSubrecordMemory(_data.Slice(browDataLocation), _package.Meta);
            public int EyeIndex => 2;
            public ReadOnlyMemorySlice<byte> EyeData => HeaderTranslation.ExtractSubrecordMemory(_data.Slice(eyeDataLocation), _package.Meta);
            public int LipIndex => 3;
            public ReadOnlyMemorySlice<byte> LipData => HeaderTranslation.ExtractSubrecordMemory(_data.Slice(lipDataLocation), _package.Meta);

            partial void ParseCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                throw new NotImplementedException();
                for (int i = 0; i < 4; i++)
                {
                    var subFrame = _package.Meta.ReadSubrecordFrame(stream);
                    if (subFrame.Header.RecordType != AvailableMorphs_Registration.MPAI_HEADER
                        || subFrame.Header.ContentLength != 4)
                    {
                        throw new ArgumentException();
                    }

                    var contentFrame = _package.Meta.ReadSubrecordFrame(stream);
                    if (contentFrame.Header.RecordType != AvailableMorphs_Registration.MPAV_HEADER
                        || contentFrame.Header.ContentLength != 32)
                    {
                        throw new ArgumentException();
                    }

                    var index = BinaryPrimitives.ReadInt32LittleEndian(subFrame.Content);
                    switch (index)
                    {
                    }
                }
            }
        }
    }
}
