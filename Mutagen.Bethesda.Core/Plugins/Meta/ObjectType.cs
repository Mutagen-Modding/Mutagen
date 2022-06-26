namespace Mutagen.Bethesda.Plugins.Meta;

/// <summary>
/// Categories of objects that can be generated
/// </summary>
public enum ObjectType
{
    /// <summary>
    /// A subrecord that is contained by a Record object.  No FormID
    /// </summary>
    Subrecord,
    /// <summary>
    /// A Record that is contained by a Group object, and contains Subrecords.  Has FormID
    /// </summary>
    Record,
    /// <summary>
    /// A list of Records or sub-Groups
    /// </summary>
    Group,
    /// <summary>
    /// The top level Mod object that contains all records
    /// </summary>
    Mod
}