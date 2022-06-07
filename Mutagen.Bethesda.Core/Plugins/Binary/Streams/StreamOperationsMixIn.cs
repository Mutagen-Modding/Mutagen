using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda;

public static class StreamOperationsMixIn
{
    /// <summary>
    /// Tries to locate a given subrecord in the upcoming stream.  Will stop looking if any unexpected types are encountered.  Stream's position
    /// will be moved to located record if found, otherwise it will be reset to its original position.
    /// </summary>
    public static bool TryScanToRecord<T>(this T stream, RecordType type, out SubrecordFrame foundRecord, IReadOnlyRecordCollection expectedTypes)
        where T : IMutagenReadStream
    {
        var pos = stream.Position;
        while (!stream.Complete)
        {
            var subRec = stream.ReadSubrecord();
            var recType = subRec.RecordType;
            if (!expectedTypes.Contains(recType))
            {
                stream.Position = pos;
                foundRecord = default;
                return false;
            }
            if (type == recType)
            {
                foundRecord = subRec;
                return true;
            }
        }

        stream.Position = pos;
        foundRecord = default;
        return false;
    }
}
