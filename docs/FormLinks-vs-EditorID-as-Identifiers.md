# EditorID vs FormLink
When choosing how you want to look up records, or store lists of record identifiers, there's the common choice between EditorIDs and FormLinks. 

For example, let's say you want to look up Skyrim's `ArmorImperialCuriass` `0136D5:Skyrim.esm`.

You can either do it via EditorID:
```cs
// EditorIDs are strings
string editorId = "ArmorImperialCuriass";
// Look up via EditorID in the cache
var foundArmorRecord = myLinkCache.Resolve<IArmorGetter>(editorId);
```
or by FormLink:
```cs
// FormLinks contain type and a FormID
IFormLinkGetter<IArmorGetter> link = FormKey.Factory("0136D5:Skyrim.esm");
// Can alternatively use the FormKeys library to avoid typing the ID by hand
link = Skyrim.Armor.ArmorImperialCuirass;
// Look up the FormLink against the cache
var foundArmorRecord = link.Resolve(myLinkCache);
```

Similarly, you can store containers of identifiers via EditorID:
```cs
var armors = new HashSet<string>();
armors.Add("ArmorImperialCuriass");
```
or by FormLink:
```cs
var armors = new HashSet<IFormLinkGetter<IArmorGetter>>();
armors.Add(Skyrim.Armor.ArmorImperialCuirass);
```

What are the reasons for choosing one over the other?  **Generally FormLinks are the recommended route.**  This article will go over the differences and reasons why.

# EditorID Upsides
## Readability and FormLink Mapping
### EditorIDs are More Readable
We're going to start out with the one upside that EditorIDs have: readability.

`ArmorImperialCuriass` is much more human readable and understandable compared to `0136D5:Skyrim.esm`.

Generally, this is the reason people initially gravitate towards EditorIDs, as lots of other tooling uses EditorIDs as they are more human friendly.

### FormLink Mapping Brings Readability to FormLink-Based Systems
Work has been done to mitigate this readability downside of FormLinks.  [FormLink Mapping](https://github.com/Mutagen-Modding/Mutagen/wiki/Reference-FormLinks-By-EditorID) is a system where you can point to a mod and generate mappings so that you can reference and use FormLinks by their EditorID name.

This allows for the best of both worlds, where the code is human readable by writing EditorIDs, while still using FormLinks under the hood for the code to use.  These systems mostly nullify the readability problem that FormKeys have.

```cs
// Left hand side is a FormLink, but right hand side is written and readable like an EditorID
IFormLinkGetter<IArmorGetter> myFormLink = Skyrim.Armor.ArmorImperialCuriass;
```

# EditorID Downsides
As far as code is concerned, it is much more preferable to deal with FormLinks.  Here are some reasons EditorIDs can cause some problems.
## Overriding Mods Can Ruin Lookups
Let's say you're looking for the Armor in question `ArmorImperialCuirass`.  You can look it up by EditorID:
```cs
// Looking up to find a winning Armor override with EditorID 
var foundArmorRecord = linkCache.Resolve<IArmorGetter>("ArmorImperialCuirass");
```
or check if it's in a set:
```cs
myEditorIdSet.Add("ArmorImperialCuirass");
// Checking to see if a given record is within the set, by EditorID
myEditorIdSet.Contains(potentialMatchingArmorRecord.EditorID);
```

Both of these operations are fragile.  You're looking up a record by its EditorID, which is a field that mods are able to override and change.  It's not often recommended for mods to do so, but they do it.  So if ModA.esp decided to change `ArmorImperialCuriass` to `ImperialCuriass`, for whatever reason, then your check will fail to find the record.

EditorID is mutable and inconsistent.  In contrast, a FormLink/FormKey/FormID is a constant immutable aspect of a record, and so lookups via FormLink will never fail, no matter what mods change what values:
```cs
// Looking up to find a winning Armor override with the given FormID
var foundArmorRecord = myArmorLink.Resolve(linkCache);

myFormLinkSet.Add(Skyrim.Armor.ArmorImperialCuirass);
// Checking to see if a given record is within the set, by FormID
myFormLinkSet.Contains(potentialMatchingArmorRecord);
```
These will consistently connect with the record you're looking for no matter what mods have done to them.

## Type Info Is Lost, Potentially Losing Speed
Mutagen gets a lot of its speed by short circuiting work that is unnecessary.  One of the ways it does this is by making use of the Type of record involved.  For example, if you tell the LinkCache it is an Armor you're looking for, then it can skip parsing 99% of `Skyrim.esm`.

EditorIDs are just strings, and so have no type information.  So when looking up a record via EditorID, you ideally want to specify the type involved:
```cs
// Specifying IArmorGetter speeds up the lookup
var foundArmorRecord = linkCache.Resolve<IArmorGetter>("ArmorImperialCuirass");

// Forgetting to specify type leads to parsing the entire mod's contents:
var foundArmorRecord = linkCache.Resolve("ArmorImperialCuirass");
```

FormLinks have type info built into their structure.  They are an ID + a Type.  As such, you do not need to remember to specify the type:
```cs
// No need to specify type, as the FormLink we're Resolving knows its type: Armor
var foundArmorRecord = Skyrim.Armor.ArmorImperialCuirass.Resolve(linkCache);
```

## Requires Decompression, Losing Speed
This is due to a gritty implementation detail of how records are stored on disk.  A record's header is never compressed, while all the contents can be.  An EditorID is content that gets compressed.

FormIDs are always in the same known location in the record header and never compressed.   So, when looking up records, a FormLink can simply look at a record to see if its a match at near instant speed.

EditorID lookups, on the other hand, require the entire record's contents to be decompressed before it can begin looking for the EDID record to parse the string needed to see if the record is a match.

So, by using EditorIDs as your lookup identifier, you're implicitly losing a good deal of speed by requiring the systems to do a bunch of potentially unnecessary work parsing a record you're not even interested in.

## Some Records Cannot Be Looked Up
EditorIDs are not actually required.  Most of the common records people are interested in have them, but there are plenty of records that do not. 

This is more applicable for set building.  If you're compiling a `HashSet<string>` of EditorIDs of all records that satisfy XYZ, then some records might not be able to be put into the set. 

FormIDs are fundamental required data for a record to exist, and so are ensured to always be viable.

## Specialized UI Input Systems
The Mutagen ecosystem comes with specialized UI record pickers:

![FormKey Picker](https://i.imgur.com/gtlg5Md.gif)

These allow users to search for records easily, and when a record is chosen, the FormLink in question is passed to the code.

The system is not currently set up to bind to EditorIDs in the code backend.  As such, you'd likely have to instead use a list of raw strings.

Theoretically the work could be done to expose it in a similar UI, but it has not been done because EditorIDs are generally less preferable to use for all the other reasons mentioned.

## Typos
### You Can Make Typos
EditorIDs are just raw strings, and so are vulnerable to typos:
```cs
// Will never resolve, as we've misspelled Imperial
// Compiler cannot give you a heads up, as it's just a raw string
var foundArmorRecord = linkCache.Resolve<IArmorGetter>("ArmorImprialCuirass");
```

FormLinks would be vulnerable as well when typing in their ID, except when you're using the FormLink map systems:
![](https://i.imgur.com/054RXKp.png)

As a bonus, the IDE intellisense can suggest EditorIDs for you and autocomplete:

![](https://i.imgur.com/fH7YSEa.gif)

### Your Users Can Make Typos
Users will also have to deal with typos.  With EditorID based input, you're typically going to be exposing these via a list of strings on a UI or in a json file.  As they type in their desired records, they will be very vulnerable to making typos.  It's highly likely they don't realize and might complain to you the developer that your program is broken.  

# String Comparisons Are Slower than Int Comparisons
I put this last, as it's a factor, but is minor compared to the other issues.  Comparing a long string of an EditorID like "ArmorImprialCuirass" is slower than comparing FormLinks which use integers for comparison.   As such, there will be a slight speed penalty.  Nothing to worry about too much, but just another small negative to add to the pile.