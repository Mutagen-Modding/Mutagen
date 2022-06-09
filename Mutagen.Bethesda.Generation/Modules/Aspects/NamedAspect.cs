using Loqui.Generation;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Plugins.Aspects;
using Noggog.StructuredStrings.CSharp;
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
            new(LoquiInterfaceType.Direct, _ => "Name", (o, tg, sb) =>
            {
                if (tg is not StringType nameField) throw new ArgumentException("Name is not a String", nameof(tg));
                var isTransl = nameField.Translated.HasValue;

                if (isTransl)
                {
                    if (nameField.Nullable)
                    {
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine("string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;");
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine("string? INamedGetter.Name => this.Name?.String;");
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine($"{nameof(ITranslatedStringGetter)}? {nameof(ITranslatedNamedGetter)}.Name => this.Name;");
                    }
                    else
                    {
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine($"{nameof(ITranslatedStringGetter)} {nameof(ITranslatedNamedRequiredGetter)}.Name => this.Name ?? {nameof(TranslatedString)}.Empty;");
                    }
                }
                else if (nameField.Nullable)
                {
                    sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    sb.AppendLine("string INamedRequiredGetter.Name => this.Name ?? string.Empty;");
                }

                if (isTransl)
                {
                    if (nameField.IsNullable)
                    {
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine($"{nameof(ITranslatedStringGetter)} {nameof(ITranslatedNamedRequiredGetter)}.Name => this.Name ?? string.Empty;");
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine($"string? INamed.Name");
                        using (sb.CurlyBrace())
                        {
                            sb.AppendLine("get => this.Name?.String;");
                            sb.AppendLine("set => this.Name = value;");
                        }
                    }
                    else
                    {
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine("string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;");
                    }

                    sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    sb.AppendLine("string INamedRequired.Name");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine("get => this.Name?.String ?? string.Empty;");
                        sb.AppendLine("set => this.Name = value;");
                    }
                    sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    sb.AppendLine($"{nameof(TranslatedString)} {nameof(ITranslatedNamedRequired)}.Name");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine("get => this.Name ?? string.Empty;");
                        sb.AppendLine("set => this.Name = value;");
                    }
                }
                else if (nameField.Nullable)
                {
                    sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    sb.AppendLine("string INamedRequired.Name");
                    using (sb.CurlyBrace())
                    {
                        sb.AppendLine("get => this.Name ?? string.Empty;");
                        sb.AppendLine("set => this.Name = value;");
                    }
                }
            }),
            new(LoquiInterfaceType.IGetter, _ => "Name", (o, tg, sb) =>
            {
                if (tg is not StringType nameField) throw new ArgumentException("Name is not a String", nameof(tg));
                var isTransl = nameField.Translated.HasValue;
                if (isTransl)
                {
                    sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    sb.AppendLine("string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;");
                    if (nameField.Nullable)
                    {
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine("string? INamedGetter.Name => this.Name?.String;");
                        sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                        sb.AppendLine($"{nameof(ITranslatedStringGetter)} {nameof(ITranslatedNamedRequiredGetter)}.Name => this.Name ?? {nameof(TranslatedString)}.Empty;");
                    }
                }
                else if (nameField.Nullable)
                {
                    sb.AppendLine("[DebuggerBrowsable(DebuggerBrowsableState.Never)]");
                    sb.AppendLine("string INamedRequiredGetter.Name => this.Name ?? string.Empty;");
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