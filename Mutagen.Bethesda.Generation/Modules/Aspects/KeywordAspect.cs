using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class KeywordAspect : AspectInterfaceDefinition
{
    public KeywordAspect() : base("IKeywordCommon") { }

    public override bool Test(ObjectGeneration o, Dictionary<string, TypeGeneration> allFields) => o.Name == "Keyword";
}