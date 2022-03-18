using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class KeywordedAspect : AspectFieldInterfaceDefinition
{
    public KeywordedAspect()
        : base(
            "IKeyworded",
            AspectSubInterfaceDefinition.Factory(
                Registrations,
                (_, f) => Test(f)))
    {
        FieldActions = new()
        {
            new(LoquiInterfaceType.Direct, "Keywords", (o, tg, fg) =>
            {
                fg.AppendLine("IReadOnlyList<IFormLinkGetter<IKeywordGetter>>? IKeywordedGetter<IKeywordGetter>.Keywords => this.Keywords;");
                fg.AppendLine("IReadOnlyList<IFormLinkGetter<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
            }),
            new(LoquiInterfaceType.IGetter, "Keywords", (o, tg, fg) =>
            {
                fg.AppendLine("IReadOnlyList<IFormLinkGetter<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
            })
        };
    }

    public static IEnumerable<(string Name, bool Setter)> Registrations
    {
        get
        {
            yield return ($"IKeyworded<IKeywordGetter>", true);
            yield return ($"IKeywordedGetter<IKeywordGetter>", false);
        }
    }

    public static bool Test(Dictionary<string, TypeGeneration> allFields)
        => allFields.TryGetValue("Keywords", out var field)
           && field is ContainerType cont
           && typeof(FormLinkType).Equals(cont.SubTypeGeneration.GetType());
}