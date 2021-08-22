using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Testing.AutoData
{
    public class ModListingBuilder : ISpecimenBuilder
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
                    if (t == typeof(ModListing))
                    {
                        var keys = context.CreateMany<ModKey>();
                        return keys.Select(x => new ModListing(x, true));
                    }
                }
            }
            else if (request is Type t)
            {
                if (t == typeof(ModListing))
                {
                    return new ModListing(TestConstants.PluginModKey, true);
                }
            }
            
            return new NoSpecimen();
        }
    }
}