using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Pex;

public partial class DebugInfo
{
    private readonly GameCategory _gameCategory;

    public DebugInfo(GameCategory gameCategory)
    {
        _gameCategory = gameCategory;
    }

    internal static DebugInfo Create(PexParseMeta parse)
    {
        var ret = new DebugInfo(parse.Category);

        ret.ModificationTime = parse.Reader.ReadUInt64().ToDateTime();

        var functionCount = parse.Reader.ReadUInt16();
        for (var i = 0; i < functionCount; i++)
        {
            var function = DebugFunction.Create(parse);
            ret.Functions.Add(function);
        }

        //F04 only
        if (ret._gameCategory != GameCategory.Fallout4) return ret;

        var propertyGroupsCount = parse.Reader.ReadUInt16();
        for (var i = 0; i < propertyGroupsCount; i++)
        {
            var propertyGroup = DebugPropertyGroup.Create(parse);
            ret.PropertyGroups.Add(propertyGroup);
        }

        var structOrderCount = parse.Reader.ReadUInt16();
        for (var i = 0; i < structOrderCount; i++)
        {
            var structOrder = DebugStructOrder.Create(parse);
            ret.StructOrders.Add(structOrder);
        }
        return ret;
    }

    internal void Write(PexWriteMeta bw)
    {
        bw.Writer.Write(ModificationTime.ToUInt64());

        bw.Writer.Write((ushort)Functions.Count);
        foreach (var debugFunction in Functions)
        {
            debugFunction.Write(bw);
        }

        //F04 only
        if (_gameCategory != GameCategory.Fallout4) return;

        bw.Writer.Write((ushort)PropertyGroups.Count);
        foreach (var propertyGroup in PropertyGroups)
        {
            propertyGroup.Write(bw);
        }

        bw.Writer.Write((ushort)StructOrders.Count);
        foreach (var structOrder in StructOrders)
        {
            structOrder.Write(bw);
        }
    }
}

public partial class DebugFunction
{
    internal static DebugFunction Create(PexParseMeta parse)
    {
        var ret = new DebugFunction();
        ret.ObjectName = parse.ReadString();
        ret.StateName = parse.ReadString();
        ret.FunctionName = parse.ReadString();

        ret.FunctionType = (DebugFunctionType)parse.Reader.ReadUInt8();

        var instructionCount = parse.Reader.ReadUInt16();
        for (var i = 0; i < instructionCount; i++)
        {
            var lineNumber = parse.Reader.ReadUInt16();
            ret.Instructions.Add(lineNumber);
        }
        return ret;
    }

    internal void Write(PexWriteMeta meta)
    {
        meta.WriteString(ObjectName);
        meta.WriteString(StateName);
        meta.WriteString(FunctionName);
        meta.Writer.Write((byte)FunctionType);

        meta.Writer.Write((ushort)Instructions.Count);
        foreach (var lineNumber in Instructions)
        {
            meta.Writer.Write(lineNumber);
        }
    }
}

public partial class DebugPropertyGroup
{
    internal static DebugPropertyGroup Create(PexParseMeta parse)
    {
        var ret = new DebugPropertyGroup();
        ret.ObjectName = parse.ReadString();
        ret.GroupName = parse.ReadString();

        var count = parse.Reader.ReadUInt16();
        for (var i = 0; i < count; i++)
        {
            ret.PropertyNames.Add(parse.ReadString());
        }
        return ret;
    }

    internal void Write(PexWriteMeta meta)
    {
        meta.WriteString(ObjectName);
        meta.WriteString(GroupName);
        meta.Writer.Write((ushort)PropertyNames.Count);

        foreach (var name in PropertyNames)
        {
            meta.WriteString(name);
        }
    }
}

public partial class DebugStructOrder
{
    internal static DebugStructOrder Create(PexParseMeta parse)
    {
        var ret = new DebugStructOrder();
        ret.ObjectName = parse.ReadString();
        ret.OrderName = parse.ReadString();

        var count = parse.Reader.ReadUInt16();
        for (var i = 0; i < count; i++)
        {
            ret.Names.Add(parse.ReadString());
        }
        return ret;
    }

    internal void Write(PexWriteMeta meta)
    {
        meta.WriteString(ObjectName);
        meta.WriteString(OrderName);
        meta.Writer.Write((ushort)Names.Count);

        foreach (var name in Names)
        {
            meta.WriteString(name);
        }
    }
}