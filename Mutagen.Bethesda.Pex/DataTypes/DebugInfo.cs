using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mutagen.Bethesda.Pex.Enums;
using Mutagen.Bethesda.Pex.Extensions;
using Mutagen.Bethesda.Pex.Interfaces;

namespace Mutagen.Bethesda.Pex.DataTypes
{
    [PublicAPI]
    public class DebugInfo : IDebugInfo
    {
        private readonly GameCategory _gameCategory;
        
        public bool HasDebugInfo { get; set; }
        public DateTime ModificationTime { get; set; }

        public List<IDebugFunction> Functions { get; set; } = new();

        public List<IDebugPropertyGroup> PropertyGroups { get; set; } = new();

        public List<IDebugStructOrder> StructOrders { get; set; } = new();

        public DebugInfo(GameCategory gameCategory)
        {
            _gameCategory = gameCategory;
        }
        
        public DebugInfo(BinaryReader br, GameCategory gameCategory) : this(gameCategory) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            var hasDebugInfo = br.ReadByte();
            HasDebugInfo = hasDebugInfo == 1;
            if (!HasDebugInfo) return;

            ModificationTime = br.ReadUInt64().ToDateTime();
            
            var functionCount = br.ReadUInt16();
            for (var i = 0; i < functionCount; i++)
            {
                var function = new DebugFunction(br);
                Functions.Add(function);
            }

            //F04 only
            if (_gameCategory != GameCategory.Fallout4) return;
            
            var propertyGroupsCount = br.ReadUInt16();
            for (var i = 0; i < propertyGroupsCount; i++)
            {
                var propertyGroup = new DebugPropertyGroup(br);
                PropertyGroups.Add(propertyGroup);
            }
            
            var structOrderCount = br.ReadUInt16();
            for (var i = 0; i < structOrderCount; i++)
            {
                var structOrder = new DebugStructOrder(br);
                StructOrders.Add(structOrder);
            }
        }

        public void Write(BinaryWriter bw)
        {
            // ReSharper disable RedundantCast
            bw.Write(HasDebugInfo ? (byte) 1 : (byte) 0);
            // ReSharper restore RedundantCast
            if (!HasDebugInfo) return;
            
            bw.Write(ModificationTime.ToUInt64());
            
            bw.Write((ushort) Functions.Count);
            foreach (var debugFunction in Functions)
            {
                debugFunction.Write(bw);
            }
            
            //F04 only
            if (_gameCategory != GameCategory.Fallout4) return;
            
            bw.Write((ushort) PropertyGroups.Count);
            foreach (var propertyGroup in PropertyGroups)
            {
                propertyGroup.Write(bw);
            }
            
            bw.Write((ushort) StructOrders.Count);
            foreach (var structOrder in StructOrders)
            {
                structOrder.Write(bw);
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
            ObjectNameIndex = br.ReadUInt16();
            StateNameIndex = br.ReadUInt16();
            FunctionNameIndex = br.ReadUInt16();
            FunctionType = (DebugFunctionType) br.ReadByte();
            InstructionCount = br.ReadUInt16();

            for (var i = 0; i < InstructionCount; i++)
            {
                var lineNumber = br.ReadUInt16();
                LineNumbers.Add(lineNumber);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(ObjectNameIndex);
            bw.Write(StateNameIndex);
            bw.Write(FunctionNameIndex);
            bw.Write((byte) FunctionType);
            bw.Write(InstructionCount);
            
            foreach (var lineNumber in LineNumbers)
            {
                bw.Write(lineNumber);
            }
        }

        public string GetObjectName(IStringTable stringTable) => stringTable.GetFromIndex(ObjectNameIndex);

        public string GetStateName(IStringTable stringTable) => stringTable.GetFromIndex(StateNameIndex);

        public string GetFunctionName(IStringTable stringTable) => stringTable.GetFromIndex(FunctionNameIndex);
    }

    [PublicAPI]
    public class DebugPropertyGroup : IDebugPropertyGroup
    {
        public ushort ObjectNameIndex { get; set; }
        public ushort GroupNameIndex { get; set; }
        public List<ushort> PropertyNameIndices { get; set; } = new();
        
        public DebugPropertyGroup() { }
        public DebugPropertyGroup(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            ObjectNameIndex = br.ReadUInt16();
            GroupNameIndex = br.ReadUInt16();

            var count = br.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                PropertyNameIndices.Add(br.ReadUInt16());
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(ObjectNameIndex);
            bw.Write(GroupNameIndex);
            bw.Write((ushort) PropertyNameIndices.Count);
            
            foreach (var nameIndex in PropertyNameIndices)
            {
                bw.Write(nameIndex);
            }
        }

        public string GetObjectName(IStringTable stringTable) => stringTable.GetFromIndex(ObjectNameIndex);

        public string GetGroupName(IStringTable stringTable) => stringTable.GetFromIndex(GroupNameIndex);

        public List<string> GetPropertyNames(IStringTable stringTable)
        {
            return PropertyNameIndices.Select(stringTable.GetFromIndex).ToList();
        }
    }

    [PublicAPI]
    public class DebugStructOrder : IDebugStructOrder
    {
        public ushort ObjectNameIndex { get; set; }
        public ushort OrderNameIndex { get; set; }
        public List<ushort> NameIndices { get; set; } = new();
        
        public DebugStructOrder() { }
        public DebugStructOrder(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            ObjectNameIndex = br.ReadUInt16();
            OrderNameIndex = br.ReadUInt16();

            var count = br.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                NameIndices.Add(br.ReadUInt16());
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(ObjectNameIndex);
            bw.Write(OrderNameIndex);
            bw.Write((ushort) NameIndices.Count);
            
            foreach (var nameIndex in NameIndices)
            {
                bw.Write(nameIndex);
            }
        }

        public string GetObjectName(IStringTable stringTable) => stringTable.GetFromIndex(ObjectNameIndex);

        public string GetOrderName(IStringTable stringTable) => stringTable.GetFromIndex(OrderNameIndex);

        public List<string> GetNames(IStringTable stringTable)
        {
            return NameIndices.Select(stringTable.GetFromIndex).ToList();
        }
    }
}
