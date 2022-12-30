<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->



<!-- END doctoc generated TOC please keep comment here to allow auto update -->

Mutagen provides C# classes, as well as matching interfaces for all records it supports.

Here's an example of what an interface for the `Potion` record might look like:
```cs
public interface IPotion : IMajorRecord, IPotionGetter
{
    // Major Record Fields
    FormKey FormKey { get; }
    string? EditorID { get; set; }

    // Spell Fields
    string? Name { get; set; }
    Model? Model { get; set; }
    string? Icon { get; set; }
    IFormIDSetLink<Script> Script { get; }
    
    ...
}
```

And its corresponding Getter interface:
```cs
public interface IPotionGetter : IMajorRecordGetter
{
    // Major Record Fields
    FormKey FormKey { get; }
    string? EditorID { get; }

    // Spell Fields
    string? Name { get; }
    Model? Model { get; }
    string? Icon { get; }
    IFormIDSetLinkGetter<Script> Script { get; }
    
    ...
}
```

These two interfaces that are generated for each record are pivotal touchstones of the entire library.  All functionality exposed will typically deal in one of these two types of interfaces, depending on whether the API requires/desires readonly functionality.

Interfaces need implementing classes, so of course Mutagen offers straight classes for use as well:
```cs
public class Potion : MajorRecord, IPotion
{
    // Major Record Fields
    FormKey FormKey { get; }
    string? EditorID { get; set; }

    // Spell Fields
    string? Name { get; set; }
    Model? Model { get; set; }
    string? Icon { get; set; }
    IFormIDSetLink<Script> Script { get; }
    
    ...

    public Potion(FormKey formKey)
        : base(formKey)
    {
    }

    ...
}
```

Of course, classes are also generated for the concepts of Mods themselves (`OblivionMod`, `SkyrimMod`), as well as subrecords (`Model`), Record Groups, etc.