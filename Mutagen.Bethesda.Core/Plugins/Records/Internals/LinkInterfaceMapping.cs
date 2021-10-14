using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Records.Internals
{
    internal static class LinkInterfaceMappingInternal
    {
        public static Dictionary<GameCategory, IReadOnlyDictionary<Type, Type[]>> Mappings = new Dictionary<GameCategory, IReadOnlyDictionary<Type, Type[]>>();
        public static Dictionary<string, Type> NameToInterfaceTypeMapping = new Dictionary<string, Type>();

        static LinkInterfaceMappingInternal()
        {
            if (!LinkInterfaceMapping.AutomaticRegistration) return;
            foreach (var interf in TypeExt.GetInheritingFromInterface<ILinkInterfaceMapping>(
                loadAssemblies: true))
            {
                Register((Activator.CreateInstance(interf) as ILinkInterfaceMapping)!);
            }
        }

        public static void Register(ILinkInterfaceMapping mapping)
        {
            Mappings[mapping.GameCategory] = mapping.InterfaceToObjectTypes;
            foreach (var interf in mapping.InterfaceToObjectTypes.Keys)
            {
                NameToInterfaceTypeMapping[interf.FullName!] = interf;
            }
        }
    }

    public static class LinkInterfaceMapping
    {
        public static bool AutomaticRegistration = true;

        public static IReadOnlyDictionary<Type, Type[]> InterfaceToObjectTypes(GameCategory mode)
        {
            if (LinkInterfaceMappingInternal.Mappings.TryGetValue(mode, out var value))
            {
                return value;
            }

            return DictionaryExt.Empty<Type, Type[]>();
        }

        public static bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type)
        {
            return LinkInterfaceMappingInternal.NameToInterfaceTypeMapping.TryGetValue(name, out type);
        }

        public static void Register(ILinkInterfaceMapping mapping)
        {
            LinkInterfaceMappingInternal.Register(mapping);
        }
    }
}
