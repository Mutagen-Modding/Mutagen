using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Generation.Fields;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog.IO;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Plugin;

public class AbstractInterfaceModule : GenerationModule
{
    
    public override async Task FinalizeGeneration(ProtocolGeneration proto)
    {
        await base.PrepareGeneration(proto);
        if (proto.Protocol.Namespace == "Bethesda") return;

        HashSet<ObjectGeneration> grupTypes = new();
        foreach (var obj in proto.ObjectGenerationsByName.Values)
        {
            foreach (var field in obj.Fields)
            {
                if (field is GroupType grup)
                {
                    var grupTarget = grup.GetGroupTarget();
                    if (await grupTarget.IsMajorRecord())
                    {
                        grupTypes.Add(grup.GetGroupTarget());
                    }
                }
                else if (field is ContainerType cont)
                {
                    if (cont.SubTypeGeneration is LoquiType loqui)
                    {
                        if (loqui.TargetObjectGeneration != null 
                            && await loqui.TargetObjectGeneration.IsMajorRecord())
                        {
                            grupTypes.Add(loqui.TargetObjectGeneration);
                        }
                    }
                }
            }
        }

        await GenerateAbstractBaseMapping(proto, grupTypes);
        await GenerateInheritingMapping(proto, grupTypes);
    }

    private static async Task GenerateAbstractBaseMapping(ProtocolGeneration proto, HashSet<ObjectGeneration> grupTypes)
    {
        HashSet<ObjectGeneration> baseClasses = new();
        
        foreach (var obj in grupTypes)
        {
            if (obj.BaseClass != null
                && !obj.BaseClass.Name.EndsWith("MajorRecord"))
            {
                if (grupTypes.Contains(obj.BaseClass)) continue;
                baseClasses.Add(obj.BaseClass);
            }
        }

        StructuredStringBuilder mappingGen = new StructuredStringBuilder();
        ObjectGeneration.AddAutogenerationComment(mappingGen);
        mappingGen.AppendLine($"using System;");
        mappingGen.AppendLine($"using System.Collections.Generic;");
        mappingGen.AppendLine($"using Mutagen.Bethesda.Plugins.Records.Mapping;");
        mappingGen.AppendLine($"using Loqui;");
        mappingGen.AppendLine();
        using (mappingGen.Namespace(proto.DefaultNamespace))
        {
            using (var c = mappingGen.Class($"{proto.Protocol.Namespace}IsolatedAbstractInterfaceMapping"))
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
        
                mappingGen.AppendLine($"public {proto.Protocol.Namespace}IsolatedAbstractInterfaceMapping()");
                using (mappingGen.CurlyBrace())
                {
                    mappingGen.AppendLine($"var dict = new Dictionary<Type, {nameof(InterfaceMappingResult)}>();");
                    foreach (var rec in baseClasses.OrderBy(x => x.Name))
                    {
                        using (var args = mappingGen.Call(
                                   $"dict[typeof({rec.Interface(getter: false)})] = new {nameof(InterfaceMappingResult)}"))
                        {
                            args.Add("true");
                            await args.Add(async regisSb =>
                            {
                                regisSb.AppendLine($"new {nameof(ILoquiRegistration)}[]");
                                using (regisSb.CurlyBrace())
                                {
                                    foreach (var inheriting in await rec.InheritingObjects())
                                    {
                                        if (grupTypes.Contains(inheriting))
                                        {
                                            regisSb.AppendLine($"{inheriting.RegistrationName}.Instance,");
                                        }
                                    }
                                }
                            });
                            args.Add(regisSb =>
                            {
                                using (var c = regisSb.Call("new InterfaceMappingTypes"))
                                {
                                    c.Add($"Setter: typeof({rec.Interface(getter: false)})");
                                    c.Add($"Getter: typeof({rec.Interface(getter: true)})");
                                }
                            });
                        }
        
                        mappingGen.AppendLine(
                            $"dict[typeof({rec.Interface(getter: true)})] = dict[typeof({rec.Interface(getter: false)})] with {{ Setter = false }};");
                    }
        
                    mappingGen.AppendLine($"InterfaceToObjectTypes = dict;");
                }
            }
        }
        
        mappingGen.AppendLine();
        var mappingPath = Path.Combine(proto.DefFileLocation.FullName,
            $"../Interfaces/IsolatedAbstractInterfaceMapping{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        ExportStringToFile exportStringToFile = new();
        exportStringToFile.ExportToFile(mappingPath, mappingGen.GetString());
        proto.GeneratedFiles.Add(mappingPath, ProjItemType.Compile);
    }

    private static async Task GenerateInheritingMapping(ProtocolGeneration proto, HashSet<ObjectGeneration> grupTypes)
    {
        Dictionary<ObjectGeneration, ObjectGeneration> inheritingChildren = new();

        foreach (var obj in proto.ObjectGenerationsByName.Values)
        {
            if (grupTypes.Contains(obj)) continue;
            if (!await obj.IsMajorRecord()) continue;
            if (obj.BaseClass != null
                && !obj.BaseClass.Name.EndsWith("MajorRecord"))
            {
                inheritingChildren.Add(obj, obj.BaseClass);
            }
        }

        StructuredStringBuilder mappingGen = new StructuredStringBuilder();
        ObjectGeneration.AddAutogenerationComment(mappingGen);
        mappingGen.AppendLine($"using System;");
        mappingGen.AppendLine($"using System.Collections.Generic;");
        mappingGen.AppendLine($"using Mutagen.Bethesda.Plugins.Records.Mapping;");
        mappingGen.AppendLine($"using Loqui;");
        mappingGen.AppendLine();
        using (mappingGen.Namespace(proto.DefaultNamespace))
        {
            using (var c = mappingGen.Class($"{proto.Protocol.Namespace}InheritingInterfaceMapping"))
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

                mappingGen.AppendLine($"public {proto.Protocol.Namespace}InheritingInterfaceMapping()");
                using (mappingGen.CurlyBrace())
                {
                    mappingGen.AppendLine($"var dict = new Dictionary<Type, {nameof(InterfaceMappingResult)}>();");
                    foreach (var rec in inheritingChildren.OrderBy(x => x.Key.Name))
                    {
                        using (var args = mappingGen.Call(
                                   $"dict[typeof({rec.Key.Interface(getter: false)})] = new {nameof(InterfaceMappingResult)}"))
                        {
                            args.Add("true");
                            args.Add(regisSb =>
                            {
                                regisSb.AppendLine($"new {nameof(ILoquiRegistration)}[]");
                                using (regisSb.CurlyBrace())
                                {
                                    regisSb.AppendLine($"{rec.Value.RegistrationName}.Instance,");
                                }
                            });
                            args.Add(regisSb =>
                            {
                                using (var c = regisSb.Call("new InterfaceMappingTypes"))
                                {
                                    c.Add($"Setter: typeof({rec.Key.Interface(getter: false)})");
                                    c.Add($"Getter: typeof({rec.Key.Interface(getter: true)})");
                                }
                            });
                        }

                        mappingGen.AppendLine(
                            $"dict[typeof({rec.Key.Interface(getter: true)})] = dict[typeof({rec.Key.Interface(getter: false)})] with {{ Setter = false }};");
                    }

                    mappingGen.AppendLine($"InterfaceToObjectTypes = dict;");
                }
            }
        }

        mappingGen.AppendLine();
        var mappingPath = Path.Combine(proto.DefFileLocation.FullName,
            $"../Interfaces/InheritingInterfaceMapping{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
        ExportStringToFile exportStringToFile = new();
        exportStringToFile.ExportToFile(mappingPath, mappingGen.GetString());
        proto.GeneratedFiles.Add(mappingPath, ProjItemType.Compile);
    }
}