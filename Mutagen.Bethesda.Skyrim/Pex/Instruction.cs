using Mutagen.Bethesda.Pex;
using Mutagen.Bethesda.Pex.Binary.Translations;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Skyrim.Pex
{
    public partial class Instruction
    {
        public abstract InstructionOpcode Opcode { get; }
        public static Instruction CreateFromBinary(PexReader reader)
        {
            var opCode = (InstructionOpcode)reader.ReadUInt8();
            switch (opCode)
            {
                case InstructionOpcode.NOP:
                    return NothingInstruction.CreateFromBinary(reader);
                case InstructionOpcode.IADD:
                case InstructionOpcode.ISUB:
                case InstructionOpcode.IMUL:
                case InstructionOpcode.IDIV:
                    {
                        var ret = ProcessIntsInstruction.CreateFromBinary(reader);
                        ret.Operation = opCode switch
                        {
                            InstructionOpcode.IADD => NumericOperationType.Add,
                            InstructionOpcode.ISUB => NumericOperationType.Subtract,
                            InstructionOpcode.IMUL => NumericOperationType.Multiply,
                            InstructionOpcode.IDIV => NumericOperationType.Divide,
                            _ => throw new NotImplementedException()
                        };
                        return ret;
                    }
                case InstructionOpcode.FADD:
                case InstructionOpcode.FSUB:
                case InstructionOpcode.FMUL:
                case InstructionOpcode.FDIV:
                    {
                        var ret = ProcessFloatsInstruction.CreateFromBinary(reader);
                        ret.Operation = opCode switch
                        {
                            InstructionOpcode.FADD => NumericOperationType.Add,
                            InstructionOpcode.FSUB => NumericOperationType.Subtract,
                            InstructionOpcode.FMUL => NumericOperationType.Multiply,
                            InstructionOpcode.FDIV => NumericOperationType.Divide,
                            _ => throw new NotImplementedException()
                        };
                        return ret;
                    }
                case InstructionOpcode.IMOD:
                    return IntRemainderInstruction.CreateFromBinary(reader);
                case InstructionOpcode.NOT:
                    return FlipBoolInstruction.CreateFromBinary(reader);
                case InstructionOpcode.INEG:
                    return NegateIntInstruction.CreateFromBinary(reader);
                case InstructionOpcode.FNEG:
                    return NegateFloatInstruction.CreateFromBinary(reader);
                case InstructionOpcode.ASSIGN:
                    return AssignInstruction.CreateFromBinary(reader);
                case InstructionOpcode.CAST:
                    return CastInstruction.CreateFromBinary(reader);
                case InstructionOpcode.CMP_EQ:
                case InstructionOpcode.CMP_LT:
                case InstructionOpcode.CMP_LTE:
                case InstructionOpcode.CMP_GT:
                case InstructionOpcode.CMP_GTE:
                    {
                        var ret = CompareInstruction.CreateFromBinary(reader);
                        ret.Operation = opCode switch
                        {
                            InstructionOpcode.CMP_EQ => CompareOperationType.Equal,
                            InstructionOpcode.CMP_LT => CompareOperationType.LessThan,
                            InstructionOpcode.CMP_LTE => CompareOperationType.LessThanOrEqual,
                            InstructionOpcode.CMP_GT => CompareOperationType.GreaterThan,
                            InstructionOpcode.CMP_GTE => CompareOperationType.GreaterThanOrEqual,
                            _ => throw new NotImplementedException()
                        };
                        return ret;
                    }
                case InstructionOpcode.JMP:
                    return UnconditionalBranchInstruction.CreateFromBinary(reader);
                case InstructionOpcode.JMPT:
                case InstructionOpcode.JMPF:
                    {
                        var ret = ConditionalBranchInstruction.CreateFromBinary(reader);
                        ret.Comparison = opCode switch
                        {
                            InstructionOpcode.JMPT => true,
                            InstructionOpcode.JMPF => false,
                            _ => throw new NotImplementedException()
                        };
                        return ret;
                    }
                case InstructionOpcode.CALLMETHOD:
                    return CallMethodInstruction.CreateFromBinary(reader);
                case InstructionOpcode.CALLPARENT:
                    return CallParentInstruction.CreateFromBinary(reader);
                case InstructionOpcode.CALLSTATIC:
                    return CallStaticInstruction.CreateFromBinary(reader);
                case InstructionOpcode.RETURN:
                    return ReturnInstruction.CreateFromBinary(reader);
                case InstructionOpcode.STRCAT:
                    return StringConcatInstruction.CreateFromBinary(reader);
                case InstructionOpcode.PROPGET:
                    return PropertyGetInstruction.CreateFromBinary(reader);
                case InstructionOpcode.PROPSET:
                    return PropertySetInstruction.CreateFromBinary(reader);
                case InstructionOpcode.ARRAY_CREATE:
                    return CreateArrayInstruction.CreateFromBinary(reader);
                case InstructionOpcode.ARRAY_LENGTH:
                    return GetArrayLengthInstruction.CreateFromBinary(reader);
                case InstructionOpcode.ARRAY_GETELEMENT:
                    return GetArrayElementInstruction.CreateFromBinary(reader);
                case InstructionOpcode.ARRAY_SETELEMENT:
                    return SetArrayElementInstruction.CreateFromBinary(reader);
                case InstructionOpcode.ARRAY_FINDELEMENT:
                case InstructionOpcode.ARRAY_RFINDELEMENT:
                    {
                        var ret = FindArrayElementInstruction.CreateFromBinary(reader);
                        ret.StartFromEnd = opCode switch
                        {
                            InstructionOpcode.ARRAY_FINDELEMENT => false,
                            InstructionOpcode.ARRAY_RFINDELEMENT => true,
                            _ => throw new NotImplementedException()
                        };
                        return ret;
                    }
                default:
                    throw new NotImplementedException();
            }
        }
        public static bool TryCreateFromBinary(PexReader reader, [MaybeNullWhen(false)] out Instruction instruction)
        {
            instruction = CreateFromBinary(reader);
            return true;
        }
    }

    namespace Internals
    {
        public partial class InstructionPexCreateTranslation
        {
            public static partial void FillBinaryOpTranslationCustom(PexReader reader, IInstruction item)
            {
            }
        }

        public partial class InstructionPexWriteTranslation
        {
            public static partial void WriteBinaryOpTranslationCustom(PexWriter writer, IInstructionGetter item)
            {
                writer.Write((byte)item.Opcode);
            }
        }
    }

    public partial interface IInstructionGetter
    {
        InstructionOpcode Opcode { get; }
    }

    public partial class NothingInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.NOP;
    }

    public partial class ProcessIntsInstruction
    {
        public override InstructionOpcode Opcode => Operation switch
        {
            NumericOperationType.Add => InstructionOpcode.IADD,
            NumericOperationType.Subtract => InstructionOpcode.ISUB,
            NumericOperationType.Divide => InstructionOpcode.IDIV,
            NumericOperationType.Multiply => InstructionOpcode.IMUL,
            _ => throw new NotImplementedException(),
        };
    }

    public partial class ProcessFloatsInstruction
    {
        public override InstructionOpcode Opcode => Operation switch
        {
            NumericOperationType.Add => InstructionOpcode.IADD,
            NumericOperationType.Subtract => InstructionOpcode.ISUB,
            NumericOperationType.Divide => InstructionOpcode.IDIV,
            NumericOperationType.Multiply => InstructionOpcode.IMUL,
            _ => throw new NotImplementedException(),
        };
    }

    public partial class IntRemainderInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.IMOD;
    }

    public partial class FlipBoolInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.NOT;
    }

    public partial class NegateIntInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.INEG;
    }

    public partial class NegateFloatInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.FNEG;
    }

    public partial class AssignInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.ASSIGN;
    }

    public partial class CastInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.CAST;
    }

    public partial class CompareInstruction
    {
        public override InstructionOpcode Opcode => Operation switch
        {
            CompareOperationType.Equal => InstructionOpcode.CMP_EQ,
            CompareOperationType.LessThan => InstructionOpcode.CMP_LT,
            CompareOperationType.LessThanOrEqual => InstructionOpcode.CMP_LTE,
            CompareOperationType.GreaterThan => InstructionOpcode.CMP_GT,
            CompareOperationType.GreaterThanOrEqual => InstructionOpcode.CMP_GTE,
            _ => throw new NotImplementedException(),
        };
    }

    public partial class UnconditionalBranchInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.JMP;
    }

    public partial class ConditionalBranchInstruction
    {
        public override InstructionOpcode Opcode => Comparison ? InstructionOpcode.JMPT : InstructionOpcode.JMPF;
    }

    public partial class CallMethodInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.CALLMETHOD;
    }

    public partial class CallParentInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.CALLPARENT;
    }

    public partial class CallStaticInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.CALLSTATIC;
    }

    public partial class ReturnInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.RETURN;
    }

    public partial class StringConcatInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.STRCAT;
    }

    public partial class PropertyGetInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.PROPGET;
    }

    public partial class PropertySetInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.PROPSET;
    }

    public partial class CreateArrayInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.ARRAY_CREATE;
    }

    public partial class GetArrayLengthInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.ARRAY_LENGTH;
    }

    public partial class GetArrayElementInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.ARRAY_GETELEMENT;
    }

    public partial class SetArrayElementInstruction
    {
        public override InstructionOpcode Opcode => InstructionOpcode.ARRAY_SETELEMENT;
    }

    public partial class FindArrayElementInstruction
    {
        public override InstructionOpcode Opcode => StartFromEnd ? InstructionOpcode.ARRAY_FINDELEMENT : InstructionOpcode.ARRAY_RFINDELEMENT;
    }

    namespace Internals
    {
        public partial class CallMethodInstructionPexCreateTranslation
        {
            public static IEnumerable<AVariable> ParseExtraVars(PexReader reader)
            {
                var intVar = IntVariable.CreateFromBinary(reader);
                for (int i = 0; i < intVar.Value; i++)
                {
                    yield return AVariable.CreateFromBinary(reader);
                }
            }

            public static partial void FillBinaryExtraVariablesCustom(PexReader reader, ICallMethodInstruction item)
            {
                item.ExtraVariables.SetTo(ParseExtraVars(reader));
            }
        }

        public partial class CallMethodInstructionPexWriteTranslation
        {
            public static void WriteExtraVars(PexWriter writer, IReadOnlyList<IAVariableGetter> list)
            {
                var intVar = new IntVariable()
                {
                    Value = list.Count
                };
                intVar.WriteToBinary(writer);
                foreach (var item in list)
                {
                    item.WriteToBinary(writer);
                }
            }

            public static partial void WriteBinaryExtraVariablesCustom(PexWriter writer, ICallMethodInstructionGetter item)
            {
                WriteExtraVars(writer, item.ExtraVariables);
            }
        }

        public partial class CallParentInstructionPexCreateTranslation
        {
            public static partial void FillBinaryExtraVariablesCustom(PexReader reader, ICallParentInstruction item)
            {
                item.ExtraVariables.SetTo(CallMethodInstructionPexCreateTranslation.ParseExtraVars(reader));
            }
        }

        public partial class CallParentInstructionPexWriteTranslation
        {
            public static partial void WriteBinaryExtraVariablesCustom(PexWriter writer, ICallParentInstructionGetter item)
            {
                CallMethodInstructionPexWriteTranslation.WriteExtraVars(writer, item.ExtraVariables);
            }
        }

        public partial class CallStaticInstructionPexCreateTranslation
        {
            public static partial void FillBinaryExtraVariablesCustom(PexReader reader, ICallStaticInstruction item)
            {
                item.ExtraVariables.SetTo(CallMethodInstructionPexCreateTranslation.ParseExtraVars(reader));
            }
        }

        public partial class CallStaticInstructionPexWriteTranslation
        {
            public static partial void WriteBinaryExtraVariablesCustom(PexWriter writer, ICallStaticInstructionGetter item)
            {
                CallMethodInstructionPexWriteTranslation.WriteExtraVars(writer, item.ExtraVariables);
            }
        }
    }
}
