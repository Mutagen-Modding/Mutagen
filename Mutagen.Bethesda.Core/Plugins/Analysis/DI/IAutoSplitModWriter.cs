using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

/// <summary>
/// Interface for writing mods with automatic splitting when master limits are exceeded.
/// </summary>
public interface IAutoSplitModWriter
{
    /// <summary>
    /// Writes a mod to the specified path, automatically splitting if master limit is exceeded.
    /// </summary>
    /// <typeparam name="TMod">Mutable mod type</typeparam>
    /// <typeparam name="TModGetter">Getter mod type</typeparam>
    /// <param name="mod">The mod to write</param>
    /// <param name="path">Target file path</param>
    /// <param name="param">Binary write parameters</param>
    void Write<TMod, TModGetter>(
        TModGetter mod,
        FilePath path,
        BinaryWriteParameters param)
        where TMod : class, IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : class, IModGetter;
}
