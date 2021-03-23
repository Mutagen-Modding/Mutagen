using System;
using System.IO;
using JetBrains.Annotations;
using Mutagen.Bethesda.Pex.Extensions;
using Mutagen.Bethesda.Pex.Interfaces;

namespace Mutagen.Bethesda.Pex.DataTypes
{
    [PublicAPI]
    public class StringTable : IStringTable
    {
        public string[] Strings { get; set; } = Array.Empty<string>();
     
        public StringTable() { }
        
        public StringTable(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            var count = br.ReadUInt16();
            Strings = new string[count];
            
            for (var i = 0; i < count; i++)
            {
                Strings[i] = br.ReadString();
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((ushort) Strings.Length);
            foreach (var s in Strings)
            {
                bw.Write(s);
            }
        }

        public string GetFromIndex(ushort index)
        {
            return Strings[index];
        }
    }
}
