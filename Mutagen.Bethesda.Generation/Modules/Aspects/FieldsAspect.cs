using Loqui.Generation;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class FieldsAspect : AspectFieldInterfaceDefinition
{
    public (string FieldName, string TypeName)[] Fields;

    public FieldsAspect(string interfaceType, params (string FieldName, string TypeName)[] fields)
        : base(interfaceType)
    {
        Fields = fields;
    }

    public override bool Test(ObjectGeneration o, Dictionary<string, TypeGeneration> allFields) =>
        Fields.All((x) => allFields.TryGetValue(x.FieldName, out var field)
                          && field.TypeName(getter: true) == x.TypeName);

    public override IEnumerable<TypeGeneration> IdentifyFields(ObjectGeneration o) =>
        from f in Fields
        join field in o.IterateFields(includeBaseClass: true)
            on f.FieldName equals field.Name
        where field.TypeName(getter: true) != f.TypeName
        select field;
}