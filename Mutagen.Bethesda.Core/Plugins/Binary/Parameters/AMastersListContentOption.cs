namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// An abstract class representing a logic choice for the content of masters
/// </summary>
public class AMastersListContentOption
{
    public static implicit operator AMastersListContentOption(MastersListContentOption option)
    {
        return new MastersListContentEnumOption()
        {
            Option = option
        };
    }

    /// <summary>
    /// Overrides the masters to be an explicitly set list
    /// </summary>
    /// <param name="modKeys">ModKeys to set the masters to</param>
    public static AMastersListContentOption Override(IReadOnlyCollection<ModKey> modKeys)
    {
        return new MastersListContentOverrideOption(modKeys);
    }
}