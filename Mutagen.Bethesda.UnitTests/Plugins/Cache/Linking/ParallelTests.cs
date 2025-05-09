using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public class ParallelTests
{
    [Theory, MutagenModAutoData]
    public void SerialResolveAll(
        SkyrimMod orig,
        SkyrimMod mod2,
        SkyrimMod mod3,
        SkyrimMod mod4,
        SkyrimMod mod5,
        Cell c1,
        Cell c2,
        Cell c3,
        Cell c4,
        Cell c5)
    {
        const int Times = 1000;
        var block = new CellBlock()
        {
            BlockNumber = 1,
            SubBlocks = new ExtendedList<CellSubBlock>()
            {
                new CellSubBlock()
                {
                    BlockNumber = 1,
                    Cells = new ExtendedList<Cell>()
                    {
                        c1, c2, c3, c4, c5
                    }
                }
            }
        };
        orig.Cells.Add(block);
        var origCache = orig.ToImmutableLinkCache();
        Cell[] cells =
        [
            c1, c2, c3, c4, c5
        ];
        var contexts = cells.Select(c =>
        {
            return origCache.ResolveContext(c);
        }).ToArray();
        SkyrimMod[] mods = [mod2, mod3, mod4, mod5];
        foreach (var mod in mods)
        {
            foreach (var context in contexts)
            {
                context.GetOrAddAsOverride(mod);
            }
        }
        mods = orig.AsEnumerable().And(mods).ToArray();
        var cache = mods.ToImmutableLinkCache();
        
        IModContext<IMajorRecordGetter>[,][] results = new IModContext<IMajorRecordGetter>[cells.Length, Times][];
        for (int cellCt = 0; cellCt < cells.Length; cellCt++)
        {
            var cell = cells[cellCt];
            for (int i = 0; i < Times; i++)
            {
                var result = cache.ResolveAllSimpleContexts(cell).ToArray();
                results[cellCt, i] = result;
            }
            var first = results[cellCt, 0];
            for (int i = 0; i < Times; i++)
            {
                var result = results[cellCt, i];
                if (!first.Select(x => x.ModKey).SequenceEqual(result.Select(x => x.ModKey)))
                {
                    throw new Exception("Uh oh");
                }
            }
        }
    }
    
    [Theory, MutagenModAutoData]
    public void ParallelResolveAll(
        SkyrimMod orig,
        SkyrimMod mod2,
        SkyrimMod mod3,
        SkyrimMod mod4,
        SkyrimMod mod5,
        Cell c1,
        Cell c2,
        Cell c3,
        Cell c4,
        Cell c5)
    {
        const int Times = 1000;
        var block = new CellBlock()
        {
            BlockNumber = 1,
            SubBlocks = new ExtendedList<CellSubBlock>()
            {
                new CellSubBlock()
                {
                    BlockNumber = 1,
                    Cells = new ExtendedList<Cell>()
                    {
                        c1, c2, c3, c4, c5
                    }
                }
            }
        };
        orig.Cells.Add(block);
        var origCache = orig.ToImmutableLinkCache();
        Cell[] cells =
        [
            c1, c2, c3, c4, c5
        ];
        var contexts = cells.Select(c =>
        {
            return origCache.ResolveContext(c);
        }).ToArray();
        SkyrimMod[] mods = [mod2, mod3, mod4, mod5];
        foreach (var mod in mods)
        {
            foreach (var context in contexts)
            {
                context.GetOrAddAsOverride(mod);
            }
        }
        mods = orig.AsEnumerable().And(mods).ToArray();
        var cache = mods.ToImmutableLinkCache();
        
        IModContext<IMajorRecordGetter>[,][] results = new IModContext<IMajorRecordGetter>[cells.Length, Times][];
        for (int cellCt = 0; cellCt < cells.Length; cellCt++)
        {
            var cell = cells[cellCt];
            Parallel.For(0, Times, i =>
            {
                var result = cache.ResolveAllSimpleContexts(cell).ToArray();
                results[cellCt, i] = result;
            });
            var first = results[cellCt, 0];
            for (int i = 0; i < Times; i++)
            {
                var result = results[cellCt, i];
                if (!first.Select(x => x.ModKey).SequenceEqual(result.Select(x => x.ModKey)))
                {
                    throw new Exception("Uh oh");
                }
            }
        }
    }
}