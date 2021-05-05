using System.IO;
using System.Linq;
using Noggog;
using Mutagen.Bethesda.Pex.Binary.Translations;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Pex.Records;

namespace Mutagen.Bethesda.Skyrim.Pex
{
    public partial class PexFile
    {
        public const uint PexMagic = 0xFA57C0DE;

        void IPexFileCommonGetter.WriteToBinary(Stream stream) => this.WriteToBinary(stream);
        void IPexFileCommonGetter.WriteToBinary(string path) => this.WriteToBinary(path);
        ReadOnlyMemorySlice<string?> IPexFileCommonGetter.UserFlags => this.UserFlags;

        static PexFile CreateFromBinary(PexReader reader)
        {
            var magic = reader.ReadUInt32();
            if (magic != PexMagic)
                throw new InvalidDataException($"File does not have fast code! Magic does not match {PexMagic:x8} is {magic:x8}");

            var file = new PexFile();
            file.MajorVersion = reader.ReadUInt8();
            file.MinorVersion = reader.ReadUInt8();
            file.GameId = reader.ReadUInt16();
            file.CompilationTime = reader.ReadUInt64().ToDateTime();
            file.SourceFileName = reader.ReadPrependedString(2);
            file.Username = reader.ReadPrependedString(2);
            file.MachineName = reader.ReadPrependedString(2);

            var stringsCount = reader.ReadUInt16();

            for (var i = 0; i < stringsCount; i++)
            {
                reader.Strings.Add((ushort)i, reader.ReadPrependedString(2));
            }

            var hasDebugInfo = reader.ReadUInt8() == 1;
            if (hasDebugInfo)
            {
                file.DebugInfo = DebugInfo.CreateFromBinary(reader);
            }

            var userFlagCount = reader.ReadUInt16();
            for (var i = 0; i < userFlagCount; i++)
            {
                var str = reader.ReadString();
                file.UserFlags[reader.ReadUInt8()] = str;
            }

            var objectCount = reader.ReadUInt16();
            for (var i = 0; i < objectCount; i++)
            {
                var pexObject = Object.CreateFromBinary(reader);
                file.Objects.Add(pexObject);
            }
            return file;
        }
    }

    public class PexFilePexWriteTranslation
    {
        public static readonly PexFilePexWriteTranslation Instance = new();

        public void Write(PexWriter writer, IPexFileGetter item)
        {
            writer.Write(PexFile.PexMagic);
            writer.Write(item.MajorVersion);
            writer.Write(item.MinorVersion);
            writer.Write(item.GameId);
            writer.Write(item.CompilationTime.ToUInt64());
            writer.Writer.Write(item.SourceFileName, StringBinaryType.PrependLengthUShort);
            writer.Writer.Write(item.Username, StringBinaryType.PrependLengthUShort);
            writer.Writer.Write(item.MachineName, StringBinaryType.PrependLengthUShort);

            var memoryTrib = new MemoryTributary();
            using (var bw2 = new BinaryWriteWrapper(memoryTrib, isLittleEndian: false))
            {
                var writeMeta = new PexWriter(bw2);
                WriteContent(writeMeta, item);

                writer.Write((ushort)writeMeta.Strings.Count);
                foreach (var pair in writeMeta.Strings
                    .OrderBy(x => x.Value))
                {
                    writer.Writer.Write(pair.Key, StringBinaryType.PrependLengthUShort);
                }
            }

            memoryTrib.Position = 0;
            memoryTrib.CopyTo(writer.Writer.BaseStream);
        }

        static void WriteContent(PexWriter write, IPexFileGetter item)
        {
            if (item.DebugInfo == null)
            {
                write.Write((byte)0);
            }
            else
            {
                write.Write((byte)1);
                item.DebugInfo.WriteToBinary(write);
            }

            write.Write((ushort)item.UserFlags.NotNull().Count());
            for (byte i = 0; i < 32; i++)
            {
                var str = item.UserFlags[i];
                if (str == null) continue;
                write.Write(str);
                write.Writer.Write(i);
            }

            write.Writer.Write((ushort)item.Objects.Count);
            foreach (var pexObject in item.Objects)
            {
                pexObject.WriteToBinary(write);
            }
        }
    }
}
