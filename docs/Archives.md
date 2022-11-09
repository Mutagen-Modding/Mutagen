<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [Reading](#reading)
  - [Archive Reader](#archive-reader)
    - [File Enumeration](#file-enumeration)
    - [Folder Lookup](#folder-lookup)
- [Finding Applicable Archives](#finding-applicable-archives)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

Certain Bethesda files like textures, meshes, and similar assets are often stored in zipped up files with extensions like `.bsa` or `b2a`.

Mutagen calls these `Archives` and offers API to read the contents from those.   Writing new Archives is not something Mutagen can currently do, but is on the list of features to eventually be added.

# Reading
## Archive Reader
To start reading an Archive, you must make an Archive Reader:
```cs
var reader = Archive.CreateReader(GameRelease.SkyrimSE, somePathToBSA);
```

### File Enumeration
With an Archive Reader, you can enumerate all the files it contains:
```cs
foreach (var file in reader.Files)
{
    Console.WriteLine($"File at {file.Path}, with size {file.Size}");

    var fileBytes = file.GetBytes();
    // You can also get the file data by stream
}
```

### Folder Lookup
Archive Readers can also look up specific folders:
```cs
if (reader.TryGetFolder("some/sub/folder", out var archiveFolder))
{
   // Found the folder inside the archive
}
```

Folders have similar API of looping contained files as an Archive Reader

# Finding Applicable Archives
(Todo)