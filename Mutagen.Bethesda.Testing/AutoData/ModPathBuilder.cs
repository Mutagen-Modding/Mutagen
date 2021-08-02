using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData
{
    public class ModPathBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is ParameterInfo p)
            {
                if (p.Name == null) return new NoSpecimen();
                if (p.Name.ContainsInsensitive("missing"))
                {
                    if (p.ParameterType == typeof(ModPath))
                    {
                        return new ModPath(
                            ModKey.FromNameAndExtension("MissingMod.esp"), 
                            Path.Combine(PathBuilder.ExistingDirectory, "GameDirectory", "DataDirectory", "MissingMod.esp"));
                    }
                }
                else if (p.Name.ContainsInsensitive("existing"))
                {
                    if (p.ParameterType == typeof(ModPath))
                    {
                        return new ModPath(
                            TestConstants.PluginModKey.FileName,
                            Path.Combine(PathBuilder.ExistingDirectory, "GameDirectory", "DataDirectory", TestConstants.PluginModKey.FileName));
                    }
                }
            }
            if (request is MultipleRequest mult)
            {
                var req = mult.Request;
                if (req is SeededRequest seed)
                {
                    req = seed.Request;
                }
                if (req is Type t)
                {
                    if (t == typeof(ModPath))
                    {
                        var dataDir = context.Create<IDataDirectoryProvider>();
                        return new ModKey[]
                        {
                            TestConstants.MasterModKey,
                            TestConstants.PluginModKey,
                            TestConstants.PluginModKey2,
                        }.Select(mk =>
                        {
                            return new ModPath(mk, Path.Combine(dataDir.Path, mk.FileName));
                        });
                    }
                }
            }
            else if (request is Type t)
            {
                var modKey = TestConstants.PluginModKey;
                if (t == typeof(ModPath))
                {
                    var dataDir = context.Create<IDataDirectoryProvider>();
                    return new ModPath(modKey, Path.Combine(dataDir.Path, modKey.FileName));
                }
            }

            return new NoSpecimen();
        }
    }
}