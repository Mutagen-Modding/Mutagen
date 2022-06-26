namespace Mutagen.Bethesda.Strings;

public interface IOptionalStringsKeyGetter
{
    /// <summary>
    /// Key related to a strings file index
    /// </summary>
    uint? StringsKey { get; }
}