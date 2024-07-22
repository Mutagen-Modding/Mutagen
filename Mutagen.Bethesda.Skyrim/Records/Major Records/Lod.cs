using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;

namespace Mutagen.Bethesda.Skyrim;

partial class LodBinaryCreateTranslation
{
    public const int TotalLen = 260;

    public static partial void FillBinaryLevel0Custom(MutagenFrame frame, ILod item)
    {
        item.Level0.GivenPath = ReadString(frame, out var bytes);
        item.Level1.GivenPath = ReadString(frame, out bytes);
        item.Level2.GivenPath = ReadString(frame, out bytes);
        item.Level3.GivenPath = ReadString(frame, out bytes);
    }

    public static string ReadString(MutagenFrame frame, out MemorySlice<byte> extraBytes)
    {
        var str = StringBinaryTranslation.Instance.Parse(frame, parseWhole: false, stringBinaryType: StringBinaryType.NullTerminate, encoding: frame.MetaData.Encodings.NonTranslated);
        extraBytes = frame.ReadBytes(TotalLen - str.Length - 1);
        return str;
    }
}

partial class LodBinaryWriteTranslation
{
    public static partial void WriteBinaryLevel0Custom(MutagenWriter writer, ILodGetter item)
    {
        WriteString(writer, item.Level0.GivenPath);
        WriteString(writer, item.Level1.GivenPath);
        WriteString(writer, item.Level2.GivenPath);
        WriteString(writer, item.Level3.GivenPath);
    }

    public static void WriteString(MutagenWriter writer, string str)
    {
        if (str.Length >= LodBinaryCreateTranslation.TotalLen)
        {
        }
        writer.Write(str, StringBinaryType.NullTerminate, writer.MetaData.Encodings.NonTranslated);
        writer.WriteZeros((uint)(LodBinaryCreateTranslation.TotalLen - str.Length - 1));
    }
}

partial class LodBinaryOverlay
{
    public AssetLinkGetter<SkyrimModelAssetType> Level0 =>
        new AssetLink<SkyrimModelAssetType>(
            LodBinaryCreateTranslation.ReadString(
                new MutagenFrame(new MutagenMemoryReadStream(_structData, _package.MetaData)), out var _));

    public ReadOnlyMemorySlice<byte>? Level0Extra
    {
        get
        {
            LodBinaryCreateTranslation.ReadString(
                new MutagenFrame(new MutagenMemoryReadStream(_structData, _package.MetaData)), out var bytes);
            return bytes;
        }
    }

    public AssetLinkGetter<SkyrimModelAssetType> Level1 =>
        new AssetLink<SkyrimModelAssetType>(
            LodBinaryCreateTranslation.ReadString(
                new MutagenFrame(new MutagenMemoryReadStream(_structData.Slice(LodBinaryCreateTranslation.TotalLen),
                    _package.MetaData)), out var _));

    public ReadOnlyMemorySlice<byte>? Level1Extra
    {
        get
        {
            LodBinaryCreateTranslation.ReadString(
                new MutagenFrame(new MutagenMemoryReadStream(_structData.Slice(LodBinaryCreateTranslation.TotalLen),
                    _package.MetaData)), out var bytes);
            return bytes;
        }
    }

    public AssetLinkGetter<SkyrimModelAssetType> Level2 =>
        new AssetLink<SkyrimModelAssetType>(
            LodBinaryCreateTranslation.ReadString(
                new MutagenFrame(new MutagenMemoryReadStream(_structData.Slice(LodBinaryCreateTranslation.TotalLen * 2),
                    _package.MetaData)), out var _));

    public ReadOnlyMemorySlice<byte>? Level2Extra
    {
        get
        {
            LodBinaryCreateTranslation.ReadString(
                new MutagenFrame(new MutagenMemoryReadStream(_structData.Slice(LodBinaryCreateTranslation.TotalLen * 2),
                    _package.MetaData)), out var bytes);
            return bytes;
        }
    }

    public AssetLinkGetter<SkyrimModelAssetType> Level3 =>
        new AssetLink<SkyrimModelAssetType>(
            LodBinaryCreateTranslation.ReadString(
                new MutagenFrame(new MutagenMemoryReadStream(_structData.Slice(LodBinaryCreateTranslation.TotalLen * 3),
                    _package.MetaData)), out var _));

    public ReadOnlyMemorySlice<byte>? Level3Extra
    {
        get
        {
            LodBinaryCreateTranslation.ReadString(
                new MutagenFrame(new MutagenMemoryReadStream(_structData.Slice(LodBinaryCreateTranslation.TotalLen * 3),
                    _package.MetaData)), out var bytes);
            return bytes;
        }
    }
}