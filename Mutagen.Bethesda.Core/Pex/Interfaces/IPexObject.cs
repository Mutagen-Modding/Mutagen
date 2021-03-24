using System.Collections.Generic;
using JetBrains.Annotations;
using Mutagen.Bethesda.Core.Pex.Enums;

namespace Mutagen.Bethesda.Core.Pex.Interfaces
{
    [PublicAPI]
    public interface IPexObject : IBinaryObject, IHasUserFlags
    {
        public string? Name { get; set; }
        public string? ParentClassName { get; set; }
        public string? DocString { get; set; }
        public bool IsConst { get; set; }
        public string? AutoStateName { get; set; }
        public List<IPexObjectStructInfo> StructInfos { get; set; }
        public List<IPexObjectVariable> Variables { get; set; }
        public List<IPexObjectProperty> Properties { get; set; }
        public List<IPexObjectState> States { get; set; }
    }

    [PublicAPI]
    public interface IPexObjectStructInfo : IBinaryObject
    {
        public string? Name { get; set; }
        public List<IPexObjectStructInfoMember> Members { get; set; }
    }

    [PublicAPI]
    public interface IPexObjectStructInfoMember : IBinaryObject, IHasUserFlags
    {
        public string? Name { get; set; }
        public string? TypeName { get; set; }
        public IPexObjectVariableData? Value { get; set; }
        public bool IsConst { get; set; }
        public string? DocString { get; set; }
    }
    
    [PublicAPI]
    public interface IPexObjectVariable : IBinaryObject, IHasUserFlags
    {
        public string? Name { get; set; }
        public string? TypeName { get; set; }
        public IPexObjectVariableData? VariableData { get; set; }
    }
    
    [PublicAPI]
    public interface IPexObjectVariableData : IBinaryObject
    {
        public VariableType VariableType { get; set; }
        
        public string? StringValue { get; set; }
        public int? IntValue { get; set; }
        public float? FloatValue { get; set; }
        public bool? BoolValue { get; set; }
    }

    [PublicAPI]
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

    [PublicAPI]
    public interface IPexObjectState : IBinaryObject
    {
        public string? Name { get; set; }
        public List<IPexObjectNamedFunction> Functions { get; set; }
    }

    [PublicAPI]
    public interface IPexObjectNamedFunction : IBinaryObject
    {
        public string? FunctionName { get; set; }
        public IPexObjectFunction? Function { get; set; }
    }

    [PublicAPI]
    public interface IPexObjectFunction : IBinaryObject, IHasUserFlags
    {
        public string? ReturnTypeName { get; set; }
        public string? DocString { get; set; }
        public FunctionFlags Flags { get; set; }
        
        public List<IPexObjectFunctionVariable> Parameters { get; set; }
        public List<IPexObjectFunctionVariable> Locals { get; set; }
        public List<IPexObjectFunctionInstruction> Instructions { get; set; }
    }

    [PublicAPI]
    public interface IPexObjectFunctionVariable : IBinaryObject
    {
        public string? Name { get; set; }
        public string? TypeName { get; set; }
    }

    [PublicAPI]
    public interface IPexObjectFunctionInstruction : IBinaryObject
    {
        public InstructionOpcode OpCode { get; set; }
        
        public List<IPexObjectVariableData> Arguments { get; set; }
    }
}
