using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using Noggog.Testing.IO;
using NSubstitute;

namespace Mutagen.Bethesda.Testing.AutoData;

public class OrderBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type t)
        {
            if (t == typeof(ICreationClubEnabledProvider))
            {
                var def = new CreationClubEnabledProvider(
                    context.Create<IGameCategoryContext>()).Used;
                var ret = Substitute.For<ICreationClubEnabledProvider>();
                ret.Used.Returns(def);
                return ret;
            }
            else if (t == typeof(IPluginListingsPathContext))
            {
                var ret = Substitute.For<IPluginListingsPathContext>();
                ret.Path.Returns(new FilePath($"{PathingUtil.DrivePrefix}{Path.Combine("ExistingDirectory", "Plugins.txt")}"));
                return ret;
            }
            else if (t == typeof(ICreationClubListingsPathProvider))
            {
                var def = new CreationClubListingsPathProvider(
                    context.Create<IGameCategoryContext>(),
                    context.Create<ICreationClubEnabledProvider>(),
                    context.Create<IGameDirectoryProvider>()).Path;
                var ret = Substitute.For<ICreationClubListingsPathProvider>();
                ret.Path.Returns(def);
                return ret;
            }
            else if (t == typeof(ILoadOrderListingsProvider))
            {
                var ret = Substitute.For<ILoadOrderListingsProvider>();
                var keys = context.CreateMany<ModListing>();
                ret.Get().Returns(keys);
                return ret;
            }
        }
            
        return new NoSpecimen();
    }
}