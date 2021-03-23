using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Mutagen.Bethesda.Pex.Enums;
using Mutagen.Bethesda.Pex.Extensions;
using Mutagen.Bethesda.Pex.Interfaces;

namespace Mutagen.Bethesda.Pex.DataTypes
{
    [PublicAPI]
    public class DebugInfo : IDebugInfo
    {
        public bool HasDebugInfo { get; set; }
        public DateTime ModificationTime { get; set; }

        public List<IDebugFunction> Functions { get; set; } = new();

        public DebugInfo() { }
        
        public DebugInfo(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            var hasDebugInfo = br.ReadByte();
            HasDebugInfo = hasDebugInfo == 1;
            if (!HasDebugInfo) return;

            ModificationTime = br.ReadUInt64BE().ToDateTime();
            var functionCount = br.ReadUInt16BE();
            
            for (var i = 0; i < functionCount; i++)
            {
                var function = new DebugFunction(br);
                Functions.Add(function);
            }
        }

        public void Write(BinaryWriter bw)
        {
            // ReSharper disable RedundantCast
            bw.Write(HasDebugInfo ? (byte) 1 : (byte) 0);
            // ReSharper restore RedundantCast
            if (!HasDebugInfo) return;
            
            bw.WriteUInt64BE(ModificationTime.ToUInt64());
            bw.WriteUInt16BE((ushort) Functions.Count);
            foreach (var debugFunction in Functions)
            {
                debugFunction.Write(bw);
            }
        }
    }

    [PublicAPI]
    public class DebugFunction : IDebugFunction
    {
        public ushort ObjectNameIndex { get; set; } = ushort.MaxValue;
        public ushort StateNameIndex { get; set; } = ushort.MaxValue;
        public ushort FunctionNameIndex { get; set; } = ushort.MaxValue;
        public DebugFunctionType FunctionType { get; set; }
        public ushort InstructionCount { get; set; } = ushort.MaxValue;
        public List<ushort> LineNumbers { get; set; } = new();

        public DebugFunction() { }
        public DebugFunction(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            ObjectNameIndex = br.ReadUInt16BE();
            StateNameIndex = br.ReadUInt16BE();
            FunctionNameIndex = br.ReadUInt16BE();
            FunctionType = (DebugFunctionType) br.ReadByte();
            InstructionCount = br.ReadUInt16BE();

            for (var i = 0; i < InstructionCount; i++)
            {
                var lineNumber = br.ReadUInt16BE();
                LineNumbers.Add(lineNumber);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.WriteUInt16BE(ObjectNameIndex);
            bw.WriteUInt16BE(StateNameIndex);
            bw.WriteUInt16BE(FunctionNameIndex);
            bw.Write((byte) FunctionType);
            bw.WriteUInt16BE(InstructionCount);
            
            foreach (var lineNumber in LineNumbers)
            {
                bw.WriteUInt16BE(lineNumber);
            }
        }

        public string GetObjectName(IStringTable stringTable) => stringTable.GetFromIndex(ObjectNameIndex);

        public string GetStateName(IStringTable stringTable) => stringTable.GetFromIndex(StateNameIndex);

        public string GetFunctionName(IStringTable stringTable) => stringTable.GetFromIndex(FunctionNameIndex);
    }
}
