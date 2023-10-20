# Strings Files
Newer Bethesda titles come with the ability for a single string record to have multiple different translations in different languages.  

If a strings flag in the mod is turned on, then seprate `.strings` files are included as part of the mod "package".  This file contains an index to string mapping to alternate translations for a language.  On the mod file binary side, string record contents are replaced with an index that can be used to lookup a value in the strings files for the language desired.  

This section deals with how Mutagen itself interacts with Strings file concepts. You can read more about strings file structures on more generic modding documentation websites.

## Within Generated Classes
Within its generated classes, Mutagen exposes members of type `TranslatedString`.  This class provides the embedded language as a direct string, but also acts like a dictionary when localization is enabled and there are multiple potential languages.

`TranslatedString` queries and loads data in a lazy fashion.  Strings files for any given language will not be loaded until they are asked for.   Loose strings file located for per language will be used if present, otherwise BSAs will be searched for the applicable strings language file. 

All operations are threadsafe for `TranslatedString`.

### Simple String Access
If a mod is not localized, then strings are embedded.  You can easily get access to this string.

``` { .cs hl_lines=3-4}
Npc npc = ...;

System.Console.WriteLine($"Name: {npc.Name}");
npc.Name = "New Name";
```

### Default Language

When localization is turned on, then the simple string accesses the "default language", which is English, normally.   You can set the default language via a static field:
``` { .cs hl_lines=3 }
Npc npc = ...;

TranslatedString.DefaultLanguage = Language.French;

System.Console.WriteLine($"French name: {npc.Name}");
npc.Name = "Lutin";
```

!!! danger "Future Expansion"
    The route for setting the default language may change in the future to be less static, so use with caution.

### Multi-Language Support
`TranslatedString` also acts as a language dictionary when localization is turned on, allowing access to all the alternate strings for each language.
``` { .cs hl_lines="3 8" }
Npc npc = ...;

if (npc.Name.TryLookup(Language.French, out var frenchString))
{
    System.Console.WriteLine($"French name: {frenchString}");
}

npc.Name.Set(Language.French, "Lutin");
```

## Direct Strings File Access
Mutagen has API for interacting with strings files and folders directly.

### Reading Single Strings File
This class lightly wraps the data from a single strings file, indexing the available string keys but doing no parsing on the strings themselves.  The raw string byte data is cached internally for easy random access.  Strings are parsed from the raw data on lookup, and the results not cached.  
All operations on `StringsLookupOverlay` are threadsafe.

``` { .cs hl_lines=3-4 }
uint key = 1234;

var stringsOverlay = new StringsLookupOverlay(pathToStringsFile);
if (stringsOverlay.TryLookup(key, out var containedString))
{
    System.Console.WriteLine($"{pathToStringsFile} contained {key}: {containedString}");
}
```  

### Reading Strings Folder
This class overlays on top an entire data folder, allowing lazy access to all strings files within, both loose and inside BSA files.  Internal StringLookupOverlays will be created and cached when required to serve a query.  Loose strings files for a query will have priority, followed by the first applicable strings files found in the BSAs.  All operations on `StringsFolderLookupOverlay` are threadsafe.

``` { .cs hl_lines=6-7 }
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

### Writing a Strings File
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
