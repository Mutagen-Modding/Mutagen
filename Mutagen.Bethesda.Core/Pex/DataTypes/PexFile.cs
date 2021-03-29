using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Noggog;

namespace Mutagen.Bethesda.Pex
{
    public partial class PexFile
    {
        private readonly GameCategory _gameCategory;
        private Dictionary<ushort, string> _strings = new();
        private ExtendedList<UserFlag> _userFlags = new();
        
        public PexFile(GameCategory gameCategory)
        {
            _gameCategory = gameCategory;
        }

        public PexFile(BinaryReader br, GameCategory gameCategory) : this(gameCategory)
        {
            Read(br);
        }

        internal string GetStringFromIndex(ushort index)
        {
            if(_strings.TryGetValue(index, out var value))
                return value;
            throw new InvalidDataException($"Unable to find string in table at index {index}");
        }

        internal ushort GetIndexFromString(string? value)
        {
            if (value == null) return ushort.MaxValue;
            var pair = _strings.First(x => x.Value.Equals(value));
            return pair.Key;
        }
        
        internal IEnumerable<UserFlag> GetUserFlags(uint userFlags) => _userFlags.Where(x => (userFlags & x.FlagMask) == 1);

        internal uint GetUserFlags(IEnumerable<IUserFlag> userFlags)
        {
            return userFlags.Aggregate<IUserFlag, uint>(0, (current, userFlag) => current | userFlag.FlagMask);
        }
        
        private const uint PexMagic = 0xFA57C0DE;
        
        public void Read(BinaryReader br)
        {
            Magic = br.ReadUInt32();
            if (Magic != PexMagic)
                throw new InvalidDataException($"File does not have fast code! Magic does not match {PexMagic:x8} is {Magic:x8}");
            
            MajorVersion = br.ReadByte();
            MinorVersion = br.ReadByte();
            GameId = br.ReadUInt16();
            CompilationTime = br.ReadUInt64().ToDateTime();
            SourceFileName = br.ReadString();
            Username = br.ReadString();
            MachineName = br.ReadString();

            var stringsCount = br.ReadUInt16();
            
            for (var i = 0; i < stringsCount; i++)
            {
                _strings.Add((ushort) i, br.ReadString());
            }
            
            DebugInfo = new DebugInfo(br, _gameCategory, this);
            
            var userFlagCount = br.ReadUInt16();
            for (var i = 0; i < userFlagCount; i++)
            {
                var userFlag = new UserFlag(br, this);
                _userFlags.Add(userFlag);
            }

            var objectCount = br.ReadUInt16();
            for (var i = 0; i < objectCount; i++)
            {
                var pexObject = new PexObject(br, _gameCategory, this);
                Objects.Add(pexObject);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(PexMagic);
            bw.Write(MajorVersion);
            bw.Write(MinorVersion);
            bw.Write(GameId);
            bw.Write(CompilationTime.ToUInt64());
            bw.Write(SourceFileName);
            bw.Write(Username);
            bw.Write(MachineName);
            
            bw.Write((ushort) _strings.Count);
            foreach (var pair in _strings)
            {
                bw.Write(pair.Value);
            }
            
            DebugInfo?.Write(bw);
            
            bw.Write((ushort) _userFlags.Count);
            foreach (var userFlag in _userFlags)
            {
                userFlag.Write(bw);
            }

            bw.Write((ushort) Objects.Count);
            foreach (var pexObject in Objects)
            {
                pexObject.Write(bw);
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
            using var br = new PexReader(stream, Encoding.UTF8, gameCategory.IsBigEndian());

            //https://en.uesp.net/wiki/Skyrim_Mod:Compiled_Script_File_Format
            var pexFile = new PexFile(br, gameCategory);

            if (stream.Position != stream.Length)
                throw new InvalidDataException("Finished reading but end of the stream was not reached! " +
                                              $"Current position: {stream.Position} " +
                                              $"Stream length: {stream.Length} " +
                                              $"Missing: {stream.Length - stream.Position}");

            return pexFile;
        }
    }
}
