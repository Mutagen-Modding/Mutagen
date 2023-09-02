# Binary Utility
## BinaryStringUtility
Bethesda games store their strings on disk in a single byte format, with a null terminator.  There are some convenience parsing functions inside `BinaryStringUtility` to convert these to C# strings.

### ToZString
This function assumes the entire input span is bytes that need to be converted to a string.  There should be no null termination at the end, or it will be included in the resulting string.

### ProcessWholeToZString
This function assumes the entire input span is part of the string, and may have a null termination.  It will trim off the null termination if it exists, and return a string.  Good for typical use.
```cs
var subRecordBytes = ...;
var subRecordFrame = meta.SubRecordFrame(someBytes.AsSpan());
string str = BinaryStringUtility.ProcessWholeToZString(subRecordFrame.Content);
```

### ParseUnknownLengthString
Sometimes you might not be able to trim your input span to a string's contents exactly.  This call will help by locating the first null byte, and retrieving the string up until that point.
```cs
var bytesOfUnknownLength = ...;
string str = BinaryStringUtility.ParseUnknownLengthString(bytesOfUnknownLength.AsSpan());
var amountParsed = str.Length + 1; // +1 for the null termination that was trimmed
```


## SubRecord Iteration and Location
There are some convenience methods for iterating and locating Subrecords in a set of raw bytes in `UtilityTranslation`.  These are extension methods onto `HeaderConstants` and `MajorRecordFrame`, so they can be used directly from those objects.

### EnumerateSubrecords
Print some metadata for all Subrecords in a MajorRecord
```cs
var meta = HeaderConstants.Skyrim;
byte[] majorRecordBytes = ...;
var majorFrame = meta.MajorRecordFrame(majorRecordBytes.AsSpan());

foreach (KeyValuePair<RecordType, int> loc in majorFrame.EnumerateSubrecords())
{
    // Scope to the start of the subrecord's data, relative to the MajorRecord content
    var subRecordSpan = majorFrame.Content.Slice(loc.Value);
    var subHeader = meta.SubRecord(subRecordSpan);
    System.Console.WriteLine($"Found subrecord {loc.Key} at position {loc.Value}, with length {subHeader.ContentLength}");
}
```

### TryFindFirstSubrecord
This call returns the first location of a subrecord type.  Good for locating a specific subrecord
```cs
if (majorFrame.TryFindFirstSubrecord("EDID", out SubRecordFrame subFrame))
{
    System.Console.WriteLine($"Found EDID with length {subFrame.Header.ContentLength}: ");
    System.Console.WriteLine($"   {BinaryStringUtility.ProcessWholeToZString(subFrame.Content)}");
}
```

### FindFirstSubrecords
This call returns the first location of each subrecord type queried for.  Good for finding a set of subrecords in one pass.
```cs
var finds = majorFrame.FindFirstSubrecords("EDID", "FULL");
// If EDID found
if (finds[0] != -1)
{
    // Scope to the start of the subrecord's data, relative to the MajorRecord content
    var subRecordSpan = majorFrame.Content.Slice(finds[0]);
    var subFrame = meta.SubRecord(subRecordSpan);
    System.Console.WriteLine($"Found EDID with length {subFrame.Header.ContentLength}: ");
    System.Console.WriteLine($"   {BinaryStringUtility.ProcessWholeToZString(subFrame.Content)}");
}

// If FULL found
if (finds[1] != -1)
{
    // Scope to the start of the subrecord's data, relative to the MajorRecord content
    var subRecordSpan = majorFrame.Content.Slice(finds[1]);
    var subFrame = meta.SubRecord(subRecordSpan);
    System.Console.WriteLine($"Found FULL with length {subFrame.Header.ContentLength}: ");
    System.Console.WriteLine($"   {BinaryStringUtility.ProcessWholeToZString(subFrame.Content)}");
}
```

### CompileFirstSubrecordLocations
Similar to FindFirstSubrecords, except the results are put into a `Dictionary<RecordType, int>` for cleaner use.  Comes at the cost of higher overhead to construct this dictionary.  Another low cost alternative is just using `EnumerateSubrecords` with a `switch` statement.

```cs
var edid = new RecordType("EDID");
var full = new RecordType("FULL");
var finds = majorFrame.CompileFirstSubrecordLocations(edid, full);

// If EDID found
if (finds.TryGetValue(edid, out var loc))
{
    // Scope to the start of the subrecord's data, relative to the MajorRecord content
    var subRecordSpan = majorFrame.Content.Slice(loc);
    var subFrame = meta.SubRecord(subRecordSpan);
    System.Console.WriteLine($"Found EDID with length {subFrame.Header.ContentLength}: ");
    System.Console.WriteLine($"   {BinaryStringUtility.ProcessWholeToZString(subFrame.Content)}");
}

// If FULL found
if (finds.TryGetValue(full, out loc))
{
    // Scope to the start of the subrecord's data, relative to the MajorRecord content
    var subRecordSpan = majorFrame.Content.Slice(loc);
    var subFrame = meta.SubRecord(subRecordSpan);
    System.Console.WriteLine($"Found FULL with length {subFrame.Header.ContentLength}: ");
    System.Console.WriteLine($"   {BinaryStringUtility.ProcessWholeToZString(subFrame.Content)}");
}
```

## RecordLocator
`RecordLocator` is a convenience class that processes a mod stream and returns all the locations of Groups and MajorRecords.


It takes a few optional arguments:

- RecordInterest, to limit the search to specific `RecordType`s
- Additional Criteria lambda, to add custom filter logic

```cs
using var stream = new MutagenReadStream(pathToMod, HeaderConstants.Skyrim);
RecordLocator.FileLocations locs = RecordLocator.FileLocations.GetFileLocations(
    stream,
    // Only search for NPCs
    new RecordInterest("NPC_"),
    // That are compressed
    additionalCriteria: (stream, recordType, recLength) =>
    {
        var majorHeader = stream.Meta.GetMajorRecord(stream);
        return majorHeader.IsCompressed;
    });

/// Can enumerate Group locations
foreach (var groupLoc in locs.GrupLocations)
{
   System.Console.WriteLine($"Group located at: {groupLoc}");
}

/// Can enumerate MajorRecord locations
foreach (KeyValuePair<long, (FormID FormID, RecordType Record)> recLoc in locs.ListedRecords)
{
   System.Console.WriteLine($"MajorRecord located at: {recLoc.Key}, {recLoc.Value.FormID}");
}

/// Can query a specific FormID's location
if (locs.TryGetSection(someFormID, out RangeInt64 loc))
{
   stream.Position = loc.Min;
   var header = stream.Meta.ReadMajorRecord(stream);
   System.Console.WriteLine($"MajorRecord type: {header.RecordType}");
}

/// Can query what record exists at a location
if (locs.TryGetRecord(loc: 0x1234, out (FormID FormID, RecordType Record) rec))
{
   System.Console.WriteLine($"{rec.Record}: {rec.FormID}");
}

/// Can enumerate the parent Group locations for a FormID
foreach (var groupLoc in locs.GetContainingGroupLocations(someFormID))
{
   stream.Position = groupLoc;
   var groupHeader = stream.Meta.ReadGroupHeader(stream);
   System.Console.WriteLine($"{someFormID} has parent group at: {groupLoc}.  Type: {groupHeader.GroupType}");
}
```

This class just provides a common use case of locating records.  If more fine tuned logic is needed, just fall back to using [Header Struct](Header-Structs.md) parsing yourself with the extra logic you need.

Note that this class uses FormID, and does not make use of the abstraction concepts found in [ModKey, FormKey, FormLink](../plugins/ModKey,-FormKey,-FormLink.md)

## Decompression
MajorRecords with their compression flag enabled come in a compressed format.  The byte content needs to be unzipped before it can be read.
