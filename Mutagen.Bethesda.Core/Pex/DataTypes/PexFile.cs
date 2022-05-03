using System.Text;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Pex;

public partial class PexFile
{
    private readonly GameCategory _gameCategory;

    public PexFile(GameCategory gameCategory)
    {
        _gameCategory = gameCategory;
    }

    private const uint PexMagic = 0xFA57C0DE;

    private static void Read(PexFile file, IBinaryReadStream br)
    {
        var magic = br.ReadUInt32();
        if (magic != PexMagic)
            throw new InvalidDataException($"File does not have fast code! Magic does not match {PexMagic:x8} is {magic:x8}");

        file.MajorVersion = br.ReadUInt8();
        file.MinorVersion = br.ReadUInt8();
        file.GameId = br.ReadUInt16();
        file.CompilationTime = br.ReadUInt64().ToDateTime();
        file.SourceFileName = br.ReadPrependedString(2, MutagenEncodingProvider._1252);
        file.Username = br.ReadPrependedString(2, MutagenEncodingProvider._1252);
        file.MachineName = br.ReadPrependedString(2, MutagenEncodingProvider._1252);

        var stringsCount = br.ReadUInt16();

        var bundle = new PexParseMeta(
            file._gameCategory,
            br,
            new Dictionary<ushort, string>());
        for (var i = 0; i < stringsCount; i++)
        {
            bundle.Strings.Add((ushort)i, br.ReadPrependedString(2, MutagenEncodingProvider._1252));
        }

        var hasDebugInfo = bundle.Reader.ReadUInt8() == 1;
        if (hasDebugInfo)
        {
            file.DebugInfo = DebugInfo.Create(bundle);
        }

        var userFlagCount = br.ReadUInt16();
        for (var i = 0; i < userFlagCount; i++)
        {
            var str = bundle.ReadString();
            file.UserFlags[br.ReadUInt8()] = str;
        }

        var objectCount = br.ReadUInt16();
        for (var i = 0; i < objectCount; i++)
        {
            var pexObject = PexObject.Create(bundle);
            file.Objects.Add(pexObject);
        }
    }

    internal void Write(BinaryWriter bw)
    {
        bw.Write(PexMagic);
        bw.Write(MajorVersion);
        bw.Write(MinorVersion);
        bw.Write(GameId);
        bw.Write(CompilationTime.ToUInt64());
        bw.Write(SourceFileName);
        bw.Write(Username);
        bw.Write(MachineName);

        var memoryTrib = new MemoryTributary();
        var bw2 = new PexWriter(memoryTrib, Encoding.UTF8, _gameCategory.IsBigEndian());
        var writeMeta = new PexWriteMeta(_gameCategory, bw2);
        WriteContent(writeMeta);

        bw.Write((ushort)writeMeta.Strings.Count);
        foreach (var pair in writeMeta.Strings
                     .OrderBy(x => x.Value))
        {
            bw.Write(pair.Key);
        }

        memoryTrib.Position = 0;
        memoryTrib.CopyTo(bw.BaseStream);
    }

    private void WriteContent(PexWriteMeta write)
    {
        if (DebugInfo == null)
        {
            write.Writer.Write((byte)0);
        }
        else
        {
            write.Writer.Write((byte)1);
            DebugInfo.Write(write);
        }

        write.Writer.Write((ushort)UserFlags.NotNull().Count());
        for (byte i = 0; i < 32; i++)
        {
            var str = UserFlags[i];
            if (str == null) continue;
            write.WriteString(str);
            write.Writer.Write(i);
        }

        write.Writer.Write((ushort)Objects.Count);
        foreach (var pexObject in Objects)
        {
            pexObject.Write(write);
        }
    }

    public static PexFile CreateFromFile(string file, GameCategory gameCategory)
    {
        if (!File.Exists(file))
            throw new ArgumentException($"Input file does not exist {file}!", nameof(file));

        using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        return CreateFromStream(fs, gameCategory);
    }

    public static PexFile CreateFromStream(Stream stream, GameCategory gameCategory)
    {
        using var br = new BinaryReadStream(stream, isLittleEndian: !gameCategory.IsBigEndian());

        //https://en.uesp.net/wiki/Skyrim_Mod:Compiled_Script_File_Format
        var pexFile = new PexFile(gameCategory);
        Read(pexFile, br);

        if (stream.Position != stream.Length)
            throw new InvalidDataException("Finished reading but end of the stream was not reached! " +
                                           $"Current position: {stream.Position} " +
                                           $"Stream length: {stream.Length} " +
                                           $"Missing: {stream.Length - stream.Position}");

        return pexFile;
    }
}