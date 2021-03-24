using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Mutagen.Bethesda.Core.Pex.Enums;
using Mutagen.Bethesda.Core.Pex.Exceptions;
using Mutagen.Bethesda.Core.Pex.Interfaces;

namespace Mutagen.Bethesda.Core.Pex.DataTypes
{
    [PublicAPI]
    public class PexObject : IPexObject
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public ushort ParentClassNameIndex { get; set; } = ushort.MaxValue;
        public ushort DocStringIndex { get; set; } = ushort.MaxValue;
        public bool IsConst { get; set; }
        public uint UserFlags { get; set; } = uint.MaxValue;
        public ushort AutoStateNameIndex { get; set; } = ushort.MaxValue;
        public List<IPexObjectStructInfo> StructInfos { get; set; } = new();
        public List<IPexObjectVariable> Variables { get; set; } = new();
        public List<IPexObjectProperty> Properties { get; set; } = new();
        public List<IPexObjectState> States { get; set; } = new();
        
        public string GetParentClassName(IStringTable stringTable) => stringTable.GetFromIndex(ParentClassNameIndex);

        public string GetDocString(IStringTable stringTable) => stringTable.GetFromIndex(DocStringIndex);

        public string GetAutoStateName(IStringTable stringTable) => stringTable.GetFromIndex(AutoStateNameIndex);

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);
        
        public List<IUserFlag> GetUserFlags(IUserFlagsTable userFlagsTable) => userFlagsTable.GetUserFlags(UserFlags);

        private readonly GameCategory _gameCategory;
        
        public PexObject(GameCategory gameCategory)
        {
            _gameCategory = gameCategory;
        }
        
        public PexObject(BinaryReader br, GameCategory gameCategory) : this(gameCategory) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16();
            
            /*
             * This is the size of the entire object in bytes not some count variable for a loop. This also includes
             * the size of itself thus the - sizeof(uint)
             */
            var size = br.ReadUInt32() - sizeof(uint);
            var currentPos = br.BaseStream.Position;
            
            ParentClassNameIndex = br.ReadUInt16();
            DocStringIndex = br.ReadUInt16();
            if (_gameCategory == GameCategory.Fallout4)
                IsConst = br.ReadBoolean();
            UserFlags = br.ReadUInt32();
            AutoStateNameIndex = br.ReadUInt16();

            if (_gameCategory == GameCategory.Fallout4)
            {
                var infoCount = br.ReadUInt16();
                for (var i = 0; i < infoCount; i++)
                {
                    var structInfo = new PexObjectStructInfo(br);
                    StructInfos.Add(structInfo);
                }
            }
            
            var variables = br.ReadUInt16();
            for (var i = 0; i < variables; i++)
            {
                var variable = new PexObjectVariable(br);
                Variables.Add(variable);
            }

            var properties = br.ReadUInt16();
            for (var i = 0; i < properties; i++)
            {
                var property = new PexObjectProperty(br);
                Properties.Add(property);
            }

            var states = br.ReadUInt16();
            for (var i = 0; i < states; i++)
            {
                var state = new PexObjectState(br);
                States.Add(state);
            }

            var newPos = br.BaseStream.Position;
            if (newPos != currentPos + size)
                throw new PexParsingException("Current position in Stream does not match expected position: " +
                                              $"Current: {newPos} Expected: {currentPos + size}");
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(NameIndex);

            //needed for later changing
            var currentPos = bw.BaseStream.Position;
            bw.Write(sizeof(uint));

            bw.Write(ParentClassNameIndex);
            bw.Write(DocStringIndex);
            
            if (_gameCategory == GameCategory.Fallout4) {
                // ReSharper disable RedundantCast
                bw.Write(IsConst ? (byte) 1 : (byte) 0);
                // ReSharper restore RedundantCast
            }
            
            bw.Write(UserFlags);
            bw.Write(AutoStateNameIndex);

            if (_gameCategory == GameCategory.Fallout4)
            {
                bw.Write((ushort) StructInfos.Count);
                foreach (var structInfo in StructInfos)
                {
                    structInfo.Write(bw);
                }
            }
            
            bw.Write((ushort) Variables.Count);
            foreach (var objectVariable in Variables)
            {
                objectVariable.Write(bw);
            }
            
            bw.Write((ushort) Properties.Count);
            foreach (var objectProperty in Properties)
            {
                objectProperty.Write(bw);
            }
            
            bw.Write((ushort) States.Count);
            foreach (var objectState in States)
            {
                objectState.Write(bw);
            }
            
            //calculate object size, go back, change it and return to the current position
            var newPos = bw.BaseStream.Position;
            bw.BaseStream.Position = currentPos;

            var objectSize = newPos - currentPos;
            bw.Write((uint) objectSize);

            bw.BaseStream.Position = newPos;
        }
    }

    [PublicAPI]
    public class PexObjectStructInfo : IPexObjectStructInfo
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public List<IPexObjectStructInfoMember> Members { get; set; } = new();
        
        public PexObjectStructInfo() { }
        public PexObjectStructInfo(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16();

            var count = br.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                var member = new PexObjectStructInfoMember(br);
                Members.Add(member);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(NameIndex);
            bw.Write((ushort) Members.Count);
            foreach (var infoMember in Members)
            {
                infoMember.Write(bw);
            }
        }

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);
    }

    [PublicAPI]
    public class PexObjectStructInfoMember : IPexObjectStructInfoMember
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public ushort TypeNameIndex { get; set; } = ushort.MaxValue;
        public uint UserFlags { get; set; } = ushort.MaxValue;
        public IPexObjectVariableData? Value { get; set; }
        public bool IsConst { get; set; }
        public ushort DocStringIndex { get; set; } = ushort.MaxValue;
        
        public PexObjectStructInfoMember() { }
        public PexObjectStructInfoMember(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16();
            TypeNameIndex = br.ReadUInt16();
            UserFlags = br.ReadUInt32();
            Value = new PexObjectVariableData(br);
            IsConst = br.ReadBoolean();
            DocStringIndex = br.ReadUInt16();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(NameIndex);
            bw.Write(TypeNameIndex);
            bw.Write(UserFlags);
            Value?.Write(bw);
            bw.Write(IsConst);
            bw.Write(DocStringIndex);
        }

        public List<IUserFlag> GetUserFlags(IUserFlagsTable userFlagsTable) => userFlagsTable.GetUserFlags(UserFlags);

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public string GetTypeName(IStringTable stringTable) => stringTable.GetFromIndex(TypeNameIndex);

        public string GetDocString(IStringTable stringTable) => stringTable.GetFromIndex(DocStringIndex);
    }
    
    [PublicAPI]
    public class PexObjectVariable : IPexObjectVariable
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public ushort TypeNameIndex { get; set; } = ushort.MaxValue;
        public uint UserFlags { get; set; } = uint.MaxValue;
        public IPexObjectVariableData? VariableData { get; set; }
        
        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public string GetTypeName(IStringTable stringTable) => stringTable.GetFromIndex(TypeNameIndex);
        
        public List<IUserFlag> GetUserFlags(IUserFlagsTable userFlagsTable) => userFlagsTable.GetUserFlags(UserFlags);
        
        public PexObjectVariable() { }
        public PexObjectVariable(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16();
            TypeNameIndex = br.ReadUInt16();
            UserFlags = br.ReadUInt32();

            VariableData = new PexObjectVariableData(br);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(NameIndex);
            bw.Write(TypeNameIndex);
            bw.Write(UserFlags);
            
            VariableData?.Write(bw);
        }
    }

    [PublicAPI]
    public class PexObjectVariableData : IPexObjectVariableData
    {
        public VariableType VariableType { get; set; } = VariableType.Null;
        public ushort? StringValueIndex { get; set; }
        public int? IntValue { get; set; }
        public float? FloatValue { get; set; }
        public bool? BoolValue { get; set; }

        public string? GetStringValue(IStringTable stringTable) =>
            StringValueIndex.HasValue ? stringTable.GetFromIndex(StringValueIndex.Value) : null; 
        
        public PexObjectVariableData() { }
        public PexObjectVariableData(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            VariableType = (VariableType) br.ReadByte();
            switch (VariableType)
            {
                case VariableType.Null:
                    break;
                case VariableType.Identifier:
                case VariableType.String:
                    StringValueIndex = br.ReadUInt16();
                    break;
                case VariableType.Integer:
                    IntValue = br.ReadInt32();
                    break;
                case VariableType.Float:
                    FloatValue = br.ReadSingle();
                    break;
                case VariableType.Bool:
                    //TODO: use ReadByte instead?
                    BoolValue = br.ReadBoolean();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((byte) VariableType);
            switch (VariableType)
            {
                case VariableType.Null:
                    break;
                case VariableType.Identifier:
                case VariableType.String:
                    bw.Write(StringValueIndex ?? ushort.MaxValue);
                    break;
                case VariableType.Integer:
                    bw.Write(IntValue ?? int.MaxValue);
                    break;
                case VariableType.Float:
                    bw.Write(FloatValue ?? float.MaxValue);
                    break;
                case VariableType.Bool:
                    bw.Write(BoolValue ?? false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [PublicAPI]
    public class PexObjectProperty : IPexObjectProperty
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public ushort TypeNameIndex { get; set; } = ushort.MaxValue;
        public ushort DocStringIndex { get; set; } = ushort.MaxValue;
        public uint UserFlags { get; set; } = uint.MaxValue;
        public PropertyFlags Flags { get; set; }
        public ushort? AutoVarNameIndex { get; set; }
        public IPexObjectFunction? ReadHandler { get; set; }
        public IPexObjectFunction? WriteHandler { get; set; }

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public string GetTypeName(IStringTable stringTable) => stringTable.GetFromIndex(TypeNameIndex);

        public string? GetAutoVarName(IStringTable stringTable) =>
            AutoVarNameIndex.HasValue ? stringTable.GetFromIndex(AutoVarNameIndex.Value) : null;
        
        public List<IUserFlag> GetUserFlags(IUserFlagsTable userFlagsTable) => userFlagsTable.GetUserFlags(UserFlags);
        
        public PexObjectProperty() { }
        public PexObjectProperty(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16();
            TypeNameIndex = br.ReadUInt16();
            DocStringIndex = br.ReadUInt16();
            UserFlags = br.ReadUInt32();

            var flags = br.ReadByte();
            Flags = (PropertyFlags) flags;
            
            if ((flags & 4) != 0)
            {
                AutoVarNameIndex = br.ReadUInt16();
            }

            if ((flags & 5) == 1)
            {
                ReadHandler = new PexObjectFunction(br);
            }

            if ((flags & 6) == 2)
            {
                WriteHandler = new PexObjectFunction(br);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(NameIndex);
            bw.Write(TypeNameIndex);
            bw.Write(DocStringIndex);
            bw.Write(UserFlags);

            var flags = (byte) Flags;
            bw.Write(flags);
            
            if ((flags & 4) != 0)
            {
                bw.Write(AutoVarNameIndex ?? ushort.MaxValue);
            }

            if ((flags & 5) == 1)
            {
                ReadHandler?.Write(bw);
            }

            if ((flags & 6) == 2)
            {
                WriteHandler?.Write(bw);
            }
        }
    }

    [PublicAPI]
    public class PexObjectState : IPexObjectState
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;

        public List<IPexObjectNamedFunction> Functions { get; set; } = new();

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);
        
        public PexObjectState() { }
        public PexObjectState(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16();

            var functions = br.ReadUInt16();
            for (var i = 0; i < functions; i++)
            {
                var namedFunction = new PexObjectNamedFunction(br);
                Functions.Add(namedFunction);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(NameIndex);
            bw.Write((ushort) Functions.Count);
            foreach (var namedFunction in Functions)
            {
                namedFunction.Write(bw);
            }
        }
    }

    [PublicAPI]
    public class PexObjectNamedFunction : IPexObjectNamedFunction
    {
        public ushort FunctionNameIndex { get; set; } = ushort.MaxValue;
        
        public IPexObjectFunction? Function { get; set; }

        public string GetFunctionName(IStringTable stringTable) => stringTable.GetFromIndex(FunctionNameIndex);
        
        public PexObjectNamedFunction() { }
        public PexObjectNamedFunction(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            FunctionNameIndex = br.ReadUInt16();
            Function = new PexObjectFunction(br);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(FunctionNameIndex);
            Function?.Write(bw);
        }
    }

    [PublicAPI]
    public class PexObjectFunction : IPexObjectFunction
    {
        public ushort ReturnTypeNameIndex { get; set; } = ushort.MaxValue;
        public ushort DocStringIndex { get; set; } = ushort.MaxValue;
        public uint UserFlags { get; set; } = uint.MaxValue;
        public FunctionFlags Flags { get; set; }
        public List<IPexObjectFunctionVariable> Parameters { get; set; } = new();
        public List<IPexObjectFunctionVariable> Locals { get; set; } = new();
        public List<IPexObjectFunctionInstruction> Instructions { get; set; } = new();
        
        public string GetReturnTypeName(IStringTable stringTable) => stringTable.GetFromIndex(ReturnTypeNameIndex);
        public string GetDocString(IStringTable stringTable) => stringTable.GetFromIndex(DocStringIndex);
        public List<IUserFlag> GetUserFlags(IUserFlagsTable userFlagsTable) => userFlagsTable.GetUserFlags(UserFlags);
        
        public PexObjectFunction() { }
        public PexObjectFunction(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            ReturnTypeNameIndex = br.ReadUInt16();
            DocStringIndex = br.ReadUInt16();
            UserFlags = br.ReadUInt32();
            Flags = (FunctionFlags) br.ReadByte();

            var parameters = br.ReadUInt16();
            for (var i = 0; i < parameters; i++)
            {
                var parameter = new PexObjectFunctionVariable(br);
                Parameters.Add(parameter);
            }
            
            var locals = br.ReadUInt16();
            for (var i = 0; i < locals; i++)
            {
                var local = new PexObjectFunctionVariable(br);
                Locals.Add(local);
            }
            
            var instructions = br.ReadUInt16();
            for (var i = 0; i < instructions; i++)
            {
                var instruction = new PexObjectFunctionInstruction(br);
                Instructions.Add(instruction);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(ReturnTypeNameIndex);
            bw.Write(DocStringIndex);
            bw.Write(UserFlags);
            bw.Write((byte) Flags);
            
            bw.Write((ushort) Parameters.Count);
            foreach (var parameter in Parameters)
            {
                parameter.Write(bw);
            }
            
            bw.Write((ushort) Locals.Count);
            foreach (var local in Locals)
            {
                local.Write(bw);
            }
            
            bw.Write((ushort) Instructions.Count);
            foreach (var instruction in Instructions)
            {
                instruction.Write(bw);
            }
        }
    }

    [PublicAPI]
    public class PexObjectFunctionVariable : IPexObjectFunctionVariable
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public ushort TypeNameIndex { get; set; } = ushort.MaxValue;
        
        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public string GetTypeName(IStringTable stringTable) => stringTable.GetFromIndex(TypeNameIndex);
        
        public PexObjectFunctionVariable() { }
        public PexObjectFunctionVariable(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16();
            TypeNameIndex = br.ReadUInt16();
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(NameIndex);
            bw.Write(TypeNameIndex);
        }
    }

    [PublicAPI]
    public class PexObjectFunctionInstruction : IPexObjectFunctionInstruction
    {
        public InstructionOpcode OpCode { get; set; } = InstructionOpcode.NOP;
        public List<IPexObjectVariableData> Arguments { get; set; } = new();
        
        public PexObjectFunctionInstruction() { }
        public PexObjectFunctionInstruction(BinaryReader br) { Read(br); }

        public void Read(BinaryReader br)
        {
            OpCode = (InstructionOpcode) br.ReadByte();
            
            var arguments = InstructionOpCodeArguments.GetArguments(OpCode);
            foreach (var current in arguments)
            {
                var argument = new PexObjectVariableData(br);
                Arguments.Add(argument);

                switch (current)
                {
                    case '*' when argument.VariableType != VariableType.Integer || !argument.IntValue.HasValue:
                        throw new PexParsingException($"Variable-Length Arguments require an Integer Argument! Argument is {argument.VariableType}");
                    case '*':
                    {
                        for (var i = 0; i < argument.IntValue.Value; i++)
                        {
                            var anotherArgument = new PexObjectVariableData(br);
                            Arguments.Add(anotherArgument);
                        }

                        break;
                    }
                    //TODO: figure out what do to with this
                    /*
                     * u apparently means unsigned integer and indicates that the integer value we get should be
                     * interpreted as an unsigned integer.
                     */
                    case 'u' when argument.VariableType != VariableType.Integer:
                        throw new PexParsingException($"Argument is unsigned integer but Variable Type is not integer: {argument.VariableType}");
                }
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((byte) OpCode);

            foreach (var argument in Arguments)
            {
                argument.Write(bw);
            }
        }
    }
}
