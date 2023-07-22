# Header Structs
## General Concept
Header Structs are lightweight overlays that "lay" on top of some bytes and offers API to retrieve the various header fields or content bytes they contain.  They are extremely cheap to create, as they do no parsing unless asked.  They are aware of any differences in data alignments from game to game, so the same systems can be applied even if alignments change slightly.

Using Header Structs, very performant and low level parsing is possible while retaining a large degree of safety and usability.  

Some notable features:
- **Alignment is handled internally**.  User can access the fields they are interested in, without needing to worry about proper offsetting.
- **No parsing is done except what the user asks for**.  If only one field is accessed, then most of the header data will remain unparsed, and that work skipped.
- **Code written with this setup can work with any Bethesda game**, as swapping out [Game Constants](Game-Constants.md) will realign everything properly.
- **No data is copied**, as the structs are simply overlaid on top of the original source bytes.

They still require a lot of knowledge of the underlying binary structures of a mod, but the system goes a long way to empower the user to do it quickly, and with minimal potential for typo or misalignment errors.

## Example Usage
The following code will print all EditorIDs of all npcs from any game type.
```csharp
var modPath = ModPath.FromPath("SomeFolder/Skyrim.esm");
var release = GameRelease.SkyrimSE;
var encoding = MutagenEncodingProvider.Instance.GetEncoding(release, Language.English);

using var stream = new MutagenBinaryReadStream(modPath, release);

// Skip mod header
stream.ReadModHeaderFrame();

// Keep reading group frames out of the stream
while (stream.TryReadGroupFrame(out var groupFrame))
{
    // Check that the group contains NPCs
    if (groupFrame.ContainedRecordType != RecordTypes.NPC_) continue;

    // Loop over all major record structs in the group's content
    foreach (var majorPin in groupFrame)
    {
        // Iterate and search the subrecords for EDID
        if (majorPin.Frame.TryLocateSubrecordFrame(RecordTypes.EDID, out var subFrame))
        {
            // Interpret the subrecord's content as a string and print
            System.Console.WriteLine($"{majorFrame.FormID} => {subFrame.AsString(encoding)}");
        }
    }

    // We found a matching NPC group, we'll assume there's no others and break
    break;
}
```

This code will only do the minimal parsing necessary to locate/print the EditorIDs.  Most data will be skipped over and left unparsed.

## Headers, Frames and Pins
Header Structs come in a few combinations and flavors.  The above code makes use of several of them.
### Categories
There are Header Structs for:
- Groups
- MajorRecords
- Subrecords
- ModHeader

These are the few different types of records we can expect to encounter in a mod file, and there is a separate struct for each, offering the specific API for its type.

### Flavors
Each category also comes in a few flavors.

#### Header
This is the most basic version that has been discussed in the descriptions above.  It overlays on top of bytes and offers API to access the various aspects of the header.

Typical accessors include:
- `RecordType` that the header is (EDID, NPC_, etc)
- `HeaderLength`
- `ContentLength`
- `TotalLength`
- Other fields more specialized for the category (`MajorRecordFlags`, `FormID`, etc)

All of these fields align themselves properly by bouncing off a [Game Constants](Game-Constants.md)object which has all the appropriate alignment information.

#### Frame
Frames add a single additional member `ReadOnlyMemorySlice<byte> Content { get; }`, and thus overlay on top of a whole record in its entirety, both the header and its content.  This struct offers a nice easy package to access anything about an entire record in one location.

#### Pin
Pins add yet another single additional member `int Location { get; }`.  This represents the location a frame exists relative to its parent.  This facilitates parsing and operations where knowing a record's location is important.

## Additional Functionality
## Iteration
Both Group and MajorRecord Frames offer iteration functionality.
```
foreach (var subrecordPin in majorRecordFrame)
{
   ...
}
```
This allows the user to easily iterate and process contained records without needing to manually construct and align the headers themselves.

### Subrecord Location
MajorRecord Frames also have API for searching for a specific subrecord type.

```
var edidType = new RecordType("EDID");
if (majorRecordFrame.TryLocateSubrecordFrame(edidType, out var edidFrame))
{
   ...
}
```

This allows users to easily locate a specific record they are looking for, without needing to iterate and search themselves.

Note that it does iterate each Subrecord internally, so it is not a good solution if you are trying to process/find a large portion of Subrecords within a single Major Record.  It is more appropriate for finding one or two specific ones.  If you want to process all subrecords by type, it is recommended you iterate and switch on the type directly, or store the resulting SubrecordPins in a dictionary for later use.

### Subrecord Frame Data Interpretation
#### Primitives
Once a Subrecord Frame is located that you wish to retrieve data from, the content is still only offered as raw bytes (or rather, `ReadOnlyMemorySlice<byte>`).  There are a lot of functions to help interpret that data to the appropriate type, while confirming correctness.
```
var subrecordFrame = ...;
int contentAsInt = subrecordFrame.AsInt32();
```
This code will interpret the Subrecord Frame's content as an int for you.  It will also do the additional safety check to confirm that the Subrecord's content is exactly 4 bytes.  If the content length is 3, or 5, say, then the call will throw an exception alerting you to an unexpected length.

If you happen to want to extract an integer without enforcing that the content is exactly 4 bytes, then accessing the byte slice directly is the route to take.
```
var subrecordFrame = ...;
int contentAsInt = subrecordFrame.Content.Int32();
```
This route will not do the check to enforce that the content length is exactly 4.  It would only throw if the content wasn't long enough to be an int at all (less than 4).
#### Strings
Strings, unlike primitives, do not have a set length.  So the call to interpret a Subrecord Frame's content as a string is just for convenience, and does not add any safety mechanisms.
```
var subrecordFrame = ...;
string contentAsString = subrecordFrame.AsString();
```

## Writable Structs
All the above concepts mentioned have been read-only.  Header Structs can be overlaid on top of spans, and read data from them.

There are writable structs as well, which have both getter and setter API.  You can then read a section of data, and then make modifications which will affect the source `byte[]` at the correct indices.

These systems are less mature, but will be expanded on in the future.
