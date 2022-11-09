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