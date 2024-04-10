namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// A class representing how to handle determining what the minimum allowed FormID is. <br />
/// Options: <br />
///   AutomaticLowerFormIdRangeOption <br />
///   ForceLowerFormIdRangeOption
/// </summary>
public abstract class AMinimumFormIdOption
{
    public static AutomaticLowerFormIdRangeOption Automatic { get; } = new AutomaticLowerFormIdRangeOption();
    public static ForceLowerFormIdRangeOption Force(bool on)
    {
        return new ForceLowerFormIdRangeOption()
        {
            ForceLowerRangeSetting = on,
        };
    }
}

/// <summary>
/// Instructions to handle determining what the minimum allowed FormID is by looking at the
/// header versions and game releases involved.
/// </summary>
public class AutomaticLowerFormIdRangeOption : AMinimumFormIdOption
{
}

/// <summary>
/// Instructions to handle determining what the minimum allowed FormID is by forcing it on/off
/// </summary>
public class ForceLowerFormIdRangeOption : AMinimumFormIdOption
{
    /// <summary>
    /// Whether to force using lower FormID ranges on or off
    /// </summary>
    public bool ForceLowerRangeSetting { get; init; }
}