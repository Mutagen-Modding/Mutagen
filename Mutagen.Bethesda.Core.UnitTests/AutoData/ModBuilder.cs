using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using NSubstitute;

namespace Mutagen.Bethesda.Core.UnitTests.AutoData
{
    public class ModBuilder : ISpecimenBuilder
    {
        private readonly GameRelease _release;

        public ModBuilder(GameRelease release)
        {
            _release = release;
        }
        
        public object Create(object request, ISpecimenContext context)
        {
            if (request is MultipleRequest mult)
            {
                var req = mult.Request;
                if (req is SeededRequest seed)
                {
                    req = seed.Request;
                }
                if (request is not Type t) return new NoSpecimen();
                if (t == typeof(IMod)
                    || t == typeof(IModGetter))
                {
                    var modKeys = context.Create<IEnumerable<ModKey>>();
                    return modKeys.Select(mk =>
                    {
                        return GetMod(mk);
                    }).ToArray<object>();
                }
            }
            else
            {
                if (request is SeededRequest seed)
                {
                    request = seed.Request;
                }

                if (request is not Type t) return new NoSpecimen();
                if (t == typeof(IMod)
                    || t == typeof(IModGetter))
                {
                    return GetMod(context.Create<ModKey>());
                }
            }
            
            return new NoSpecimen();
        }

        public IMod GetMod(ModKey modKey)
        {
            var ret = Substitute.For<IMod>();
            ret.ModKey.Returns(modKey);
            ret.GameRelease.Returns(_release);
            ret.NextFormID = 0x800;;
            return ret;
        }
    }
}