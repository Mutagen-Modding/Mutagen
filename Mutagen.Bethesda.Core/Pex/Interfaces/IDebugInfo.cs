using System;
using System.Collections.Generic;
using Noggog;

namespace Mutagen.Bethesda.Pex
{
    public interface IDebugInfo : IBinaryObject
    {
        public bool HasDebugInfo { get; set; }
        
        public DateTime ModificationTime { get; set; }
        
        public ExtendedList<IDebugFunction> Functions { get; set; }
        
        public ExtendedList<IDebugPropertyGroup> PropertyGroups { get; set; }
        
        public ExtendedList<IDebugStructOrder> StructOrders { get; set; }
    }

    public interface IDebugFunction : IBinaryObject
    {
        public string? ObjectName { get; set; }
        public string? StateName { get; set; }
        public string? FunctionName { get; set; }
        
        public DebugFunctionType FunctionType { get; set; }
        
        public ExtendedList<ushort> Instructions { get; set; }
    }

    public interface IDebugPropertyGroup : IBinaryObject
    {
        public string? ObjectName { get; set; }
        public string? GroupName { get; set; }
        public ExtendedList<string> PropertyNames { get; set; }
    }

    public interface IDebugStructOrder : IBinaryObject
    {
        public string? ObjectName { get; set; }
        public string? OrderName { get; set; }
        public ExtendedList<string> Names { get; set; }
    }
}
