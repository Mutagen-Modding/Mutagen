using System;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;

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
                var modKey = TestConstants.PluginModKey;
                if (t == typeof(ModKey))
                {
                    return modKey;
                }
            }

            return new NoSpecimen();
        }
    }
}