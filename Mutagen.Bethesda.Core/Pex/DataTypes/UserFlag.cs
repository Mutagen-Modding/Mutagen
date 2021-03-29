using System.IO;

namespace Mutagen.Bethesda.Pex
{
    public partial interface IUserFlagGetter
    {
        uint FlagMask { get; }
    }

    public partial class UserFlag
    {
        public uint FlagMask => (uint)1 << FlagIndex;

        private readonly PexFile _pexFile = null!;
        
        public UserFlag(PexFile pexFile) { _pexFile = pexFile; }

        public UserFlag(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            Name = _pexFile.GetStringFromIndex(br.ReadUInt16());
            FlagIndex = br.ReadByte();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(Name));
            bw.Write(FlagIndex);
        }
    }
}
