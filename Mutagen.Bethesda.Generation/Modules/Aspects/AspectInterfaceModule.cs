using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Loqui.Generation;
using Noggog;
using Loqui;
using Mutagen.Bethesda.Plugins.Records.Mapping;

namespace Mutagen.Bethesda.Generation.Modules.Aspects;

public class AspectInterfaceModule : GenerationModule
{
    public List<AspectInterfaceDefinition> Definitions = new();
    public static readonly Dictionary<ProtocolKey, Dictionary<AspectInterfaceDefinition, Dictionary<IAspectSubInterfaceDefinition, List<ObjectGeneration>>>> ObjectMappings = new();

    public AspectInterfaceModule()
    {
        Definitions.Add(new KeywordedAspect());
        Definitions.Add(new KeywordAspect());
        Definitions.Add(new NamedAspect());
        Definitions.Add(new ObjectBoundedAspect());
        Definitions.Add(new RefAspect("IScripted", "VirtualMachineAdapter", "VirtualMachineAdapter"));
        Definitions.Add(new RefAspect("IModeled", "Model", "Model"));
        Definitions.Add(new RefAspect("IHasIcons", "Icons", "Icons"));
        Definitions.Add(new FieldsAspect("IWeightValue",
            ("Value", "UInt32"),
            ("Weight", "Single")));
    }

    public override async Task LoadWrapup(ObjectGeneration obj)
    {
        await obj.GetObjectData().WiringComplete.Task;
        var allFields = obj.IterateFields(includeBaseClass: true).ToDictionary(x => x.Name);
        foreach (var def in Definitions)
        {
            if (!def.Test(obj, allFields)) continue;
            foreach (var subDef in def.SubDefinitions)
            {
                if (!subDef.Test(obj, allFields)) continue;
                subDef.Registrations.ForEach(x => obj.Interfaces.Add(x.Setter ? LoquiInterfaceDefinitionType.ISetter : LoquiInterfaceDefinitionType.IGetter, x.Name));
                lock (ObjectMappings)
                {
                    ObjectMappings.GetOrAdd(obj.ProtoGen.Protocol).GetOrAdd(def).GetOrAdd(subDef).Add(obj);
                }
                obj.RequiredNamespaces.Add("Mutagen.Bethesda.Plugins.Aspects");
            }
        }
    }

    public override async Task GenerateInField(ObjectGeneration obj, TypeGeneration tg, FileGeneration fg, LoquiInterfaceType type)
    {
        using (new RegionWrapper(fg, "Aspects")
               {
                   AppendExtraLine = false,
                   SkipIfOnlyOneLine = true,
               })
        {
            var allFields = obj.IterateFields(includeBaseClass: true).ToDictionary(x => x.Name);
            foreach (var def in Definitions.OfType<AspectFieldInterfaceDefinition>())
            {
                if (!def.Test(obj, allFields)) continue;
                def.FieldActions
                    .Where(x => x.Type == type && tg.Name == x.Name)
                    .ForEach(x => x.Actions(obj, tg, fg));
            }
        }
    }

    public override async Task PostLoad(ObjectGeneration obj)
    {
        Dictionary<LoquiInterfaceDefinitionType, HashSet<string>>? aspects = null;
        Dictionary<string, (TypeGeneration type, Dictionary<LoquiInterfaceDefinitionType, HashSet<string>> aspects)>? fieldsToAspects = null;

        var allFields = obj.IterateFields(includeBaseClass: true).ToDictionary(x => x.Name);

        foreach (var def in Definitions)
        {
            if (!def.Test(obj, allFields)) continue;
            foreach (var subDef in def.SubDefinitions)
            {
                if (!subDef.Test(obj, allFields)) continue;
                var interfaces = subDef.Registrations.Select(x => new AspectInterfaceData(x.Setter ? LoquiInterfaceDefinitionType.ISetter : LoquiInterfaceDefinitionType.IGetter, x.Name));
                if (def is AspectFieldInterfaceDefinition fieldDef)
                {
                    foreach (var f in fieldDef.IdentifyFields(obj))
                    {
                        RecordAspects((fieldsToAspects ??= new()).GetOrAdd(f.Name, () => new(f, new())).aspects, interfaces);
                    }
                }
                else
                {
                    RecordAspects(aspects ??= new(), interfaces);
                    RecordAspects(aspects ??= new(), interfaces);
                }
            }
        }

        if (aspects is not null)
        {
            AddAspectComment(aspects, obj.Comments ??= new());
        }

        if (fieldsToAspects is not null)
        {
            foreach (var (type, typeAspects) in fieldsToAspects.Values)
            {
                AddAspectComment(typeAspects, type.Comments ??= new());
            }
        }
    }

    private static void RecordAspects(
        Dictionary<LoquiInterfaceDefinitionType, HashSet<string>> aspects,
        IEnumerable<AspectInterfaceData> interfaces)
    {
        foreach (var (Type, Interface) in interfaces)
        {
            var escapedInterface = @Interface.Replace("<", "&lt;").Replace(">", "&gt;");
            switch (Type)
            {
                case LoquiInterfaceDefinitionType.Direct:
                case LoquiInterfaceDefinitionType.IGetter:
                    aspects.GetOrAdd(Type).Add(escapedInterface);
                    break;
                case LoquiInterfaceDefinitionType.ISetter:
                    aspects.GetOrAdd(Type).Add(escapedInterface);
                    aspects.GetOrAdd(LoquiInterfaceDefinitionType.Direct).Add(escapedInterface);
                    break;
                case LoquiInterfaceDefinitionType.Dual:
                    aspects.GetOrAdd(LoquiInterfaceDefinitionType.Direct).Add(escapedInterface);
                    aspects.GetOrAdd(LoquiInterfaceDefinitionType.ISetter).Add(escapedInterface);
                    aspects.GetOrAdd(LoquiInterfaceDefinitionType.IGetter).Add(escapedInterface);
                    break;
                default:
                    break;
            }
        }
    }

    private static void AddAspectComment(Dictionary<LoquiInterfaceDefinitionType, HashSet<string>> aspects, CommentCollection comments)
    {
        foreach (var item in aspects)
        {
            FileGeneration summary = (item.Key switch
            {
                LoquiInterfaceDefinitionType.IGetter => (comments.GetterInterface ??= new(null!)).Summary,
                LoquiInterfaceDefinitionType.ISetter => (comments.SetterInterface ??= new(null!)).Summary,
                _ => comments.Comments.Summary,
            });
            if (!summary.Empty)
                summary.AppendLine("<br />");
            summary.AppendLine("Aspects: " + string.Join(", ", item.Value.OrderBy(x => x)));
        }
    }

    public override async Task FinalizeGeneration(ProtocolGeneration proto)
    {
        // Generate interface files themselves
        if (!ObjectMappings.TryGetValue(proto.Protocol, out var mappings)) return;
        
        GenerateAspectInterfaceMapping(proto, mappings);
    }

    private static void GenerateAspectInterfaceMapping(ProtocolGeneration proto, Dictionary<AspectInterfaceDefinition, Dictionary<IAspectSubInterfaceDefinition, List<ObjectGeneration>>> mappings)
    {
        // Generate interface to major record mapping registry
        FileGeneration mappingGen = new FileGeneration();
        ObjectGeneration.AddAutogenerationComment(mappingGen);
        mappingGen.AppendLine($"using System;");
        mappingGen.AppendLine($"using System.Collections.Generic;");
        mappingGen.AppendLine($"using Mutagen.Bethesda.Plugins.Records.Mapping;");
        mappingGen.AppendLine($"using Mutagen.Bethesda.Plugins.Aspects;");
        mappingGen.AppendLine($"using Loqui;");
        mappingGen.AppendLine();
        using (new NamespaceWrapper(mappingGen, $"{proto.DefaultNamespace}.Internals", fileScoped: false))
        {
            using (var c = new ClassWrapper(mappingGen, "AspectInterfaceMapping"))
            {
                c.Interfaces.Add(nameof(IAspectInterfaceMapping));
            }
            
            using (new BraceWrapper(mappingGen))
            {
                mappingGen.AppendLine(
                    $"public IReadOnlyDictionary<Type, {nameof(InterfaceMappingResult)}> InterfaceToObjectTypes {{ get; }}");
                mappingGen.AppendLine();
                mappingGen.AppendLine(
                    $"public {nameof(GameCategory)} GameCategory => {nameof(GameCategory)}.{proto.Protocol.Namespace};");
                mappingGen.AppendLine();

                mappingGen.AppendLine("public AspectInterfaceMapping()");
                using (new BraceWrapper(mappingGen))
                {
                    mappingGen.AppendLine($"var dict = new Dictionary<Type, {nameof(InterfaceMappingResult)}>();");
                    foreach (var aspectDef in mappings.OrderBy(x => x.Key.Nickname))
                    {
                        foreach (var subDef in aspectDef.Value.OrderBy(x => x.Key.Name))
                        {
                            (string Name, bool Setter)? first = null;
                            foreach (var reg in subDef.Key.Registrations.OrderBy(x => x.Name))
                            {
                                if (first == null)
                                {
                                    first = reg;
                                    mappingGen.AppendLine($"dict[typeof({first.Value.Name})] = new {nameof(InterfaceMappingResult)}({first.Value.Setter.ToString().ToLower()}, new {nameof(ILoquiRegistration)}[]");
                                    using (new BraceWrapper(mappingGen) { AppendSemicolon = true, AppendParenthesis = true })
                                    {
                                        foreach (var obj in subDef.Value.OrderBy(x => x.Name))
                                        {
                                            mappingGen.AppendLine($"{obj.RegistrationName}.Instance,");
                                        }
                                    }
                                }
                                else
                                {
                                    mappingGen.AppendLine($"dict[typeof({reg.Name})] = dict[typeof({first.Value.Name})] with {{ Setter = {reg.Setter.ToString().ToLower()} }};");
                                }
                            }
                        }

                    }

                    mappingGen.AppendLine($"InterfaceToObjectTypes = dict;");
                }
            }
        }

        mappingGen.AppendLine();
        var mappingPath = Path.Combine(proto.DefFileLocation.FullName,
            $"../Interfaces/Aspect/AspectInterfaceMapping{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        mappingGen.Generate(mappingPath);
        proto.GeneratedFiles.Add(mappingPath, ProjItemType.Compile);
    }
}