using System.IO.Abstractions;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace Mutagen.Bethesda.Testing.AutoData;

public class GameEnvironmentBuilder : ISpecimenBuilder
{
    private readonly GameRelease _release;

    public GameEnvironmentBuilder(GameRelease release)
    {
        _release = release;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }

        if (request is not Type t
            || !t.InheritsFrom(typeof(IGameEnvironment)))
        {
            return new NoSpecimen();
        }

        var fs = context.Create<IFileSystem>();
        var pluginPath = context.Create<IPluginListingsPathContext>();
        var dataDirectoryProvider = context.Create<IDataDirectoryProvider>();
        var cccListingPathContext = context.Create<ICreationClubListingsPathProvider>();
        var modKeys = context.Create<IEnumerable<ModKey>>();
        var rel = new GameReleaseInjection(_release);
        var loWriter = new LoadOrderWriter(
            fs,
            new HasEnabledMarkersProvider(
                rel),
            new ImplicitListingModKeyProvider(
                rel));
        
        loWriter.Write(pluginPath.Path, modKeys.Select((x, i) => new LoadOrderListing(x, i % 2 == 0)));

        var mods = modKeys.Select(x => ModInstantiator.Activator(x, _release)).ToArray();
        mods.ForEach(m => m.WriteToBinary(Path.Combine(dataDirectoryProvider.Path, m.ModKey.FileName), new BinaryWriteParameters()
        {
            FileSystem = fs
        }));

        var resolver = new Func<Type, object?>(t =>
        {
            if (t == typeof(IFileSystem)) return fs;
            if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
            if (t == typeof(IPluginListingsPathContext)) return pluginPath;
            if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
            return default;
        });

        if (t.GenericTypeArguments.Length == 2)
        {
            return GetCreateMethod()
                .MakeGenericMethod(t.GenericTypeArguments[0], t.GenericTypeArguments[1])
                .Invoke(this, new object[] { _release, fs, resolver})!;
        }
        else
        {
            return GameEnvironment.Typical.Builder(_release)
                .WithFileSystem(fs)
                .WithResolver(resolver)
                .Build();
        }
    }

    public static MethodInfo GetCreateMethod()
    {
        return typeof(GameEnvironmentBuilder)
            .GetMethod("CreateEnvironment", BindingFlags.Static | BindingFlags.Public)!;
    }

    public static IGameEnvironment<TModSetter, TModGetter> CreateEnvironment<TModSetter, TModGetter>(
        GameRelease gameRelease,
        IFileSystem fs,
        Func<Type, object?> resolver)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
    {
        return GameEnvironment.Typical.Builder<TModSetter, TModGetter>(gameRelease)
            .WithFileSystem(fs)
            .WithResolver(resolver)
            .Build();
    }
}