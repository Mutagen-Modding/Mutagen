using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui;

namespace Mutagen.Bethesda.Generation
{
    public class AspectInterfaceModule : GenerationModule
    {
        public List<AspectInterfaceDefinition> Definitions = new List<AspectInterfaceDefinition>();
        public static Dictionary<ProtocolKey, Dictionary<AspectInterfaceDefinition, List<ObjectGeneration>>> ObjectMappings = new Dictionary<ProtocolKey, Dictionary<AspectInterfaceDefinition, List<ObjectGeneration>>>();

        public AspectInterfaceModule()
        {
            // Keyworded
            Definitions.AddReturn(new AspectInterfaceDefinition(
                "IKeyworded",
                o =>
                {
                    if (!(o.Fields.FirstOrDefault(x => x.Name == "Keywords") is ContainerType cont)) return false;
                    return typeof(FormLinkType).Equals(cont.SubTypeGeneration.GetType());
                })
            {
                Interfaces = (o) => new List<(LoquiInterfaceDefinitionType Type, string Interface)>()
                {
                    (LoquiInterfaceDefinitionType.IGetter, $"IKeywordedGetter<IKeywordGetter>"),
                    (LoquiInterfaceDefinitionType.ISetter, $"IKeyworded<IKeywordGetter>"),
                },
                FieldActions = new List<(LoquiInterfaceType Type, string Name, Action<ObjectGeneration, Loqui.FileGeneration> Actions)>()
                {
                    (LoquiInterfaceType.Direct, "Keywords", (o, fg) =>
                    {
                        fg.AppendLine("IReadOnlyList<IFormLink<IKeywordGetter>>? IKeywordedGetter<IKeywordGetter>.Keywords => this.Keywords;");
                        fg.AppendLine("IReadOnlyList<IFormLink<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                    }),
                    (LoquiInterfaceType.IGetter, "Keywords", (o, fg) =>
                    {
                        fg.AppendLine("IReadOnlyList<IFormLink<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => this.Keywords;");
                    })
                }
            });

            // INamed
            Definitions.AddReturn(new AspectInterfaceDefinition(
                "INamed",
                o => o.TestTrueForAnyInClassChain(o2 => o2.Fields.FirstOrDefault(x => x.Name == "Name") is Mutagen.Bethesda.Generation.StringType str))
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
                },
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
                }
            });
        }

        public override async Task LoadWrapup(ObjectGeneration obj)
        {
            await obj.GetObjectData().WiringComplete.Task;
            foreach (var def in Definitions)
            {
                if (!def.Test(obj)) continue;
                if (def.Interfaces != null)
                {
                    def.Interfaces(obj).ForEach(x => obj.Interfaces.Add(x.Type, x.Interface));
                }
                lock (ObjectMappings)
                {
                    ObjectMappings.GetOrAdd(obj.ProtoGen.Protocol).GetOrAdd(def).Add(obj);
                }
            }
        }

        public override async Task GenerateInField(ObjectGeneration obj, TypeGeneration typeGeneration, FileGeneration fg, LoquiInterfaceType type)
        {
            using (new RegionWrapper(fg, "Aspects")
            {
                AppendExtraLine = false,
                SkipIfOnlyOneLine = true,
            })
            {
                foreach (var def in Definitions)
                {
                    if (!def.Test(obj)) continue;
                    def.FieldActions
                        .Where(x => x.Type == type && typeGeneration.Name == x.Name)
                        .ForEach(x => x.Actions(obj, fg));
                }
            }
        }
    }
}
