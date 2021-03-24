using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Mutagen.Bethesda.Core.Pex.Exceptions;
using Mutagen.Bethesda.Core.Pex.Extensions;
using Mutagen.Bethesda.Core.Pex.Interfaces;

namespace Mutagen.Bethesda.Core.Pex.DataTypes
{
    [PublicAPI]
    public class PexFile : IPexFile
    {
        private readonly GameCategory _gameCategory;
        
        public uint Magic { get; set; }
        
        public byte MajorVersion { get; set; }
        
        public byte MinorVersion { get; set; }
        
        public ushort GameId { get; set; }
        
        public DateTime CompilationTime { get; set; }

        public string SourceFileName { get; set; } = string.Empty;
        
        public string Username { get; set; } = string.Empty;
        
        public string MachineName { get; set; } = string.Empty;
        
        public IStringTable? StringTable { get; set; }
        
        public IDebugInfo? DebugInfo { get; set; }

        public IUserFlagsTable? UserFlags { get; set; }

        public List<IPexObject> Objects { get; set; } = new();

        public PexFile(GameCategory gameCategory)
        {
            _gameCategory = gameCategory;
        }

        public PexFile(BinaryReader br, GameCategory gameCategory) : this(gameCategory)
        {
            Read(br);
        }

        private const uint PexMagic = 0xFA57C0DE;
        
        public void Read(BinaryReader br)
        {
            Magic = br.ReadUInt32();
            if (Magic != PexMagic)
                throw new PexParsingException($"File does not have fast code! Magic does not match {PexMagic:x8} is {Magic:x8}");
            
            MajorVersion = br.ReadByte();
            MinorVersion = br.ReadByte();
            GameId = br.ReadUInt16();
            CompilationTime = br.ReadUInt64().ToDateTime();
            SourceFileName = br.ReadString();
            Username = br.ReadString();
            MachineName = br.ReadString();

            StringTable = new StringTable(br);
            DebugInfo = new DebugInfo(br, _gameCategory);
            UserFlags = new UserFlagsTable(br);

            var objectCount = br.ReadUInt16();
            for (var i = 0; i < objectCount; i++)
            {
                var pexObject = new PexObject(br, _gameCategory);
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
            
            StringTable?.Write(bw);
            DebugInfo?.Write(bw);
            UserFlags?.Write(bw);

            bw.Write((ushort) Objects.Count);
            foreach (var pexObject in Objects)
            {
                pexObject.Write(bw);
            }
        }
    }
}
