FormLinks are used widely as a strongly typed identifier of a record, as an alternative to FormID, EditorID, or even FormKey.

When using them, though, there are two variants:
- `FormLink`
- `FormLinkNullable`

# Which You Should Use
Generally, the answer is you should always use `FormLink`, rather than `FormLinkNullable`.

Mutagen exposes `FormLinkNullable` in certain areas, but you yourself should rarely if ever decide to create a `FormLinkNullable` when writing your own code.

# What is FormLinkNullable
If you shouldn't use it, what is `FormLinkNullable` for?

It is used by Mutagen itself to expose a very specific difference in how FormIDs can be null within a binary file on the disk.

A FormLinkNullable can be null in two ways:

An example of this is in Skyrim Npc's Worn Armor.   It is a `FormID` in the subrecord `WNAM`, which points to an Armor an Npc wears.
Consider how this Worn Armor can be null:
- `WNAM`'s value can be 0
- WNAM can be missing entirely

And how a `FormLinkNullable`'s `FormKey` will be exposed in those differing scenarios:
- When `WNAM`s value is 0, `FormKey` will not be null, and will contain a zero ID
- When `WNAM` is missing entirely, `FormKey` member will be null

This difference lets you detect the various ways a FormID can be null on disk.  This subtle difference is not normally important, which is why FormLink itself has an `IsNull` member, which checks for both cases, and is preferable way to check for whether a FormID is null.

For your own code, you should -not- use `FormLinkNullable`, as the above differences are not applicable for most program logic that is not interacting with an on-disk representation directly.
