using Loqui;
using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class GetterInterfaceModule : GenerationModule
    {
        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            GenerateInterfaceGetters(obj, fg);
        }

        public static void GenerateInterfaceGetters(ObjectGeneration obj, FileGeneration fg)
        {
            TypeExt.LoadAssemblies();
            foreach (var interf in obj.Interfaces)
            {
                switch (interf.Type)
                {
                    case LoquiInterfaceDefinitionType.IGetter:
                    case LoquiInterfaceDefinitionType.Dual:
                        break;
                    case LoquiInterfaceDefinitionType.Direct:
                    case LoquiInterfaceDefinitionType.ISetter:
                    default:
                        continue;
                }
                Type t = GetType(interf.GetterInterface);
                if (t == null)
                {
                    throw new ArgumentException($"Could not find interface: {interf.GetterInterface}");
                }
            }
        }

        public static Type GetType(string name)
        {
            foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assemb.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }
                foreach (Type p in types)
                {
                    if (p.Name == name)
                    {
                        return p;
                    }
                }
            }
            return null;
        }
    }
}
