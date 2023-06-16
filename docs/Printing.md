(Latest version of Mutagen does not match these docs.  Will soon be updated to match this documentation)

# Normal ToString
If you call `ToString()` on a Mutagen object, you will typically not see any content within the string.  You will usually get just the class name, or some short but informative identifying string, such as Major Records returning their FormKey:

`Skyrim.Npc 123456:Skyrim.esm`

# IPrintable and StructuredStringBuilder
All Mutagen objects implement `IPrintable`, which exposes a `Print()` method.  Calling this will do a more verbose printing of the content:
```cs
var str = npc.Print();
```
