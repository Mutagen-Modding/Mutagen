There are a few options for turning a Mutagen object into a `string``.

## ToString
If you call `ToString()` on a Mutagen object, you will typically not see any content within the string.  You will usually get just the class name, or some short but informative identifying string, such as Major Records returning their FormKey:

`Skyrim.Npc 123456:Skyrim.esm`

## Print
All Mutagen objects implement `IPrintable`, which exposes a `Print()` method.   Calling this will do a more verbose printing of the content.

``` cs
var str = npc.Print();
```

### StructuredStringBuilder

If you are printing multiple objects, or have more complex needs, you might consider utilizing the `StructuredStringBuilder`.  This is a more advanced `StringBuilder` with some extra functionality.

``` { .cs hl_lines=4 }
StructuredStringBuilder sb = ...;
sb.AppendLine("The NPC had the content:");

npc.Print(sb);

var str = sb.ToString();
```
