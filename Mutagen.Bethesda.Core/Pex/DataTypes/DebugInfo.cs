using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Pex
{
    public partial interface IDebugInfo : IBinaryObject
    {
    }

    public partial class DebugInfo
    {
        private readonly GameCategory _gameCategory;
        private readonly PexFile _pexFile = null!;
        
        public DebugInfo(GameCategory gameCategory, PexFile pexFile)
        {
            _gameCategory = gameCategory;
            _pexFile = pexFile;
        }
        
        public DebugInfo(BinaryReader br, GameCategory gameCategory, PexFile pexFile)
            : this(gameCategory, pexFile) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            var hasDebugInfo = br.ReadByte();
            HasDebugInfo = hasDebugInfo == 1;
            if (!HasDebugInfo) return;

            ModificationTime = br.ReadUInt64().ToDateTime();
            
            var functionCount = br.ReadUInt16();
            for (var i = 0; i < functionCount; i++)
            {
                var function = new DebugFunction(br, _pexFile);
                Functions.Add(function);
            }

            //F04 only
            if (_gameCategory != GameCategory.Fallout4) return;
            
            var propertyGroupsCount = br.ReadUInt16();
            for (var i = 0; i < propertyGroupsCount; i++)
            {
                var propertyGroup = new DebugPropertyGroup(br, _pexFile);
                PropertyGroups.Add(propertyGroup);
            }
            
            var structOrderCount = br.ReadUInt16();
            for (var i = 0; i < structOrderCount; i++)
            {
                var structOrder = new DebugStructOrder(br, _pexFile);
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

    public partial class DebugFunction
    {
        private readonly PexFile _pexFile = null!;

        public DebugFunction(PexFile pexFile)
        {
            _pexFile = pexFile;
        }

        public DebugFunction(BinaryReader br, PexFile pexFile) : this(pexFile)
        {
            Read(br);
        }
        
        public void Read(BinaryReader br)
        {
            ObjectName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            StateName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            FunctionName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            
            FunctionType = (DebugFunctionType) br.ReadByte();
            
            var instructionCount = br.ReadUInt16();
            for (var i = 0; i < instructionCount; i++)
            {
                var lineNumber = br.ReadUInt16();
                Instructions.Add(lineNumber);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(ObjectName));
            bw.Write(_pexFile.GetIndexFromString(StateName));
            bw.Write(_pexFile.GetIndexFromString(FunctionName));
            bw.Write((byte) FunctionType);
            
            bw.Write((ushort) Instructions.Count);
            foreach (var lineNumber in Instructions)
            {
                bw.Write(lineNumber);
            }
        }
    }

    public partial class DebugPropertyGroup
    {
        private readonly PexFile _pexFile = null!;

        public DebugPropertyGroup(PexFile pexFile)
        {
            _pexFile = pexFile;
        }
        
        public DebugPropertyGroup(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            ObjectName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            GroupName = _pexFile.GetStringFromIndex(br.ReadUInt16());

            var count = br.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                PropertyNames.Add(_pexFile.GetStringFromIndex(br.ReadUInt16()));
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(ObjectName));
            bw.Write(_pexFile.GetIndexFromString(GroupName));
            bw.Write((ushort) PropertyNames.Count);
            
            foreach (var name in PropertyNames)
            {
                bw.Write(_pexFile.GetIndexFromString(name));
            }
        }
    }

    public partial class DebugStructOrder
    {
        private readonly PexFile _pexFile = null!;

        public DebugStructOrder(PexFile pexFile)
        {
            _pexFile = pexFile;
        }
        
        public DebugStructOrder(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            ObjectName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            OrderName = _pexFile.GetStringFromIndex(br.ReadUInt16());

            var count = br.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                Names.Add(_pexFile.GetStringFromIndex(br.ReadUInt16()));
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(ObjectName));
            bw.Write(_pexFile.GetIndexFromString(OrderName));
            bw.Write((ushort) Names.Count);
            
            foreach (var name in Names)
            {
                bw.Write(_pexFile.GetIndexFromString(name));
            }
        }
    }
}
