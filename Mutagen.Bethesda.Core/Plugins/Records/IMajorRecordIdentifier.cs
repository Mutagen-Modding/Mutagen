namespace Mutagen.Bethesda.Plugins.Records;

public interface IMajorRecordIdentifier : IFormKeyGetter
{
    /// <summary>
    /// The usually unique string identifier assigned to the Major Record
    /// </summary>
    string? EditorID { get; }
}