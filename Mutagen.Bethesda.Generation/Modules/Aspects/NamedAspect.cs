using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Plugins.Aspects;
using StringType = Mutagen.Bethesda.Generation.Fields.StringType;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class NamedAspect : AspectFieldInterfaceDefinition
{
    public NamedAspect()
        : base("INamed")
    {
        FieldActions = new()
        {
            new(LoquiInterfaceType.Direct, "Name", (o, tg, fg) =>
            {
                if (tg is not StringType nameField) throw new ArgumentException("Name is not a String", nameof(tg));
                var isTransl = nameField.Translated.HasValue;

                if (isTransl)
                {
                    if (nameField.Nullable)
                    {
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine("string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;");
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine("string? INamedGetter.Name => this.Name?.String;");
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine($"{nameof(ITranslatedStringGetter)}? {nameof(ITranslatedNamedGetter)}.Name => this.Name;");
                    }
                    else
                    {
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine($"{nameof(ITranslatedStringGetter)} {nameof(ITranslatedNamedRequiredGetter)}.Name => this.Name ?? {nameof(TranslatedString)}.Empty;");
                    }
                }
                else if (nameField.Nullable)
                {
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine("string INamedRequiredGetter.Name => this.Name ?? string.Empty;");
                }

                if (isTransl)
                {
                    if (nameField.IsNullable)
                    {
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine($"{nameof(ITranslatedStringGetter)} {nameof(ITranslatedNamedRequiredGetter)}.Name => this.Name ?? string.Empty;");
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine($"string? INamed.Name");
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine("get => this.Name?.String;");
                            fg.AppendLine("set => this.Name = value;");
                        }
                    }
                    else
                    {
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine("string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;");
                    }

                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine("string INamedRequired.Name");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("get => this.Name?.String ?? string.Empty;");
                        fg.AppendLine("set => this.Name = value;");
                    }
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine($"{nameof(TranslatedString)} {nameof(ITranslatedNamedRequired)}.Name");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("get => this.Name ?? string.Empty;");
                        fg.AppendLine("set => this.Name = value;");
                    }
                }
                else if (nameField.Nullable)
                {
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine("string INamedRequired.Name");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("get => this.Name ?? string.Empty;");
                        fg.AppendLine("set => this.Name = value;");
                    }
                }
            }),
            new(LoquiInterfaceType.IGetter, "Name", (o, tg, fg) =>
            {
                if (tg is not StringType nameField) throw new ArgumentException("Name is not a String", nameof(tg));
                var isTransl = nameField.Translated.HasValue;
                if (isTransl)
                {
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine("string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;");
                    if (nameField.Nullable)
                    {
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine("string? INamedGetter.Name => this.Name?.String;");
                        fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        fg.AppendLine($"{nameof(ITranslatedStringGetter)} {nameof(ITranslatedNamedRequiredGetter)}.Name => this.Name ?? {nameof(TranslatedString)}.Empty;");
                    }
                }
                else if (nameField.Nullable)
                {
                    fg.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    fg.AppendLine("string INamedRequiredGetter.Name => this.Name ?? string.Empty;");
                }
            })
        };
    }

    public override bool Test(ObjectGeneration o, Dictionary<string, TypeGeneration> allFields) 
        => allFields.TryGetValue("Name", out var field) && field is StringType;

    public override List<AspectInterfaceData> Interfaces(ObjectGeneration obj)
    {
        var nameField = obj.IterateFields(includeBaseClass: true).OfType<StringType>().Single(x => x.Name == "Name");

        var list = new List<AspectInterfaceData>();
        AddInterfaces(list, nameof(INamedRequired), nameof(INamedRequiredGetter));

        if (nameField.Nullable)
        {
            AddInterfaces(list, nameof(INamed), nameof(INamedGetter));
        }

        if (nameField.Translated.HasValue)
        {
            AddInterfaces(list, nameof(ITranslatedNamedRequired), nameof(ITranslatedNamedRequiredGetter));
            if (nameField.Nullable)
            {
                AddInterfaces(list, nameof(ITranslatedNamed), nameof(ITranslatedNamedGetter));
            }
        }
        return list;
    }
}