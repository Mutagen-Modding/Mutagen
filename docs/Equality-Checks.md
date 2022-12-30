<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [Basic Equality](#basic-equality)
- [Equals Mask](#equals-mask)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

# Basic Equality
Mutagen generates Equals and Hash functions for all classes based on record content.  Normal C# equality checks can be used:
```cs
bool isEqual = npc1.Equals(npc2);
bool isEqual2 = object.Equals(npc1, npc2);
int hash = npc1.GetHashCode();
```

# Equals Mask
Mutagen generates additional helper classes called "Masks".  These can be used to help specify what fields a user wants to check for equality, or hash with.
```cs
// Create a mask that will only check EditorID and Name
NPC.Mask<bool> mask = new NPC.Mask(initialValue: false)
{
    EditorID = true,
    Name = true,
}

bool isEqual = npc1.Equals(npc2, mask);
int hash = npc1.GetHashCode(mask);
```

A user can also check equality and get a Mask back as the return value:
```cs
NPC.Mask<bool> equalsMask = npc1.GetEqualsMask(npc2);
bool isEditorIdEqual = equalsMask.EditorID;
bool isNameEqual = equalsMask.Name;
bool allFieldsEqual = equalsMask.AllEqual(isEqualBool => isEqualBool);
```