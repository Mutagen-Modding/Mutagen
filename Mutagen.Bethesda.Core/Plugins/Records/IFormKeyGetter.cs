namespace Mutagen.Bethesda.Plugins.Records;

public interface IFormKeyGetter
{
    /// <summary>
    /// The unique identifier assigned to the Major Record
    /// </summary>
    FormKey FormKey { get; }
}