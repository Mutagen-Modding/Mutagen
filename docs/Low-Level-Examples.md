<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [Record Navigation Using Header Structs](#record-navigation-using-header-structs)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

# Record Navigation Using Header Structs
The following code will open a _simple_ mod file from disk, and print all EditorIDs. 

```cs
var meta = GameConstants.Oblivion;
var edid = new RecordType("EDID");
using var stream = new BinaryReadStream(File.Open(path));

// Read/Skip mod header
stream.ReadModHeader(meta);

// Read while stream has more data
while (stream.Remaining > 0)
{
    // Read bytes then overlay a GroupHeader struct
    var groupMeta = stream.ReadGroupHeader(meta);

    // Find the final position of the group's data
    var finalPos = stream.Position + groupMeta.ContentLength;

    // Read while still in group's data
    while (stream.Position < finalPos)
    {
        // Note we're switching to "Frame" mechanics, just for kicks
        var majorFrame = stream.ReadMajorRecord(meta);
        
        // Iterate over subrecords
        foreach (var subRecord in majorFrame.EnumerateSubrecords())
        {
            if (subRecord.Header.RecordType == edid)
            {
                System.Console.WriteLine(subRecord.AsString(MutagenEncodingProvider.Instance.GetEncoding(GameRelease.Oblivion, Language.English)));
            }
        }
    }
}
```

Note that the above code does not handle the complexity of Sub-Groups such as Cells/Worldspaces, and will break if the mod contains those types of records, unless upgraded to also handle them. 

However, it does give an extreme amount of control and flexibility of logic, while still abstracting the error prone concepts of header alignments away from the user.