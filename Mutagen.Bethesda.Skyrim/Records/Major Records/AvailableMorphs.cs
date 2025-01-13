using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System.Buffers.Binary;
using Mutagen.Bethesda.Skyrim.Internals;
using static Mutagen.Bethesda.Skyrim.AvailableMorphsBinaryCreateTranslation;

namespace Mutagen.Bethesda.Skyrim;

partial class AvailableMorphsBinaryCreateTranslation
{
    public static readonly RecordType MPAV = new RecordType("MPAV");

    public enum MorphEnum
    {
        Nose = 0,
        Brow = 1,
        Eye = 2,
        Lip = 3,
    }

    public static void FillBinaryParse(MutagenFrame frame, IAvailableMorphs item)
    {
        for (int i = 0; i < 4; i++)
        {
            if (!frame.Reader.TryReadSubrecord(RecordTypes.MPAI, out var indexFrame)) break;
            if (indexFrame.Content.Length != 4)
            {
                throw new ArgumentException($"Unexpected Morphs index length: {indexFrame.Content.Length} != 4");
            }
            if (!frame.Reader.TryReadSubrecord(MPAV, out var dataFrame))
            {
                continue;
            }
            if (dataFrame.Content.Length != 32)
            {
                throw new ArgumentException($"Morph data length unexpected: {dataFrame.Content.Length} != 32");
            }
            var index = (MorphEnum)BinaryPrimitives.ReadInt32LittleEndian(indexFrame.Content);
            var morph = new Morph()
            {
                Data = dataFrame.Content.Span.ToArray()
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

    public static partial ParseResult FillBinaryParseCustom(MutagenFrame frame, IAvailableMorphs item, PreviousParse lastParsed)
    {
        FillBinaryParse(frame, item);
        return lastParsed;
    }
}

partial class AvailableMorphsBinaryWriteTranslation
{
    static void WriteMorph(MutagenWriter writer, MorphEnum e, IMorphGetter? morph)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.MPAI))
        {
            writer.Write((int)e);
        }
        if (morph == null) return;
        using (HeaderExport.Subrecord(writer, AvailableMorphsBinaryCreateTranslation.MPAV))
        {
            writer.Write(morph.Data);
        }
    }

    public static partial void WriteBinaryParseCustom(MutagenWriter writer, IAvailableMorphsGetter item)
    {
        WriteMorph(writer, MorphEnum.Nose, item.Nose);
        WriteMorph(writer, MorphEnum.Brow, item.Brow);
        WriteMorph(writer, MorphEnum.Eye, item.Eye);
        WriteMorph(writer, MorphEnum.Lip, item.Lip);
    }
}

partial class AvailableMorphsBinaryOverlay
{
    private AvailableMorphs morphs = null!;

    public IMorphGetter? Nose => morphs.Nose;

    public IMorphGetter? Brow => morphs.Brow;

    public IMorphGetter? Eye => morphs.Eye;

    public IMorphGetter? Lip => morphs.Lip;

    public partial ParseResult ParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        morphs = new AvailableMorphs();
        AvailableMorphsBinaryCreateTranslation.FillBinaryParse(
            new MutagenFrame(new MutagenInterfaceReadStream(stream: stream, metaData: _package.MetaData)),
            morphs);
        return lastParsed;
    }
}