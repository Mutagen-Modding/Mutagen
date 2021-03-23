using System.Collections.Generic;
using JetBrains.Annotations;
using Mutagen.Bethesda.Pex.Enums;

namespace Mutagen.Bethesda.Pex.Interfaces
{
    [PublicAPI]
    public interface IPexObject : IBinaryObject, IHasUserFlags
    {
        public ushort NameIndex { get; set; }
        
        public ushort ParentClassNameIndex { get; set; }
        
        public ushort DocStringIndex { get; set; }
        
        public bool IsConst { get; set; }

        public ushort AutoStateNameIndex { get; set; }
        
        public List<IPexObjectStructInfo> StructInfos { get; set; }
        
        public List<IPexObjectVariable> Variables { get; set; }
        
        public List<IPexObjectProperty> Properties { get; set; }
        
        public List<IPexObjectState> States { get; set; }

        public string GetParentClassName(IStringTable stringTable);
        public string GetDocString(IStringTable stringTable);
        public string GetAutoStateName(IStringTable stringTable);
        public string GetName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectStructInfo : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public List<IPexObjectStructInfoMember> Members { get; set; }
        
        public string GetName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectStructInfoMember : IBinaryObject, IHasUserFlags
    {
        public ushort NameIndex { get; set; }
        
        public ushort TypeNameIndex { get; set; }
        
        public IPexObjectVariableData? Value { get; set; }
        
        public bool IsConst { get; set; }
        
        public ushort DocStringIndex { get; set; }
        
        public string GetName(IStringTable stringTable);
        public string GetTypeName(IStringTable stringTable);
        public string GetDocString(IStringTable stringTable);
    }
    
    [PublicAPI]
    public interface IPexObjectVariable : IBinaryObject, IHasUserFlags
    {
        public ushort NameIndex { get; set; }
        
        public ushort TypeNameIndex { get; set; }

        public IPexObjectVariableData? VariableData { get; set; }

        public string GetName(IStringTable stringTable);
        public string GetTypeName(IStringTable stringTable);
    }
    
    [PublicAPI]
    public interface IPexObjectVariableData : IBinaryObject
    {
        public VariableType VariableType { get; set; }
        
        public ushort? StringValueIndex { get; set; }
        
        public int? IntValue { get; set; }
        
        public float? FloatValue { get; set; }
        
        public bool? BoolValue { get; set; }

        public string? GetStringValue(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectProperty : IBinaryObject, IHasUserFlags
    {
        public ushort NameIndex { get; set; }
        
        public ushort TypeNameIndex { get; set; }
        
        public ushort DocStringIndex { get; set; }

        public PropertyFlags Flags { get; set; }
        
        public ushort? AutoVarNameIndex { get; set; }
        
        public IPexObjectFunction? ReadHandler { get; set; }
        
        public IPexObjectFunction? WriteHandler { get; set; }

        public string GetName(IStringTable stringTable);
        public string GetTypeName(IStringTable stringTable);
        public string? GetAutoVarName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectState : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public List<IPexObjectNamedFunction> Functions { get; set; }

        public string GetName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectNamedFunction : IBinaryObject
    {
        public ushort FunctionNameIndex { get; set; }

        public IPexObjectFunction? Function { get; set; }
        
        public string GetFunctionName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectFunction : IBinaryObject, IHasUserFlags
    {
        public ushort ReturnTypeNameIndex { get; set; }
        
        public ushort DocStringIndex { get; set; }

        public FunctionFlags Flags { get; set; }
        
        public List<IPexObjectFunctionVariable> Parameters { get; set; }
        
        public List<IPexObjectFunctionVariable> Locals { get; set; }
        
        public List<IPexObjectFunctionInstruction> Instructions { get; set; }

        public string GetReturnTypeName(IStringTable stringTable);
        public string GetDocString(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectFunctionVariable : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public ushort TypeNameIndex { get; set; }

        public string GetName(IStringTable stringTable);
        public string GetTypeName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectFunctionInstruction : IBinaryObject
    {
        public InstructionOpcode OpCode { get; set; }
        
        public List<IPexObjectVariableData> Arguments { get; set; }
    }
}
