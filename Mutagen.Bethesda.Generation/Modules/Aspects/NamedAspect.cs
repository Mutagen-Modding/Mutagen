using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Plugins.Aspects;
using StringType = Mutagen.Bethesda.Generation.Fields.StringType;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class NamedAspect : AspectFieldInterfaceDefinition
{
    public NamedAspect()
        : base(
            "INamed",
            AspectSubInterfaceDefinition.Factory(
                nameof(INamed),
                (_, f) => Test(f, translated: null, nullable: true)),
            AspectSubInterfaceDefinition.Factory(
                nameof(INamedRequired),
                (_, f) => Test(f, translated: null, nullable: null)),
            AspectSubInterfaceDefinition.Factory(
                nameof(ITranslatedNamed),
                (_, f) => Test(f, translated: true, nullable: true)),
            AspectSubInterfaceDefinition.Factory(
                nameof(ITranslatedNamedRequired),
                (_, f) => Test(f, translated: true, nullable: null)))
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

    public static bool Test(Dictionary<string, TypeGeneration> allFields, bool? translated, bool? nullable) => 
        allFields.TryGetValue("Name", out var field) 
        && field is StringType str
        && (nullable == null || str.Nullable == nullable.Value)
        && (translated == null || str.Translated.HasValue == translated.Value);
}