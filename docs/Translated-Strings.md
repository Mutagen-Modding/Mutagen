Newer Bethesda titles come with the ability for a single string record to have multiple different translations in different languages.  This is provided via the concept of extra `.strings` files, which provide an index to string mapping to alternate translations for a language.  On the mod file binary side, string record contents are replaced with an index that can be used to lookup a value in the strings files for the language desired.

# TranslatedString
Mutagen exposes this functionality via the `TranslatedString` class.  It can be used almost as a string, with its basic get/set accessor.  This will get/set the for language `TranslatedString.DefaultLanguage`, which is by default set to English.

```csharp
Npc npc = ...;
System.Console.WriteLine($"English name: {npc.Name}");
npc.Name = "New English Name";
```

## Multi-Language Support
`TranslatedString` also acts as a language dictionary in more advanced usage, allowing access to the alternate strings.
```csharp
Npc npc = ...;
if (npc.Name.TryLookup(Language.French, out var frenchString))
{
    System.Console.WriteLine($"French name: {frenchString}");
}

npc.Name.Set(Language.French, "Lutin");
```

## Data Loading Patterns
`TranslatedString` queries and loads data in a lazy fashion.  Strings files for any given language will not be loaded until they are asked for.   Loose strings file located for per language will be used if present, otherwise BSAs will be searched for the applicable strings language file. 

All operations are threadsafe for `TranslatedString`.

# Internals
## StringsLookupOverlay
This class lightly wraps the data from a single strings file, indexing the available string keys but doing no parsing on the strings themselves.  The raw string byte data is cached internally for easy random access.  Strings are parsed from the raw data on lookup, and the results not cached.  
All operations on `StringsLookupOverlay` are threadsafe.
```csharp
var stringsOverlay = new StringsLookupOverlay(pathToStringsFile);
uint key = 1234;
if (stringsOverlay.TryLookup(key, out var containedString))
{
    System.Console.WriteLine($"{pathToStringsFile} contained {key}: {containedString}");
}
```  

## StringsFolderLookupOverlay
This class overlays on top an entire data folder, allowing lazy access to all strings files within, both loose and inside BSA files.  Internal StringLookupOverlays will be created and cached when required to serve a query.  Loose strings files for a query will have priority, followed by the first applicable strings files found in the BSAs.  All operations on `StringsFolderLookupOverlay` are threadsafe.

```csharp
ModKey modKey = ...;
uint key = 1234;
Language language = Language.English;
StringsSource source = StringsSource.DL;

var stringsFolderOverlay = StringsFolderLookupOverlay.TypicalFactory(pathToDataFolder);
if (stringsFolderOverlay.TryLookup(source, language, modKey, key, out var containedString))
{
    System.Console.WriteLine($"{source} {language} contained {key}: {containedString}");
}
```

## StringsWriter
A class for exporting strings of various languages into the appropriate strings files.  This currently only supports export into Loose Files, not into a packed BSA.  A `StringsWriter` can be loaded with strings, and then exports to disk when disposed.
```csharp
ModKey modKey = ...;
StringsSource source = StringsSource.DL;
TranslatedString myStr = new TranslatedString();
myStr.String = "Goblin";
myStr.Set(Language.French, "Lutin");

using (var stringsWriter = new StringsWriter(modKey, pathToStringsFolder))
{
    uint key = stringsWriter.Register(myStr, source);
    System.Console.WriteLine($"{myStr} was registered into the strings files for {source} under key: {key}.");
}
```