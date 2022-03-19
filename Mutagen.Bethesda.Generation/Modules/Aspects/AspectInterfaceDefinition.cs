using Loqui.Generation;
using Noggog;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public abstract class AspectFieldInterfaceDefinition : AspectInterfaceDefinition
{
    public AspectFieldInterfaceDefinition(
        string nickName,
        IAspectSubInterfaceDefinition subInterfaceDefinition,
        params IAspectSubInterfaceDefinition[] additionalSubInterfaceDefinitions)
        : base(nickName, subInterfaceDefinition, additionalSubInterfaceDefinitions)
    {
        
    }

    public List<FieldAction> FieldActions = new();

    public virtual IEnumerable<TypeGeneration> IdentifyFields(ObjectGeneration o) =>
        from field in o.Fields
        join f in FieldActions.Select(x => x.Name).Distinct()
            on field.Name equals f
        select field;
}

public abstract class AspectInterfaceDefinition
{
    public readonly IAspectSubInterfaceDefinition[] SubDefinitions;
    public string Nickname { get; }

    public AspectInterfaceDefinition(
        string nickname, 
        IAspectSubInterfaceDefinition subInterfaceDefinition,
        params IAspectSubInterfaceDefinition[] additionalSubInterfaceDefinitions)
    {
        SubDefinitions = subInterfaceDefinition.AsEnumerable().And(additionalSubInterfaceDefinitions).ToArray();
        Nickname = nickname;
    }
    
    public AspectInterfaceDefinition(string nickName)
        : this(nickName, AspectSubInterfaceDefinition.Factory(nickName))
    {
    }

    public virtual bool Test(ObjectGeneration obj, Dictionary<string, TypeGeneration> allFields)
    {
        return SubDefinitions.Any(def => def.Test(obj, allFields));
    }
}

public record AspectInterfaceData(LoquiInterfaceDefinitionType Type, string Interface);