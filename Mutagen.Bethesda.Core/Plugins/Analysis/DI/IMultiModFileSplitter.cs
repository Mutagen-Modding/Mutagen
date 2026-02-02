using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

/// <summary>
/// Interface for splitting a mod into multiple files when master limits are exceeded.
/// </summary>
public interface IMultiModFileSplitter
{
    /// <summary>
    /// Splits a given input mod into multiple output mods, each containing at most the specified master limit.
    /// </summary>
    /// <typeparam name="TMod">Mutable mod type</typeparam>
    /// <typeparam name="TModGetter">Getter mod type</typeparam>
    /// <param name="inputMod">The mod to split</param>
    /// <param name="masterLimit">Maximum number of masters allowed per output mod</param>
    /// <returns>Collection of split mods</returns>
    IReadOnlyCollection<TMod> Split<TMod, TModGetter>(TMod inputMod, int masterLimit)
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter;
}
