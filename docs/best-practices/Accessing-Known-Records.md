# Accessing Known Records
## Recommended Patterns
Often, especially for base master files like `Skyrim.esm`, there are specific records that you want to look up.

The recommended strategy is to use the [FormKeys library](https://github.com/Mutagen-Modding/Mutagen.Bethesda.FormKeys), which lets you refer to records by EditorID, while still using FormKeys under the hood:
```
var env = ...; // Some environment, like Synthesis, or Mutagen's GameEnvironment

if (Skyrim.Race.ArgonianRace.TryResolve(env.LinkCache, out var race))
{
    Console.WriteLine($"Found the race record");
}
```

The following sections will outline the alternatives and reasoning for the recommended best practices.

## Desire To Access Known Records

### By FormKey
For example, if you wanted to look up the Argonian Race record, you might do the following:
```cs
var env = ...; // Some environment, like Synthesis, or Mutagen's GameEnvironment

var formKey = FormKey.Factory("123456:Skyrim.esm");

if (env.LinkCache.TryResolve<IRaceGetter>(formKey, out var race))
{
    Console.WriteLine($"Found the race record");
}
```

First, a FormKey is created pointing to the known record for the Argonian race in Skyrim.esm.   Then the link cache is asked to resolve the winning override for the record. 
This will retrieve the winning override.

However, there's a few annoyances:

- Neither the dev nor future readers know the FormID for records offhand, and so must always look them up.
- The only indication that `123456` points to the argonian race is to look it up and check, or hope the variable is named something intelligent (like `argonianRaceFormKey`)
- Potential for typos.  What if it was actually `123457` and got mis-copied?

### EditorID
EditorIDs are a common alternative for looking up records, as they are human readable.

```cs
var env = ...; // Some environment, like Synthesis, or Mutagen's GameEnvironment

if (env.LinkCache.TryResolve<IRaceGetter>("ArgonianRace", out var race))
{
    Console.WriteLine($"Found the race record");
}
```

However, they are not recommended for general use, for the reasons [outlined here](FormLinks-vs-EditorID-as-Identifiers.md).

## Neither is Ideal
Neither direct FormKeys or EditorIDs are ideal for looking up known records.  This is why the recommended pattern is to use the [FormLinks library](https://github.com/Mutagen-Modding/Mutagen.Bethesda.FormKeys) to bridge the gap and get the best of both worlds
