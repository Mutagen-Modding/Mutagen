using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface that Mod objects implement to hook into the common getter systems
    /// </summary>
    public interface IModGetter : IMajorRecordGetterEnumerable, ILinkedFormKeyContainer
    {
        /// <summary>
        /// The associated game release
        /// </summary>
        GameRelease GameRelease { get; }

        /// <summary>
        /// Read only list of master reference getters.
        /// </summary>
        /// <returns>Read only list of master reference getters</returns>
        IReadOnlyList<IMasterReferenceGetter> MasterReferences { get; }

        /// <summary>
        /// Returns the Group getter object associated with the given Major Record Type.
        /// </summary>
        /// <returns>Group getter object associated with the given Major Record Type</returns>
        /// <typeparam name="TMajor">The type of Major Record to get the Group for</typeparam>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.
        /// Unexpected types include:
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        IReadOnlyCache<TMajor, FormKey> GetGroupGetter<TMajor>() where TMajor : IMajorRecordCommonGetter;

        /// <summary>
        /// Exports to disk in Bethesda binary format.
        /// Access and iterates through the mod object's contents in a single thread.
        /// </summary>
        /// <param name="path">Path to export to</param>
        /// <param name="param">Optional customization parameters</param>
        void WriteToBinary(string path, BinaryWriteParameters? param = null);

        /// <summary>
        /// Exports to disk in Bethesda binary format.
        /// Access and iterates through the mod groups in separate threads.  All provided mod objects
        /// are thread safe to use with this function.
        /// </summary>
        /// <param name="path">Path to export to</param>
        /// <param name="param">Optional customization parameters</param>
        void WriteToBinaryParallel(string path, BinaryWriteParameters? param = null);

        /// <summary>
        /// The key associated with this mod
        /// </summary>
        ModKey ModKey { get; }

        /// <summary>
        /// Whether a mod supports localization features
        /// </summary>
        bool CanUseLocalization { get; }
    }

    /// <summary>
    /// An interface that Mod objects implement to hook into the common systems
    /// </summary>
    public interface IMod : IModGetter, IMajorRecordEnumerable, IFormKeyAllocator
    {
        /// <summary>
        /// List of master references.
        /// </summary>
        /// <returns>List of master references</returns>
        new IList<MasterReference> MasterReferences { get; }

        /// <summary>
        /// Returns the Group object associated with the given Major Record Type.
        /// </summary>
        /// <returns>Group object associated with the given Major Record Type</returns>
        /// <typeparam name="TMajor">The type of Major Record to get the Group for</typeparam>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.
        /// Unexpected types include:
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        ICache<TMajor, FormKey> GetGroup<TMajor>() where TMajor : IMajorRecordCommon;

        uint NextFormID { get; set; }

        void SyncRecordCount();
    }

    /// <summary>
    /// An interface for Overlay mod systems
    /// </summary>
    public interface IModDisposeGetter : IModGetter, IDisposable
    {
    }
}
