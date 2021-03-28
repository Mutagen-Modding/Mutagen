using System.Collections.Generic;
using Noggog;

namespace Mutagen.Bethesda.Pex
{
    public interface IPexObject : IBinaryObject, IHasUserFlags
    {
        public string? Name { get; set; }
        public string? ParentClassName { get; set; }
        public string? DocString { get; set; }
        public bool IsConst { get; set; }
        public string? AutoStateName { get; set; }
        public ExtendedList<IPexObjectStructInfo> StructInfos { get; set; }
        public ExtendedList<IPexObjectVariable> Variables { get; set; }
        public ExtendedList<IPexObjectProperty> Properties { get; set; }
        public ExtendedList<IPexObjectState> States { get; set; }
    }

    public interface IPexObjectStructInfo : IBinaryObject
    {
        public string? Name { get; set; }
        public ExtendedList<IPexObjectStructInfoMember> Members { get; set; }
    }

    public interface IPexObjectStructInfoMember : IBinaryObject, IHasUserFlags
    {
        public string? Name { get; set; }
        public string? TypeName { get; set; }
        public IPexObjectVariableData? Value { get; set; }
        public bool IsConst { get; set; }
        public string? DocString { get; set; }
    }
    
    public interface IPexObjectVariable : IBinaryObject, IHasUserFlags
    {
        public string? Name { get; set; }
        public string? TypeName { get; set; }
        public IPexObjectVariableData? VariableData { get; set; }
    }
    
    public interface IPexObjectVariableData : IBinaryObject
    {
        public VariableType VariableType { get; set; }
        
        public string? StringValue { get; set; }
        public int? IntValue { get; set; }
        public float? FloatValue { get; set; }
        public bool? BoolValue { get; set; }
    }

    public interface IPexObjectProperty : IBinaryObject, IHasUserFlags
    {
        public string? Name { get; set; }
        public string? TypeName { get; set; }
        public string? DocString { get; set; }
        public PropertyFlags Flags { get; set; }
        public string? AutoVarName { get; set; }
        public IPexObjectFunction? ReadHandler { get; set; }
        public IPexObjectFunction? WriteHandler { get; set; }
    }

    public interface IPexObjectState : IBinaryObject
    {
        public string? Name { get; set; }
        public ExtendedList<IPexObjectNamedFunction> Functions { get; set; }
    }

    public interface IPexObjectNamedFunction : IBinaryObject
    {
        public string? FunctionName { get; set; }
        public IPexObjectFunction? Function { get; set; }
    }

    public interface IPexObjectFunction : IBinaryObject, IHasUserFlags
    {
        public string? ReturnTypeName { get; set; }
        public string? DocString { get; set; }
        public FunctionFlags Flags { get; set; }
        
        public ExtendedList<IPexObjectFunctionVariable> Parameters { get; set; }
        public ExtendedList<IPexObjectFunctionVariable> Locals { get; set; }
        public ExtendedList<IPexObjectFunctionInstruction> Instructions { get; set; }
    }

    public interface IPexObjectFunctionVariable : IBinaryObject
    {
        public string? Name { get; set; }
        public string? TypeName { get; set; }
    }

    public interface IPexObjectFunctionInstruction : IBinaryObject
    {
        public InstructionOpcode OpCode { get; set; }
        
        public ExtendedList<IPexObjectVariableData> Arguments { get; set; }
    }
}
