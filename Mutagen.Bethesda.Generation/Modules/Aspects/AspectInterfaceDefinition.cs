using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

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

    public virtual IEnumerable<(string Name, bool Setter)> Registrations
    {
        get
        {
            yield return ($"typeof({Name})", true);
            yield return ($"typeof({Name}Getter)", false);
        }
    }

    public abstract bool Test(ObjectGeneration obj, Dictionary<string, TypeGeneration> allFields );

    public virtual List<AspectInterfaceData> Interfaces(ObjectGeneration obj)
    {
        var interfaceData = new List<AspectInterfaceData>();
        AddInterfaces(interfaceData, Name);
        return interfaceData;
    }

    protected static void AddInterfaces(List<AspectInterfaceData> interfaceData, string setterInterfaceName)
    {
        interfaceData.Add(new(LoquiInterfaceDefinitionType.ISetter, setterInterfaceName));
        interfaceData.Add(new(LoquiInterfaceDefinitionType.IGetter, $"{setterInterfaceName}Getter"));
    }

    protected static void AddInterfaces(List<AspectInterfaceData> interfaceData, string setterInterfaceName, string getterInterfaceName)
    {
        interfaceData.Add(new(LoquiInterfaceDefinitionType.ISetter, setterInterfaceName));
        interfaceData.Add(new(LoquiInterfaceDefinitionType.IGetter, getterInterfaceName));
    }
}

public record AspectInterfaceData(LoquiInterfaceDefinitionType Type, string Interface);