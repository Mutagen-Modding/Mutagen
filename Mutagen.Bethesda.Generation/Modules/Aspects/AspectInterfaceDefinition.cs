using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public abstract class AspectFieldInterfaceDefinition : AspectInterfaceDefinition
    {
        public AspectFieldInterfaceDefinition(string name) : base(name) { }

        public List<FieldAction> FieldActions = new();

        public virtual IEnumerable<TypeGeneration> IdentifyFields(ObjectGeneration o) =>
            from field in o.Fields
            join f in FieldActions.Select(x => x.Name).Distinct()
              on field.Name equals f
            select field;
    }

    public abstract class AspectInterfaceDefinition
    {
        public string Name { get; }

        public AspectInterfaceDefinition(string name)
        {
            Name = name;
        }

        public abstract bool Test(ObjectGeneration obj, Dictionary<string, TypeGeneration> allFields );

        public virtual List<AspectInterfaceData> Interfaces(ObjectGeneration obj) {
            var interfaceData = new List<AspectInterfaceData>();
            AddInterfaces(interfaceData, Name);
            return interfaceData;
        }

        protected static void AddInterfaces(List<AspectInterfaceData> interfaceData, string setterInterfaceName)
        {
            interfaceData.Add((LoquiInterfaceDefinitionType.ISetter, setterInterfaceName));
            interfaceData.Add((LoquiInterfaceDefinitionType.IGetter, $"{setterInterfaceName}Getter"));
        }

        protected static void AddInterfaces(List<AspectInterfaceData> interfaceData, string setterInterfaceName, string getterInterfaceName)
        {
            interfaceData.Add((LoquiInterfaceDefinitionType.ISetter, setterInterfaceName));
            interfaceData.Add((LoquiInterfaceDefinitionType.IGetter, getterInterfaceName));
        }
    }

    public struct AspectInterfaceData
    {
        public LoquiInterfaceDefinitionType Type;
        public string Interface;
        public string EscapedInterface;

        public AspectInterfaceData(LoquiInterfaceDefinitionType type, string @interface)
        {
            Type = type;
            Interface = @interface;
            EscapedInterface = @interface.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public override bool Equals(object? obj)
        {
            return obj is AspectInterfaceData other &&
                   Type == other.Type &&
                   Interface == other.Interface;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Interface);
        }

        public void Deconstruct(out LoquiInterfaceDefinitionType type, out string @interface)
        {
            type = Type;
            @interface = Interface;
        }

        public void Deconstruct(out LoquiInterfaceDefinitionType type, out string @interface, out string escapedInterface)
        {
            type = Type;
            @interface = Interface;
            escapedInterface = EscapedInterface;
        }

        public static implicit operator (LoquiInterfaceDefinitionType Type, string Interface)(AspectInterfaceData value)
        {
            return (value.Type, value.Interface);
        }

        public static implicit operator AspectInterfaceData((LoquiInterfaceDefinitionType Type, string Interface) value)
        {
            return new AspectInterfaceData(value.Type, value.Interface);
        }

        public static bool operator ==(AspectInterfaceData left, AspectInterfaceData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AspectInterfaceData left, AspectInterfaceData right)
        {
            return !(left == right);
        }
    }
}
