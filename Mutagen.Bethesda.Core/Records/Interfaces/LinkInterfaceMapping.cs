using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Core
{
    internal static class LinkInterfaceMappingInternal
    {
        public static Dictionary<GameRelease, IReadOnlyDictionary<Type, Type[]>> Mappings = new Dictionary<GameRelease, IReadOnlyDictionary<Type, Type[]>>();

        static LinkInterfaceMappingInternal()
        {
            if (!LinkInterfaceMapping.AutomaticRegistration) return;
            foreach (var interf in TypeExt.GetInheritingFromInterface<ILinkInterfaceMapping>(
                loadAssemblies: true))
            {
                ILinkInterfaceMapping? mapping = Activator.CreateInstance(interf) as ILinkInterfaceMapping;
                Mappings[mapping!.GameRelease] = mapping!.InterfaceToObjectTypes;
            }
        }
    }

    public static class LinkInterfaceMapping
    {
        public static bool AutomaticRegistration = true;

        public static IReadOnlyDictionary<Type, Type[]> InterfaceToObjectTypes(GameRelease mode)
        {
            return LinkInterfaceMappingInternal.Mappings[mode];
        }

        public static void Register(ILinkInterfaceMapping mapping)
        {
            LinkInterfaceMappingInternal.Mappings[mapping!.GameRelease] = mapping!.InterfaceToObjectTypes;
        }
    }
}
