# Most Subrecords are Optional
Most Bethesda subrecords are optional; They can be set to a value, or not exist at all.  Mutagen uses a field's nullability to indicate which fields/subrecords are optional.   This leverages one of C#'s newer concepts of [Nullable References](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references).

**TLDR: Fields can be marked as to whether they are allowed to be null or not**:
```cs
string RequiredField { get; set; }
string? OptionalField { get; set; }
```

The `RequiredField` cannot be `null`, and always must have some value.  The `OptionalField` is allowed to be `null`.  Mutagen utilizes this distinction to communicate that `OptionalField` is considered "unset" if/when it is `null`.

Going off the earlier `Potion` interface:
- FormKey cannot be null, and must always have a value.
- EditorID, Name, Model, Icon are all optional, and will be null if they are not set.

## Dealing with Nullable Fields
In recent C#, the compiler will error out if you're trying to access or use a value that might be null without ensuring it is not null first.  When a field is nullable, you will want to adjust your code to deal with the potential that it might not exist.

### Don't Check Unless Needed
Not all fields are nullable.  If a field's type does not end with a `?` then it cannot be null and thus does not need to be checked.

Checking if a non-nullable field is null is unnecessary, and actually confuses the compiler in certain circumstances.  Typically the compiler will let you know when something hasn't been checked by giving you an error.  You can usually wait until it complains before worrying about writing null checks.  

### Skipping if null
If you're within a foreach loop you can opt to skip the record if the field you're interested in is null:
```cs
IObjectEffect objectEffect = ...;
foreach (var effect in objectEffect.Effects)
{
    if (effect.Data == null) continue;
    Console.WriteLine($"Magnitude was: {effect.Data.Magnitude}");
}
```
### Using a fallback value if null
You can also use [Null Conditional Operators](https://www.dotnetperls.com/null-coalescing) to help specify "fallback" values if a field is null
```cs
IObjectEffect objectEffect = ...;
foreach (var effect in objectEffect.Effects)
{
    // This will be set to -1 if Data is null
    var magnitude = effect.Data?.Magnitude ?? -1;
    Console.WriteLine($"Magnitude was: {magnitude}");
}
```
### Setting the null field to have a value
Sometimes when you're creating a record, you might need to create the subclasses yourself on the nullable fields so that you can fill them.
```cs
IObjectEffect objectEffect = ...;
foreach (var effect in objectEffect.Effects)
{
    // A fancy way of setting the field to a new EffectData if it's null
    effect.Data ??= new EffectData();

    // And now we can set its Magnitude knowing it will never be null
    effect.Data.Magnitude = 100;
}
```

### Other
There are many other ways to deal with the potential of null values besides the examples given, depending on the circumstances and goals.
