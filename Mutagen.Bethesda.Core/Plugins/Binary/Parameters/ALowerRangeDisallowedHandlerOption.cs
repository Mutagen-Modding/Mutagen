using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// A class representing how to handle when lower formID ranges are used in a non-allowed way. <br />
/// Typically this occurs when the lower ranges are used without any masters present <br />
/// Options: <br />
///   NoCheckIfLowerRangeDisallowed
///   ThrowIfLowerRangeDisallowed
///   AddPlaceholderMasterIfLowerRangeDisallowed
/// </summary>
public abstract class ALowerRangeDisallowedHandlerOption
{
    public static NoCheckIfLowerRangeDisallowed NoCheck { get; } = new NoCheckIfLowerRangeDisallowed();
    public static ThrowIfLowerRangeDisallowed Throw { get; } = new ThrowIfLowerRangeDisallowed();
    public static AddPlaceholderMasterIfLowerRangeDisallowed AddPlaceholder(ModKey key)
    {
        return new AddPlaceholderMasterIfLowerRangeDisallowed()
        {
            ModKey = key
        };
    }

    public static AddPlaceholderMasterIfLowerRangeDisallowed AddPlaceholder(ILoadOrderGetter loadOrder)
    {
        return new AddPlaceholderMasterIfLowerRangeDisallowed()
        {
            ModKey = loadOrder.ListedOrder.Select<ModKey, ModKey?>(x => x).FirstOrDefault()
        };
    }

    public static AddPlaceholderMasterIfLowerRangeDisallowed AddPlaceholder(IEnumerable<ModKey> loadOrder)
    {
        return new AddPlaceholderMasterIfLowerRangeDisallowed()
        {
            ModKey = loadOrder.Select<ModKey, ModKey?>(x => x).FirstOrDefault()
        };
    }
}

/// <summary>
/// A class representing how to handle when lower formID ranges are used in a non-allowed way. <br />
/// Typically this occurs when the lower ranges are used without any masters present. <br />
/// This option skips any checking of this situation
/// </summary>
public class NoCheckIfLowerRangeDisallowed : ALowerRangeDisallowedHandlerOption
{
}


/// <summary>
/// A class representing how to handle when lower formID ranges are used in a non-allowed way. <br />
/// Typically this occurs when the lower ranges are used without any masters present. <br />
/// If this occurs with this option, an exception will be thrown
/// </summary>
public class ThrowIfLowerRangeDisallowed : ALowerRangeDisallowedHandlerOption
{
}

/// <summary>
/// A class representing how to handle when lower formID ranges are used in a non-allowed way. <br />
/// Typically this occurs when the lower ranges are used without any masters present. <br />
/// If this occurs with this option, the base mod (first on the load order) will be added as a placeholder.
/// If no load order exists, then an exception will be thrown
/// </summary>
public class AddPlaceholderMasterIfLowerRangeDisallowed : ALowerRangeDisallowedHandlerOption
{
    public ModKey? ModKey { get; init; }
}
