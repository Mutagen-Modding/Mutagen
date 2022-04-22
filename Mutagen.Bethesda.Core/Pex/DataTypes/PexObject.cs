using System;
using System.IO;

namespace Mutagen.Bethesda.Pex;

public partial class PexObject
{
    internal static PexObject Create(PexParseMeta parse)
    {
        var ret = new PexObject();
        ret.Name = parse.ReadString();

        /*
         * This is the size of the entire object in bytes not some count variable for a loop. This also includes
         * the size of itself thus the - sizeof(uint)
         */
        var size = parse.Reader.ReadUInt32() - sizeof(uint);
        var currentPos = parse.Reader.Position;

        ret.ParentClassName = parse.ReadString();
        ret.DocString = parse.ReadString();

        if (parse.Category == GameCategory.Fallout4)
            ret.IsConst = parse.Reader.ReadBoolean();

        ret.RawUserFlags = parse.Reader.ReadUInt32();
        ret.AutoStateName = parse.ReadString();

        if (parse.Category == GameCategory.Fallout4)
        {
            var infoCount = parse.Reader.ReadUInt16();
            for (var i = 0; i < infoCount; i++)
            {
                var structInfo = PexObjectStructInfo.Create(parse);
                ret.StructInfos.Add(structInfo);
            }
        }

        var variables = parse.Reader.ReadUInt16();
        for (var i = 0; i < variables; i++)
        {
            var variable = PexObjectVariable.Create(parse);
            ret.Variables.Add(variable);
        }

        var properties = parse.Reader.ReadUInt16();
        for (var i = 0; i < properties; i++)
        {
            var property = PexObjectProperty.Create(parse);
            ret.Properties.Add(property);
        }

        var states = parse.Reader.ReadUInt16();
        for (var i = 0; i < states; i++)
        {
            var state = PexObjectState.Create(parse);
            ret.States.Add(state);
        }

        var newPos = parse.Reader.Position;
        if (newPos != currentPos + size)
            throw new InvalidDataException("Current position in Stream does not match expected position: " +
                                           $"Current: {newPos} Expected: {currentPos + size}");

        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.WriteString(Name);

        //needed for later changing
        var currentPos = write.Writer.BaseStream.Position;
        write.Writer.Write(sizeof(uint));

        write.WriteString(ParentClassName);
        write.WriteString(DocString);

        if (write.Category == GameCategory.Fallout4)
        {
            write.Writer.Write(IsConst ? (byte)1 : (byte)0);
        }

        write.Writer.Write(RawUserFlags);
        write.WriteString(AutoStateName);

        if (write.Category == GameCategory.Fallout4)
        {
            write.Writer.Write((ushort)StructInfos.Count);
            foreach (var structInfo in StructInfos)
            {
                structInfo.Write(write);
            }
        }

        write.Writer.Write((ushort)Variables.Count);
        foreach (var objectVariable in Variables)
        {
            objectVariable.Write(write);
        }

        write.Writer.Write((ushort)Properties.Count);
        foreach (var objectProperty in Properties)
        {
            objectProperty.Write(write);
        }

        write.Writer.Write((ushort)States.Count);
        foreach (var objectState in States)
        {
            objectState.Write(write);
        }

        //calculate object size, go back, change it and return to the current position
        var newPos = write.Writer.BaseStream.Position;
        write.Writer.BaseStream.Position = currentPos;

        var objectSize = newPos - currentPos;
        write.Writer.Write((uint)objectSize);

        write.Writer.BaseStream.Position = newPos;
    }
}

public partial class PexObjectStructInfo
{
    internal static PexObjectStructInfo Create(PexParseMeta parse)
    {
        var ret = new PexObjectStructInfo();
        ret.Name = parse.ReadString();

        var count = parse.Reader.ReadUInt16();
        for (var i = 0; i < count; i++)
        {
            var member = PexObjectStructInfoMember.Create(parse);
            ret.Members.Add(member);
        }
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.WriteString(Name);
        write.Writer.Write((ushort)Members.Count);
        foreach (var infoMember in Members)
        {
            infoMember.Write(write);
        }
    }
}

public partial class PexObjectStructInfoMember
{
    internal static PexObjectStructInfoMember Create(PexParseMeta parse)
    {
        var ret = new PexObjectStructInfoMember();
        ret.Name = parse.ReadString();
        ret.TypeName = parse.ReadString();
        ret.RawUserFlags = parse.Reader.ReadUInt32();
        ret.Value = PexObjectVariableData.Create(parse);
        ret.IsConst = parse.Reader.ReadBoolean();
        ret.DocString = parse.ReadString();
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.WriteString(Name);
        write.WriteString(TypeName);
        write.Writer.Write(RawUserFlags);
        Value?.Write(write);
        write.Writer.Write(IsConst);
        write.WriteString(DocString);
    }
}
    
public partial class PexObjectVariable
{
    internal static PexObjectVariable Create(PexParseMeta parse)
    {
        var ret = new PexObjectVariable();
        ret.Name = parse.ReadString();
        ret.TypeName = parse.ReadString();
        ret.RawUserFlags = parse.Reader.ReadUInt32();

        ret.VariableData = PexObjectVariableData.Create(parse);
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.WriteString(Name);
        write.WriteString(TypeName);
        write.Writer.Write(RawUserFlags);

        VariableData?.Write(write);
    }
}

public partial class PexObjectVariableData
{
    internal static PexObjectVariableData Create(PexParseMeta parse)
    {
        var ret = new PexObjectVariableData();
        ret.VariableType = (VariableType)parse.Reader.ReadUInt8();
        switch (ret.VariableType)
        {
            case VariableType.Null:
                break;
            case VariableType.Identifier:
            case VariableType.String:
                ret.StringValue = parse.ReadString();
                break;
            case VariableType.Integer:
                ret.IntValue = parse.Reader.ReadInt32();
                break;
            case VariableType.Float:
                ret.FloatValue = parse.Reader.ReadFloat();
                break;
            case VariableType.Bool:
                //TODO: use ReadByte instead?
                ret.BoolValue = parse.Reader.ReadBoolean();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.Writer.Write((byte)VariableType);
        switch (VariableType)
        {
            case VariableType.Null:
                break;
            case VariableType.Identifier:
            case VariableType.String:
                write.WriteString(StringValue);
                break;
            case VariableType.Integer:
                write.Writer.Write(IntValue ?? int.MaxValue);
                break;
            case VariableType.Float:
                write.Writer.Write(FloatValue ?? float.MaxValue);
                break;
            case VariableType.Bool:
                write.Writer.Write(BoolValue ?? false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public partial class PexObjectProperty
{
    internal static PexObjectProperty Create(PexParseMeta parse)
    {
        var ret = new PexObjectProperty();
        ret.Name = parse.ReadString();
        ret.TypeName = parse.ReadString();
        ret.DocString = parse.ReadString();
        ret.RawUserFlags = parse.Reader.ReadUInt32();

        var flags = parse.Reader.ReadUInt8();
        ret.Flags = (PropertyFlags)flags;

        if ((flags & 4) != 0)
        {
            ret.AutoVarName = parse.ReadString();
        }

        if ((flags & 5) == 1)
        {
            ret.ReadHandler = PexObjectFunction.Create(parse);
        }

        if ((flags & 6) == 2)
        {
            ret.WriteHandler = PexObjectFunction.Create(parse);
        }
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.WriteString(Name);
        write.WriteString(TypeName);
        write.WriteString(DocString);
        write.Writer.Write(RawUserFlags);

        var flags = (byte)Flags;
        write.Writer.Write(flags);

        if ((flags & 4) != 0)
        {
            write.WriteString(AutoVarName);
        }

        if ((flags & 5) == 1)
        {
            ReadHandler?.Write(write);
        }

        if ((flags & 6) == 2)
        {
            WriteHandler?.Write(write);
        }
    }
}

public partial class PexObjectState
{
    internal static PexObjectState Create(PexParseMeta parse)
    {
        var ret = new PexObjectState();
        ret.Name = parse.ReadString();

        var functions = parse.Reader.ReadUInt16();
        for (var i = 0; i < functions; i++)
        {
            var namedFunction = PexObjectNamedFunction.Create(parse);
            ret.Functions.Add(namedFunction);
        }
        return ret;
    }

    internal void Write(PexWriteMeta writer)
    {
        writer.WriteString(Name);
        writer.Writer.Write((ushort)Functions.Count);
        foreach (var namedFunction in Functions)
        {
            namedFunction.Write(writer);
        }
    }
}

public partial class PexObjectNamedFunction
{
    internal static PexObjectNamedFunction Create(PexParseMeta parse)
    {
        var ret = new PexObjectNamedFunction();
        ret.FunctionName = parse.ReadString();
        ret.Function = PexObjectFunction.Create(parse);
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.WriteString(FunctionName);
        Function?.Write(write);
    }
}

public partial class PexObjectFunction
{
    internal static PexObjectFunction Create(PexParseMeta parse)
    {
        var ret = new PexObjectFunction();
        ret.ReturnTypeName = parse.ReadString();
        ret.DocString = parse.ReadString();
        ret.RawUserFlags = parse.Reader.ReadUInt32();
        ret.Flags = (FunctionFlags)parse.Reader.ReadUInt8();

        var parameters = parse.Reader.ReadUInt16();
        for (var i = 0; i < parameters; i++)
        {
            var parameter = PexObjectFunctionVariable.Create(parse);
            ret.Parameters.Add(parameter);
        }

        var locals = parse.Reader.ReadUInt16();
        for (var i = 0; i < locals; i++)
        {
            var local = PexObjectFunctionVariable.Create(parse);
            ret.Locals.Add(local);
        }

        var instructions = parse.Reader.ReadUInt16();
        for (var i = 0; i < instructions; i++)
        {
            var instruction = PexObjectFunctionInstruction.Create(parse);
            ret.Instructions.Add(instruction);
        }
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.WriteString(ReturnTypeName);
        write.WriteString(DocString);
        write.Writer.Write(RawUserFlags);
        write.Writer.Write((byte)Flags);

        write.Writer.Write((ushort)Parameters.Count);
        foreach (var parameter in Parameters)
        {
            parameter.Write(write);
        }

        write.Writer.Write((ushort)Locals.Count);
        foreach (var local in Locals)
        {
            local.Write(write);
        }

        write.Writer.Write((ushort)Instructions.Count);
        foreach (var instruction in Instructions)
        {
            instruction.Write(write);
        }
    }
}

public partial class PexObjectFunctionVariable
{
    internal static PexObjectFunctionVariable Create(PexParseMeta parse)
    {
        var ret = new PexObjectFunctionVariable();
        ret.Name = parse.ReadString();
        ret.TypeName = parse.ReadString();
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.WriteString(Name);
        write.WriteString(TypeName);
    }
}

public partial class PexObjectFunctionInstruction
{
    internal static PexObjectFunctionInstruction Create(PexParseMeta parse)
    {
        var ret = new PexObjectFunctionInstruction();
        ret.OpCode = (InstructionOpcode)parse.Reader.ReadUInt8();

        var arguments = InstructionOpCodeArguments.GetArguments(ret.OpCode);
        foreach (var current in arguments)
        {
            var argument = PexObjectVariableData.Create(parse);
            ret.Arguments.Add(argument);

            switch (current)
            {
                case '*' when argument.VariableType != VariableType.Integer || !argument.IntValue.HasValue:
                    throw new InvalidDataException($"Variable-Length Arguments require an Integer Argument! Argument is {argument.VariableType}");
                case '*':
                {
                    for (var i = 0; i < argument.IntValue.Value; i++)
                    {
                        var anotherArgument = PexObjectVariableData.Create(parse);
                        ret.Arguments.Add(anotherArgument);
                    }

                    break;
                }
                //TODO: figure out what do to with this
                /*
                 * u apparently means unsigned integer and indicates that the integer value we get should be
                 * interpreted as an unsigned integer.
                 */
                case 'u' when argument.VariableType != VariableType.Integer:
                    throw new InvalidDataException($"Argument is unsigned integer but Variable Type is not integer: {argument.VariableType}");
            }
        }
        return ret;
    }

    internal void Write(PexWriteMeta write)
    {
        write.Writer.Write((byte)OpCode);

        foreach (var argument in Arguments)
        {
            argument.Write(write);
        }
    }
}