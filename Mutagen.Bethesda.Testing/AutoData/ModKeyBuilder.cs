using System;
using System.IO;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Testing.AutoData
{
    public class ModKeyBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is MultipleRequest mult)
            {
                var req = mult.Request;
                if (req is SeededRequest seed)
                {
                    req = seed.Request;
                }
                if (req is Type t)
                {
                    if (t == typeof(ModKey))
                    {
                        return new object[]
                        {
                            TestConstants.MasterModKey,
                            TestConstants.PluginModKey,
                            TestConstants.PluginModKey2,
                        };
                    }
                }
            }
            else if (request is Type t)
            {
                if (t == typeof(ModKey))
                {
                    return GetRandomModKey(ModType.Plugin);
                }
            }

            return new NoSpecimen();
        }

        public static ModKey GetRandomModKey(ModType type)
        {
            var fileName = new FileName(Path.GetRandomFileName());
            return new ModKey(fileName.NameWithoutExtension, ModType.Plugin);
        }
    }
}