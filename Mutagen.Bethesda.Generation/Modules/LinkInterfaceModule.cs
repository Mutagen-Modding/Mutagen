using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Core;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Generation
{
    public class LinkInterfaceModule : GenerationModule
    {
        public static Dictionary<ProtocolKey, Dictionary<string, List<ObjectGeneration>>> ObjectMappings = new Dictionary<ProtocolKey, Dictionary<string, List<ObjectGeneration>>>();

        public override async Task PreLoad(ObjectGeneration obj)
        {
            await base.PreLoad(obj);
            foreach (var item in obj.Node.Elements(XName.Get("LinkInterface", LoquiGenerator.Namespace)))
            {
                obj.Interfaces.Add(LoquiInterfaceDefinitionType.Dual, item.Value);
            }
        }

        public override async Task PrepareGeneration(ProtocolGeneration proto)
        {
            await base.PrepareGeneration(proto);

            // Compile interfaces implementing interfaces mapping data
            var interfaceInheritenceMappings = new Dictionary<string, HashSet<string>>();
            var interfMappingFile = Path.Combine(proto.DefFileLocation.FullName, $"../Interfaces/InterfaceLinkDependencies.xml");
            if (File.Exists(interfMappingFile))
            {
                var root = XElement.Load(interfMappingFile);
                foreach (var node in root.Elements(XName.Get("LinkInterface")))
                {
                    var name = node.GetAttribute("name", throwException: true);
                    foreach (var impl in node.Elements(XName.Get("Implements")))
                    {
                        interfaceInheritenceMappings.GetOrAdd(name).Add(impl.Value);
                    }
                }
            }

            foreach (var obj in proto.ObjectGenerationsByID.Values)
            {
                foreach (var item in obj.Node.Elements(XName.Get("LinkInterface", LoquiGenerator.Namespace)))
                {
                    ObjectMappings.GetOrAdd(proto.Protocol).GetOrAdd(item.Value).Add(obj);
                }
            }

            // Generate interface files themselves
            if (!ObjectMappings.TryGetValue(proto.Protocol, out var mappings)) return;
            foreach (var interf in mappings)
            {
                FileGeneration fg = new FileGeneration();
                ObjectGeneration.AddAutogenerationComment(fg);

                fg.AppendLine("using Mutagen.Bethesda;");
                fg.AppendLine();

                var implementedObjs = new HashSet<ObjectGeneration>();

                void AddObjs(string interfKey)
                {
                    implementedObjs.Add(ObjectMappings[proto.Protocol][interfKey]);
                    if (interfaceInheritenceMappings.TryGetValue(interfKey, out var parents))
                    {
                        foreach (var parent in parents)
                        {
                            AddObjs(parent);
                        }
                    }
                }
                AddObjs(interf.Key);

                using (new NamespaceWrapper(fg, proto.DefaultNamespace))
                {
                    fg.AppendLine("/// <summary>");
                    fg.AppendLine($"/// Implemented by: [{string.Join(", ", implementedObjs.Select(o => o.ObjectName))}]");
                    fg.AppendLine("/// </summary>");
                    using (var c = new ClassWrapper(fg, interf.Key))
                    {
                        c.Type = ClassWrapper.ObjectType.@interface;
                        c.Interfaces.Add($"I{proto.Protocol.Namespace}MajorRecordInternal");
                        c.Interfaces.Add($"{interf.Key}Getter");
                        if (interfaceInheritenceMappings.TryGetValue(interf.Key, out var impls))
                        {
                            c.Interfaces.Add(impls);
                        }
                        c.Partial = true;
                    }
                    using (new BraceWrapper(fg))
                    {
                    }
                    fg.AppendLine();

                    fg.AppendLine("/// <summary>");
                    fg.AppendLine($"/// Implemented by: [{string.Join(", ", implementedObjs.Select(o => o.ObjectName))}]");
                    fg.AppendLine("/// </summary>");
                    using (var c = new ClassWrapper(fg, $"{interf.Key}Getter"))
                    {
                        c.Type = ClassWrapper.ObjectType.@interface;
                        c.Interfaces.Add($"I{proto.Protocol.Namespace}MajorRecordGetter");
                        if (interfaceInheritenceMappings.TryGetValue(interf.Key, out var impls))
                        {
                            c.Interfaces.Add(impls.Select(i => $"{i}Getter"));
                        }
                        c.Partial = true;
                    }
                    using (new BraceWrapper(fg))
                    {
                    }
                }
                
                var path = Path.Combine(proto.DefFileLocation.FullName, $"../Interfaces/{interf.Key}{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
                fg.Generate(path);
                proto.GeneratedFiles.Add(path, ProjItemType.Compile);
            }

            // Generate interface to major record mapping registry
            FileGeneration mappingGen = new FileGeneration();
            ObjectGeneration.AddAutogenerationComment(mappingGen);
            mappingGen.AppendLine($"using System;");
            mappingGen.AppendLine($"using System.Collections.Generic;");
            mappingGen.AppendLine($"using Mutagen.Bethesda.Core;");
            mappingGen.AppendLine();
            using (new NamespaceWrapper(mappingGen, $"{proto.DefaultNamespace}.Internals"))
            {
                using (var c = new ClassWrapper(mappingGen, "LinkInterfaceMapping"))
                {
                    c.Interfaces.Add(nameof(ILinkInterfaceMapping));
                }
                using (new BraceWrapper(mappingGen))
                {
                    mappingGen.AppendLine($"public IReadOnlyDictionary<Type, Type[]> InterfaceToObjectTypes {{ get; }}");
                    mappingGen.AppendLine();
                    mappingGen.AppendLine($"public {nameof(GameCategory)} GameCategory => {nameof(GameCategory)}.{proto.Protocol.Namespace};");
                    mappingGen.AppendLine();

                    mappingGen.AppendLine("public LinkInterfaceMapping()");
                    using (new BraceWrapper(mappingGen))
                    {
                        mappingGen.AppendLine($"var dict = new Dictionary<Type, Type[]>();");
                        foreach (var interf in mappings)
                        {
                            mappingGen.AppendLine($"dict[typeof({interf.Key})] = new Type[]");
                            using (new BraceWrapper(mappingGen) { AppendSemicolon = true })
                            {
                                foreach (var obj in interf.Value)
                                {
                                    mappingGen.AppendLine($"typeof({obj.ObjectName}),");
                                }
                            }
                            mappingGen.AppendLine($"dict[typeof({interf.Key}Getter)] = dict[typeof({interf.Key})];");
                        }

                        mappingGen.AppendLine($"InterfaceToObjectTypes = dict;");
                    }
                }
            }
            mappingGen.AppendLine();
            var mappingPath = Path.Combine(proto.DefFileLocation.FullName, $"../Interfaces/LinkInterfaceMapping{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
            mappingGen.Generate(mappingPath);
            proto.GeneratedFiles.Add(mappingPath, ProjItemType.Compile);
        }
    }
}
