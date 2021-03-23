using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mutagen.Bethesda.Pex.Extensions;
using Mutagen.Bethesda.Pex.Interfaces;

namespace Mutagen.Bethesda.Pex.DataTypes
{
    [PublicAPI]
    public class UserFlag : IUserFlag
    {
        public ushort NameIndex { get; set; }
        public byte FlagIndex { get; set; }
        public uint FlagMask => (uint) 1 << FlagIndex;
        
        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public UserFlag() { }
        
        public UserFlag(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16();
            FlagIndex = br.ReadByte();
        }

        public void Write(BinaryWriter bw)
        {
            bw.WriteUInt16BE(NameIndex);
            bw.Write(FlagIndex);
        }
    }

    [PublicAPI]
    public class UserFlagsTable : IUserFlagsTable
    {
        private readonly List<IUserFlag> _userFlags = new();
        
        public UserFlagsTable() { }
        public UserFlagsTable(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            var userFlagCount = br.ReadUInt16();
            for (var i = 0; i < userFlagCount; i++)
            {
                var userFlag = new UserFlag(br);
                _userFlags.Add(userFlag);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((ushort) _userFlags.Count);
            foreach (var userFlag in _userFlags)
            {
                userFlag.Write(bw);
            }
        }

        public List<IUserFlag> GetUserFlags(uint userFlags)
        {
            return _userFlags.Where(x => (userFlags & x.FlagMask) == 1).ToList();
        }
    }
}
