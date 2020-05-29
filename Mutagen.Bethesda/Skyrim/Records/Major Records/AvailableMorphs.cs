using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using static Mutagen.Bethesda.Skyrim.Internals.AvailableMorphsBinaryCreateTranslation;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class AvailableMorphs_Registration
        {
            public static readonly RecordType MPAV_HEADER = new RecordType("MPAV");
        }

        public partial class AvailableMorphsBinaryCreateTranslation
        {
            public enum MorphEnum
            {
                Nose = 0,
                Brow = 1,
                Eye = 2,
                Lip = 3,
            }

            static partial void FillBinaryParseCustom(MutagenFrame frame, IAvailableMorphs item)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!frame.Reader.TryReadSubrecordFrame(AvailableMorphs_Registration.MPAI_HEADER, out var indexFrame)) break;
                    if (indexFrame.Content.Length != 4)
                    {
                        throw new ArgumentException($"Unexpected Morphs index length: {indexFrame.Content.Length} != 4");
                    }
                    if (!frame.Reader.TryReadSubrecordFrame(AvailableMorphs_Registration.MPAV_HEADER, out var dataFrame))
                    {
                        throw new ArgumentException($"Did not read in expected morph data record MPAI");
                    }
                    if (dataFrame.Content.Length != 32)
                    {
                        throw new ArgumentException($"Morph data length unexpected: {dataFrame.Content.Length} != 32");
                    }
                    var index = (MorphEnum)BinaryPrimitives.ReadInt32LittleEndian(indexFrame.Content);
                    var morph = new Morph()
                    {
                        Data = dataFrame.Content.ToArray()
                    };
                    switch (index)
                    {
                        case MorphEnum.Nose:
                            item.Nose = morph;
                            break;
                        case MorphEnum.Brow:
                            item.Brow = morph;
                            break;
                        case MorphEnum.Eye:
                            item.Eye = morph;
                            break;
                        case MorphEnum.Lip:
                            item.Lip = morph;
                            break;
                        default:
                            throw new ArgumentException($"Unexpected morph index: {index}");
                    }
                }
            }
        }

        public partial class AvailableMorphsBinaryWriteTranslation
        {
            static void WriteMorph(MutagenWriter writer, MorphEnum e, IMorphGetter? morph)
            {
                if (morph == null) return;
                using (HeaderExport.ExportSubrecordHeader(writer, AvailableMorphs_Registration.MPAI_HEADER))
                {
                    writer.Write((int)e);
                }
                using (HeaderExport.ExportSubrecordHeader(writer, AvailableMorphs_Registration.MPAV_HEADER))
                {
                    writer.Write(morph.Data);
                }
            }

            static partial void WriteBinaryParseCustom(MutagenWriter writer, IAvailableMorphsGetter item)
            {
                WriteMorph(writer, MorphEnum.Nose, item.Nose);
                WriteMorph(writer, MorphEnum.Brow, item.Brow);
                WriteMorph(writer, MorphEnum.Eye, item.Eye);
                WriteMorph(writer, MorphEnum.Lip, item.Lip);
            }
        }

        public partial class AvailableMorphsBinaryOverlay
        {
            private AvailableMorphs morphs = null!;

            public IMorphGetter? Nose => morphs.Nose;

            public IMorphGetter? Brow => morphs.Brow;

            public IMorphGetter? Eye => morphs.Eye;

            public IMorphGetter? Lip => morphs.Lip;

            partial void ParseCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                morphs = new AvailableMorphs();
                AvailableMorphsBinaryCreateTranslation.FillBinaryParseCustomPublic(
                    new MutagenFrame(new MutagenInterfaceReadStream(stream: stream, metaData: _package.MetaData)),
                    morphs);
            }
        }
    }
}
