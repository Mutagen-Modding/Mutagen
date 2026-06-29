namespace Mutagen.Bethesda.Plugins.Binary.Translations;

/// <summary>
/// Binary storage patterns for strings
/// </summary>
public enum StringBinaryType
{
    /// <summary>
    /// No custom logic
    /// </summary>
    Plain,
    /// <summary>
    /// Terminated by an extra null character at the end
    /// </summary>
    NullTerminate,
    /// <summary>
    /// Terminated by an extra null character at the end if there is content
    /// </summary>
    NullTerminateIfNotEmpty,
    /// <summary>
    /// Length prepended as a uint
    /// </summary>
    PrependLength,
    /// <summary>
    /// Length prepended as a uint, with null termination if there's content
    /// </summary>
    PrependLengthWithNullIfContent,
    /// <summary>
    /// Length prepended as a ushort
    /// </summary>
    PrependLengthUShort,
    /// <summary>
    /// Length prepended as a byte
    /// </summary>
    PrependLengthUInt8,
    /// <summary>
    /// Fixed-width slot of a known byte count, zero-padded.  Requires byteLength on the XML field.
    /// </summary>
    NullPaddedFixedLength,
}