using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Pex
{
    public partial class PexObject
    {
        IReadOnlyList<IUserFlagGetter> IHasUserFlagsGetter.UserFlags => UserFlags;

        private readonly GameCategory _gameCategory;
        private readonly PexFile _pexFile = null!;
        
        public PexObject(GameCategory gameCategory, PexFile pexFile)
        {
            _gameCategory = gameCategory;
            _pexFile = pexFile;
        }

        public PexObject(BinaryReader br, GameCategory gameCategory, PexFile pexFile)
            : this(gameCategory, pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            Name = _pexFile.GetStringFromIndex(br.ReadUInt16());

            /*
             * This is the size of the entire object in bytes not some count variable for a loop. This also includes
             * the size of itself thus the - sizeof(uint)
             */
            var size = br.ReadUInt32() - sizeof(uint);
            var currentPos = br.BaseStream.Position;

            ParentClassName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            DocString = _pexFile.GetStringFromIndex(br.ReadUInt16());

            if (_gameCategory == GameCategory.Fallout4)
                IsConst = br.ReadBoolean();

            UserFlags = _pexFile.GetUserFlags(br.ReadUInt32()).ToExtendedList();
            AutoStateName = _pexFile.GetStringFromIndex(br.ReadUInt16());

            if (_gameCategory == GameCategory.Fallout4)
            {
                var infoCount = br.ReadUInt16();
                for (var i = 0; i < infoCount; i++)
                {
                    var structInfo = new PexObjectStructInfo(br, _pexFile);
                    StructInfos.Add(structInfo);
                }
            }

            var variables = br.ReadUInt16();
            for (var i = 0; i < variables; i++)
            {
                var variable = new PexObjectVariable(br, _pexFile);
                Variables.Add(variable);
            }

            var properties = br.ReadUInt16();
            for (var i = 0; i < properties; i++)
            {
                var property = new PexObjectProperty(br, _pexFile);
                Properties.Add(property);
            }

            var states = br.ReadUInt16();
            for (var i = 0; i < states; i++)
            {
                var state = new PexObjectState(br, _pexFile);
                States.Add(state);
            }

            var newPos = br.BaseStream.Position;
            if (newPos != currentPos + size)
                throw new InvalidDataException("Current position in Stream does not match expected position: " +
                                              $"Current: {newPos} Expected: {currentPos + size}");
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(Name));

            //needed for later changing
            var currentPos = bw.BaseStream.Position;
            bw.Write(sizeof(uint));

            bw.Write(_pexFile.GetIndexFromString(ParentClassName));
            bw.Write(_pexFile.GetIndexFromString(DocString));

            if (_gameCategory == GameCategory.Fallout4)
            {
                // ReSharper disable RedundantCast
                bw.Write(IsConst ? (byte)1 : (byte)0);
                // ReSharper restore RedundantCast
            }

            bw.Write(_pexFile.GetUserFlags(UserFlags));
            bw.Write(_pexFile.GetIndexFromString(AutoStateName));

            if (_gameCategory == GameCategory.Fallout4)
            {
                bw.Write((ushort)StructInfos.Count);
                foreach (var structInfo in StructInfos)
                {
                    structInfo.Write(bw);
                }
            }

            bw.Write((ushort)Variables.Count);
            foreach (var objectVariable in Variables)
            {
                objectVariable.Write(bw);
            }

            bw.Write((ushort)Properties.Count);
            foreach (var objectProperty in Properties)
            {
                objectProperty.Write(bw);
            }

            bw.Write((ushort)States.Count);
            foreach (var objectState in States)
            {
                objectState.Write(bw);
            }

            //calculate object size, go back, change it and return to the current position
            var newPos = bw.BaseStream.Position;
            bw.BaseStream.Position = currentPos;

            var objectSize = newPos - currentPos;
            bw.Write((uint)objectSize);

            bw.BaseStream.Position = newPos;
        }
    }

    public partial class PexObjectStructInfo
    {
        private readonly PexFile _pexFile = null!;

        public PexObjectStructInfo(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectStructInfo(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            Name = _pexFile.GetStringFromIndex(br.ReadUInt16());

            var count = br.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                var member = new PexObjectStructInfoMember(br, _pexFile);
                Members.Add(member);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(Name));
            bw.Write((ushort)Members.Count);
            foreach (var infoMember in Members)
            {
                infoMember.Write(bw);
            }
        }
    }

    public partial class PexObjectStructInfoMember
    {
        IReadOnlyList<IUserFlagGetter> IHasUserFlagsGetter.UserFlags => UserFlags;

        private readonly PexFile _pexFile = null!;
        public PexObjectStructInfoMember(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectStructInfoMember(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            Name = _pexFile.GetStringFromIndex(br.ReadUInt16());
            TypeName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            UserFlags = _pexFile.GetUserFlags(br.ReadUInt32()).ToExtendedList();
            Value = new PexObjectVariableData(br, _pexFile);
            IsConst = br.ReadBoolean();
            DocString = _pexFile.GetStringFromIndex(br.ReadUInt16());
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(Name));
            bw.Write(_pexFile.GetIndexFromString(TypeName));
            bw.Write(_pexFile.GetUserFlags(UserFlags));
            Value?.Write(bw);
            bw.Write(IsConst);
            bw.Write(_pexFile.GetIndexFromString(DocString));
        }
    }
    
    public partial class PexObjectVariable
    {
        IReadOnlyList<IUserFlagGetter> IHasUserFlagsGetter.UserFlags => UserFlags;

        private readonly PexFile _pexFile = null!;
        public PexObjectVariable(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectVariable(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            Name = _pexFile.GetStringFromIndex(br.ReadUInt16());
            TypeName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            UserFlags = _pexFile.GetUserFlags(br.ReadUInt32()).ToExtendedList();

            VariableData = new PexObjectVariableData(br, _pexFile);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(Name));
            bw.Write(_pexFile.GetIndexFromString(TypeName));
            bw.Write(_pexFile.GetUserFlags(UserFlags));

            VariableData?.Write(bw);
        }
    }

    public partial class PexObjectVariableData
    {
        private readonly PexFile _pexFile = null!;
        public PexObjectVariableData(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectVariableData(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            VariableType = (VariableType)br.ReadByte();
            switch (VariableType)
            {
                case VariableType.Null:
                    break;
                case VariableType.Identifier:
                case VariableType.String:
                    StringValue = _pexFile.GetStringFromIndex(br.ReadUInt16());
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
            bw.Write((byte)VariableType);
            switch (VariableType)
            {
                case VariableType.Null:
                    break;
                case VariableType.Identifier:
                case VariableType.String:
                    bw.Write(_pexFile.GetIndexFromString(StringValue));
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

    public partial class PexObjectProperty
    {
        IReadOnlyList<IUserFlagGetter> IHasUserFlagsGetter.UserFlags => UserFlags;

        private readonly PexFile _pexFile = null!;
        public PexObjectProperty(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectProperty(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            Name = _pexFile.GetStringFromIndex(br.ReadUInt16());
            TypeName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            DocString = _pexFile.GetStringFromIndex(br.ReadUInt16());
            UserFlags = _pexFile.GetUserFlags(br.ReadUInt32()).ToExtendedList();

            var flags = br.ReadByte();
            Flags = (PropertyFlags)flags;

            if ((flags & 4) != 0)
            {
                AutoVarName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            }

            if ((flags & 5) == 1)
            {
                ReadHandler = new PexObjectFunction(br, _pexFile);
            }

            if ((flags & 6) == 2)
            {
                WriteHandler = new PexObjectFunction(br, _pexFile);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(Name));
            bw.Write(_pexFile.GetIndexFromString(TypeName));
            bw.Write(_pexFile.GetIndexFromString(DocString));
            bw.Write(_pexFile.GetUserFlags(UserFlags));

            var flags = (byte)Flags;
            bw.Write(flags);

            if ((flags & 4) != 0)
            {
                bw.Write(_pexFile.GetIndexFromString(AutoVarName));
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

    public partial class PexObjectState
    {
        private readonly PexFile _pexFile = null!;
        public PexObjectState(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectState(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            Name = _pexFile.GetStringFromIndex(br.ReadUInt16());

            var functions = br.ReadUInt16();
            for (var i = 0; i < functions; i++)
            {
                var namedFunction = new PexObjectNamedFunction(br, _pexFile);
                Functions.Add(namedFunction);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(Name));
            bw.Write((ushort)Functions.Count);
            foreach (var namedFunction in Functions)
            {
                namedFunction.Write(bw);
            }
        }
    }

    public partial class PexObjectNamedFunction
    {
        private readonly PexFile _pexFile = null!;
        public PexObjectNamedFunction(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectNamedFunction(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            FunctionName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            Function = new PexObjectFunction(br, _pexFile);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(FunctionName));
            Function?.Write(bw);
        }
    }

    public partial class PexObjectFunction
    {
        IReadOnlyList<IUserFlagGetter> IHasUserFlagsGetter.UserFlags => UserFlags;

        private readonly PexFile _pexFile = null!;
        public PexObjectFunction(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectFunction(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            ReturnTypeName = _pexFile.GetStringFromIndex(br.ReadUInt16());
            DocString = _pexFile.GetStringFromIndex(br.ReadUInt16());
            UserFlags = _pexFile.GetUserFlags(br.ReadUInt32()).ToExtendedList();
            Flags = (FunctionFlags)br.ReadByte();

            var parameters = br.ReadUInt16();
            for (var i = 0; i < parameters; i++)
            {
                var parameter = new PexObjectFunctionVariable(br, _pexFile);
                Parameters.Add(parameter);
            }

            var locals = br.ReadUInt16();
            for (var i = 0; i < locals; i++)
            {
                var local = new PexObjectFunctionVariable(br, _pexFile);
                Locals.Add(local);
            }

            var instructions = br.ReadUInt16();
            for (var i = 0; i < instructions; i++)
            {
                var instruction = new PexObjectFunctionInstruction(br, _pexFile);
                Instructions.Add(instruction);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(ReturnTypeName));
            bw.Write(_pexFile.GetIndexFromString(DocString));
            bw.Write(_pexFile.GetUserFlags(UserFlags));
            bw.Write((byte)Flags);

            bw.Write((ushort)Parameters.Count);
            foreach (var parameter in Parameters)
            {
                parameter.Write(bw);
            }

            bw.Write((ushort)Locals.Count);
            foreach (var local in Locals)
            {
                local.Write(bw);
            }

            bw.Write((ushort)Instructions.Count);
            foreach (var instruction in Instructions)
            {
                instruction.Write(bw);
            }
        }
    }

    public partial class PexObjectFunctionVariable
    {
        private readonly PexFile _pexFile = null!;
        public PexObjectFunctionVariable(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectFunctionVariable(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            Name = _pexFile.GetStringFromIndex(br.ReadUInt16());
            TypeName = _pexFile.GetStringFromIndex(br.ReadUInt16());
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(_pexFile.GetIndexFromString(Name));
            bw.Write(_pexFile.GetIndexFromString(TypeName));
        }
    }

    public partial class PexObjectFunctionInstruction
    {
        private readonly PexFile _pexFile = null!;
        public PexObjectFunctionInstruction(PexFile pexFile) { _pexFile = pexFile; }
        public PexObjectFunctionInstruction(BinaryReader br, PexFile pexFile) : this(pexFile) { Read(br); }

        public void Read(BinaryReader br)
        {
            OpCode = (InstructionOpcode)br.ReadByte();

            var arguments = InstructionOpCodeArguments.GetArguments(OpCode);
            foreach (var current in arguments)
            {
                var argument = new PexObjectVariableData(br, _pexFile);
                Arguments.Add(argument);

                switch (current)
                {
                    case '*' when argument.VariableType != VariableType.Integer || !argument.IntValue.HasValue:
                        throw new InvalidDataException($"Variable-Length Arguments require an Integer Argument! Argument is {argument.VariableType}");
                    case '*':
                        {
                            for (var i = 0; i < argument.IntValue.Value; i++)
                            {
                                var anotherArgument = new PexObjectVariableData(br, _pexFile);
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
                        throw new InvalidDataException($"Argument is unsigned integer but Variable Type is not integer: {argument.VariableType}");
                }
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((byte)OpCode);

            foreach (var argument in Arguments)
            {
                argument.Write(bw);
            }
        }
    }
}
