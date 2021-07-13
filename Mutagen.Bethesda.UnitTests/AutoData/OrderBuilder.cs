using System;
using AutoFixture;
using AutoFixture.Kernel;
using FakeItEasy;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;

namespace Mutagen.Bethesda.UnitTests.AutoData
{
    public class OrderBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is not Type t) return new NoSpecimen();
            
            if (t == typeof(ICreationClubEnabledProvider))
            {
                var ret = A.Fake<ICreationClubEnabledProvider>();
                A.CallTo(() => ret.Used).ReturnsLazily(
                    () => new CreationClubEnabledProvider(context.Create<IGameCategoryContext>()).Used);
                return ret;
            }
            else if (t == typeof(IPluginListingsPathProvider))
            {
                return new PluginListingsPathInjection($"C:\\ExistingDirectory\\Plugins.txt");
            }
            else if (t == typeof(ICreationClubListingsPathProvider))
            {
                var ret = A.Fake<ICreationClubListingsPathProvider>();
                A.CallTo(() => ret.Path).ReturnsLazily(() =>
                {
                    return new CreationClubListingsPathProvider(
                        context.Create<IGameCategoryContext>(),
                        context.Create<ICreationClubEnabledProvider>(),
                        context.Create<IGameDirectoryProvider>()).Path;
                });
                return ret;
            }
            
            return new NoSpecimen();
        }
    }
}