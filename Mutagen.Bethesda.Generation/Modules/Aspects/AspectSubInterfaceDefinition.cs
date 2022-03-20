using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public interface IAspectSubInterfaceDefinition
{
    string Name { get; }
    IEnumerable<(string Name, bool Setter)> Registrations { get; }
    bool Test(ObjectGeneration obj, Dictionary<string, TypeGeneration> allFields);
}

public abstract class AAspectSubInterfaceDefinition : IAspectSubInterfaceDefinition
{
    public string Name { get; }
    public abstract IEnumerable<(string Name, bool Setter)> Registrations { get; }
    
    protected AAspectSubInterfaceDefinition(string name)
    {
        Name = name;
    }

    public virtual bool Test(ObjectGeneration obj, Dictionary<string, TypeGeneration> allFields)
    {
        return true;
    }

    public static IEnumerable<(string Name, bool Setter)> ConstructTypicalRegistrations(string name)
    {
        yield return (name, true);
        yield return ($"{name}Getter", false);
    }
}

public class AspectSubInterfaceDefinition : IAspectSubInterfaceDefinition
{
    private readonly Func<ObjectGeneration, Dictionary<string, TypeGeneration>, bool> _test;
    public string Name { get; }
    public IEnumerable<(string Name, bool Setter)> Registrations { get; }
    
    public AspectSubInterfaceDefinition(
        string name,
        IEnumerable<(string Name, bool Setter)> registrations,
        Func<ObjectGeneration, Dictionary<string, TypeGeneration>, bool>? test)
    {
        Name = name;
        _test = test ?? new Func<ObjectGeneration, Dictionary<string, TypeGeneration>, bool>((_, _) => true);
        Registrations = registrations;
    }

    public bool Test(ObjectGeneration obj, Dictionary<string, TypeGeneration> allFields)
    {
        return _test(obj, allFields);
    }

    public static AspectSubInterfaceDefinition Factory(
        string name,
        IEnumerable<(string Name, bool Setter)> registrations,
        Func<ObjectGeneration, Dictionary<string, TypeGeneration>, bool>? test = null)
    {
        return new AspectSubInterfaceDefinition(name, registrations, test);
    }

    public static AspectSubInterfaceDefinition Factory(
        string name,
        Func<ObjectGeneration, Dictionary<string, TypeGeneration>, bool>? test = null,
        IEnumerable<(string Name, bool Setter)>? registrations = null)
    {
        return new AspectSubInterfaceDefinition(name, registrations ?? AAspectSubInterfaceDefinition.ConstructTypicalRegistrations(name), test);
    }
}