# Equality Checks

!!! bug "Work in Progress"
    Equality functionality exists, but has not been thoroughly tested.  Bugs within may exist.  Please report if you see any.

## Basic Equality
Mutagen generates Equals and Hash functions for all classes based on record content.  Normal C# equality checks can be used:
```cs
bool isEqual = npc1.Equals(npc2);
bool isEqual2 = object.Equals(npc1, npc2);
int hash = npc1.GetHashCode();
```

## Equals Mask
Mutagen generates additional helper classes called Translation Masks.  These can be used to help specify what fields a user wants to check for equality, or hash with.

[:octicons-arrow-right-24: Translation Mask](plugins/Translation-Masks.md)

```cs
// Create a mask that will only check EditorID and Name
Npc.TranslationMask mask = new Npc.TranslationMask(defaultOn: false)
{
    EditorID = true,
    Name = true,
};

bool isEqual = npc1.Equals(npc2, mask);
int hash = npc1.GetHashCode(mask);
```

A user can also check equality and get a Mask back as the return value:
```cs
var equalsMask = npc1.GetEqualsMask(npc2);
bool isEditorIdEqual = equalsMask.EditorID;
bool isNameEqual = equalsMask.Name;
bool allFieldsEqual = equalsMask.AllEqual(isEqualBool => isEqualBool);
```
