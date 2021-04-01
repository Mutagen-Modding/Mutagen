using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation.Modules.Aspects
{
    public class NamedAspect : AspectInterfaceDefinition
    {
        public NamedAspect() 
            : base("INamed", ApplicabilityTest)
        {
            Interfaces = (o) =>
            {
                var nameField = o.IterateFields(includeBaseClass: true).FirstOrDefault(x => x.Name == "Name") as Mutagen.Bethesda.Generation.StringType;
                var list = new List<(LoquiInterfaceDefinitionType Type, string Interface)>();
                list.Add((LoquiInterfaceDefinitionType.IGetter, nameof(INamedRequiredGetter)));
                list.Add((LoquiInterfaceDefinitionType.ISetter, nameof(INamedRequired)));
                if (nameField.Nullable)
                {
                    list.Add((LoquiInterfaceDefinitionType.IGetter, nameof(INamedGetter)));
                    list.Add((LoquiInterfaceDefinitionType.ISetter, nameof(INamed)));
                }
                if (nameField.Translated.HasValue)
                {
                    list.Add((LoquiInterfaceDefinitionType.IGetter, nameof(ITranslatedNamedRequiredGetter)));
                    list.Add((LoquiInterfaceDefinitionType.ISetter, nameof(ITranslatedNamedRequired)));
                    if (nameField.Nullable)
                    {
                        list.Add((LoquiInterfaceDefinitionType.IGetter, nameof(ITranslatedNamedGetter)));
                        list.Add((LoquiInterfaceDefinitionType.ISetter, nameof(ITranslatedNamed)));
                    }
                }
                return list;
            };

            FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, Loqui.FileGeneration> Actions)>()
            {
                (LoquiInterfaceType.Direct, "Name", (o, fg) =>
                {
                    var nameField = o.IterateFields(includeBaseClass: true).FirstOrDefault(x => x.Name == "Name") as Mutagen.Bethesda.Generation.StringType;
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
                (LoquiInterfaceType.IGetter, "Name", (o, fg) =>
                {
                    var nameField = o.IterateFields(includeBaseClass: true).FirstOrDefault(x => x.Name == "Name") as Mutagen.Bethesda.Generation.StringType;
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
            IdentifyFields = (o) =>
            {
                return o.IterateFields(includeBaseClass: true)
                .Where(x => x.Name == "Name")
                .OfType<StringType>();
            };
        }

        public static bool ApplicabilityTest(ObjectGeneration o)
        {
            return o.TestTrueForAnyInClassChain(o2 => o2.Fields.FirstOrDefault(x => x.Name == "Name") is Mutagen.Bethesda.Generation.StringType);
        }
    }
}
