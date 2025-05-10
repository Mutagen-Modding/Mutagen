using System.Xml.Linq;
using Loqui.Generation;
using Noggog;
using Loqui;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog.IO;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

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
        Definitions.Add(new VirtualMachineAdapterAspect());
        Definitions.Add(new RefAspect("IScripted", "VirtualMachineAdapter", "VirtualMachineAdapter"));
        Definitions.Add(new RefAspect("IModeled", "Model", "Model"));
        Definitions.Add(new RefAspect("IHasIcons", "Icons", "Icons"));
        Definitions.Add(new RefAspect("IHasDestructible", "Destructible", "Destructible"));
        Definitions.Add(new FieldsAspect("IHarvestable",
            ("Ingredient", "IFormLinkNullableGetter<IHarvestTargetGetter>"),
            ("HarvestSound", "IFormLinkNullableGetter<ISoundDescriptorGetter>")));
        Definitions.Add(new FieldsAspect("IEnchantable",
            ("ObjectEffect", "IFormLinkNullableGetter<IObjectEffectGetter>"),
            ("EnchantmentAmount", "UInt16")));
        Definitions.Add(new FieldsAspect("IWeightValue",
            ("Value", "UInt32"),
            ("Weight", "Single")));
        Definitions.Add(new FieldsAspect("IPositionRotation",
            ("Position", "P3Float"),
            ("Rotation", "P3Float")));
    }

    public override async Task GenerateInField(ObjectGeneration obj, TypeGeneration tg, StructuredStringBuilder sb, LoquiInterfaceType type)
    {
        using (sb.Region("Aspects", appendExtraLine: false, skipIfOnlyOneLine: true))
        {
            var allFields = obj.IterateFields(includeBaseClass: true).ToDictionary(x => x.Name);
            foreach (var def in Definitions.OfType<AspectFieldInterfaceDefinition>())
            {
                if (!def.Test(obj, allFields)) continue;
                def.FieldActions
                    .Where(x => x.Type == type && tg.Name == x.Name(obj))
                    .ForEach(x => x.Actions(obj, tg, sb));
            }
        }
    }

    public override async Task PostLoad(ObjectGeneration obj)
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

        Dictionary<LoquiInterfaceDefinitionType, HashSet<string>>? aspects = null;
        Dictionary<string, (TypeGeneration type, Dictionary<LoquiInterfaceDefinitionType, HashSet<string>> aspects)>? fieldsToAspects = null;

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
            StructuredStringBuilder summary = (item.Key switch
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
        StructuredStringBuilder mappingGen = new StructuredStringBuilder();
        ObjectGeneration.AddAutogenerationComment(mappingGen);
        mappingGen.AppendLine($"using System;");
        mappingGen.AppendLine($"using System.Collections.Generic;");
        mappingGen.AppendLine($"using Mutagen.Bethesda.Plugins.Records.Mapping;");
        mappingGen.AppendLine($"using Mutagen.Bethesda.Plugins.Aspects;");
        mappingGen.AppendLine($"using Loqui;");
        mappingGen.AppendLine();
        using (mappingGen.Namespace(proto.DefaultNamespace, fileScoped: false))
        {
            using (var c = mappingGen.Class($"{proto.Protocol.Namespace}AspectInterfaceMapping"))
            {
                c.AccessModifier = AccessModifier.Internal;
                c.Interfaces.Add(nameof(IInterfaceMapping));
            }
            
            using (mappingGen.CurlyBrace())
            {
                mappingGen.AppendLine(
                    $"public IReadOnlyDictionary<Type, {nameof(InterfaceMappingResult)}> InterfaceToObjectTypes {{ get; }}");
                mappingGen.AppendLine();
                mappingGen.AppendLine(
                    $"public {nameof(GameCategory)} GameCategory => {nameof(GameCategory)}.{proto.Protocol.Namespace};");
                mappingGen.AppendLine();

                mappingGen.AppendLine($"public {proto.Protocol.Namespace}AspectInterfaceMapping()");
                using (mappingGen.CurlyBrace())
                {
                    HashSet<string> addedKeys = new();
                    
                    mappingGen.AppendLine($"var dict = new Dictionary<Type, {nameof(InterfaceMappingResult)}>();");
                    List<(string Name, Action ToDo)> toDo = new();
                    foreach (var aspectDef in mappings)
                    {
                        toDo.Add((aspectDef.Key.Nickname, () =>
                        {
                            foreach (var subDef in aspectDef.Value.OrderBy(x => x.Key.Name))
                            {
                                (string Name, bool Setter)? first = null;
                                var subDefRegistrations = subDef.Key.Registrations.OrderBy(x => x.Name).ToArray();
                                foreach (var reg in subDefRegistrations)
                                {
                                    if (first == null)
                                    {
                                        first = reg;
                                        if (!addedKeys.Add(first.Value.Name))
                                        {
                                            throw new ArgumentException(
                                                $"Added two keys for aspect interface definition: {first.Value.Name}");
                                        }

                                        using (var args = mappingGen.Call(
                                                   $"dict[typeof({first.Value.Name})] = new {nameof(InterfaceMappingResult)}"))
                                        {
                                            args.Add(first.Value.Setter.ToString().ToLower());
                                            args.Add(regisSb =>
                                            {
                                                regisSb.AppendLine($"new {nameof(ILoquiRegistration)}[]");
                                                using (regisSb.CurlyBrace())
                                                {
                                                    foreach (var obj in subDef.Value.OrderBy(x => x.Name))
                                                    {
                                                        regisSb.AppendLine($"{obj.RegistrationName}.Instance,");
                                                    }
                                                }
                                            });
                                            args.Add(regisSb =>
                                            {
                                                string? setter = null;
                                                string? getter = null;

                                                void Set(string name)
                                                {
                                                    if (name.Contains("Getter"))
                                                    {
                                                        getter = name;
                                                    }
                                                    else
                                                    {
                                                        setter = name;
                                                    }
                                                }

                                                for (int i = 0; i < subDefRegistrations.Length && i < 2; i++)
                                                {
                                                    Set(subDefRegistrations[i].Name);
                                                }
                                                using (var c = regisSb.Call("new InterfaceMappingTypes"))
                                                {
                                                    c.Add($"Setter: {(setter == null ? "null" : $"typeof({setter})")}");
                                                    c.Add($"Getter: {(getter == null ? "null" : $"typeof({getter})")}");
                                                }
                                            });
                                        }
                                    }
                                    else
                                    {
                                        if (!addedKeys.Add(reg.Name))
                                        {
                                            throw new ArgumentException(
                                                $"Added two keys for aspect interface definition: {reg.Name}");
                                        }
                                        mappingGen.AppendLine($"dict[typeof({reg.Name})] = dict[typeof({first.Value.Name})] with {{ Setter = {reg.Setter.ToString().ToLower()} }};");
                                    }
                                }
                            }
                        }));
                    }

                    Dictionary<string, List<ObjectGeneration>> looseInterfaces = new();
                    foreach (var obj in proto.ObjectGenerationsByName.Values)
                    {
                        foreach (var interf in obj.Node.Elements(XName.Get("Interface", LoquiGenerator.Namespace)))
                        {
                            looseInterfaces.GetOrAdd(interf.Value).Add(obj);
                        }
                    }

                    foreach (var loose in looseInterfaces)
                    {
                        toDo.Add((loose.Key, () =>
                        {
                            if (!addedKeys.Add(loose.Key))
                            {
                                throw new ArgumentException(
                                    $"Added two keys for aspect interface definition: {loose.Key}");
                            }

                            using (var args = mappingGen.Call(
                                       $"dict[typeof({loose.Key})] = new {nameof(InterfaceMappingResult)}"))
                            {
                                args.Add("true");
                                args.Add(regisSb =>
                                {
                                    regisSb.AppendLine($"new {nameof(ILoquiRegistration)}[]");
                                    using (regisSb.CurlyBrace())
                                    {
                                        foreach (var obj in loose.Value)
                                        {
                                            regisSb.AppendLine($"{obj.RegistrationName}.Instance,");
                                        }
                                    }
                                });
                                args.Add(regisSb =>
                                {
                                    using (var c = regisSb.Call("new InterfaceMappingTypes"))
                                    {
                                        c.Add($"Setter: typeof({loose.Key})");
                                        c.Add($"Getter: typeof({loose.Key}Getter)");
                                    }
                                });
                            }
                            
                            if (!addedKeys.Add($"{loose.Key}Getter"))
                            {
                                throw new ArgumentException(
                                    $"Added two keys for aspect interface definition: {loose.Key}Getter");
                            }
                            mappingGen.AppendLine($"dict[typeof({loose.Key}Getter)] = dict[typeof({loose.Key})] with {{ Setter = false }};");
                        }));
                    }

                    foreach (var item in toDo.OrderBy(x => x.Name))
                    {
                        item.ToDo();
                    }

                    mappingGen.AppendLine($"InterfaceToObjectTypes = dict;");
                }
            }
        }

        mappingGen.AppendLine();
        var mappingPath = Path.Combine(proto.DefFileLocation.FullName,
            $"../Interfaces/Aspect/AspectInterfaceMapping{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        ExportStringToFile exportStringToFile = new(IFileSystemExt.DefaultFilesystem);
        exportStringToFile.ExportToFile(mappingPath, mappingGen.GetString());
        proto.GeneratedFiles.Add(mappingPath, ProjItemType.Compile);
    }
}