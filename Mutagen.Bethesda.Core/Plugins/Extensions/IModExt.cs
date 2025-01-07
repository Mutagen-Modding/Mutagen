using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda;

public static class IModExt
{

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <typeparam name="TMajor">The type of Major Record to get the Group for</typeparam>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    public static IGroupGetter<TMajor> GetTopLevelGroup<TMajor>(this IModGetter mod) 
        where TMajor : IMajorRecordGetter
    {
        return mod.TryGetTopLevelGroup<TMajor>() ?? throw new ArgumentException($"Unknown major record type: {typeof(TMajor)}");
    }

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <param name="type">The type of Major Record to get the Group for</param>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    public static IGroupGetter GetTopLevelGroup(this IModGetter mod, Type type)
    {
        return mod.TryGetTopLevelGroup(type) ?? throw new ArgumentException($"Unknown major record type: {type}");
    }

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
    public static IGroup<TMajor> GetTopLevelGroup<TMajor>(this IMod mod)
        where TMajor : IMajorRecord
    {
        return mod.TryGetTopLevelGroup<TMajor>() ?? throw new ArgumentException($"Unknown major record type: {typeof(TMajor)}");
    }

    /// <summary>
    /// Returns the top-level Group getter object associated with the given Major Record Type.
    /// </summary>
    /// <returns>Group getter object associated with the given Major Record Type</returns>
    /// <param name="type">The type of Major Record to get the Group for</param>
    /// <exception cref="ArgumentException">
    /// An unexpected TMajor type will throw an exception.<br />
    /// Unexpected types include: <br />
    ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod) <br />
    ///   - Nested types, where there is not just one top level group that contains given type (Placed Objects) <br />
    ///   - A setter type is requested from a getter only object. <br />
    /// </exception>
    public static IGroup GetTopLevelGroup(this IMod mod, Type type)
    {
        return mod.TryGetTopLevelGroup(type) ?? throw new ArgumentException($"Unknown major record type: {type}");
    }

    public static MasterStyle GetMasterStyle(this IModFlagsGetter mod)
    {
        bool small = mod.CanBeSmallMaster && mod.IsSmallMaster;
        bool medium = mod.CanBeMediumMaster && mod.IsMediumMaster;
        
        if (small && medium)
        {
            throw new ModHeaderMalformedException(mod.ModKey, "Mod was both a light and medium master");
        }

        if (small) return MasterStyle.Small;
        if (medium) return MasterStyle.Medium;
        return MasterStyle.Full;
    }
}