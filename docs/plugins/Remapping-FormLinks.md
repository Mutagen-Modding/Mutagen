---
order: 255
---
# Remapping FormLinks

When working with duplicated records, overridden records, or reorganizing your mod, you may need to update FormLinks to point to different records. Mutagen provides several tools for remapping FormLinks.

## Single Link Updates

For updating individual FormLinks, use the `SetTo()` method:

```csharp
var duplicatedNpc = sourceNpc.Duplicate(myMod.GetNextFormKey());

// Update a specific FormLink to point to the new record
someOtherRecord.NpcLink.SetTo(duplicatedNpc);
```

This is useful when you know exactly which links need to be updated.

## Batch Remapping

For updating multiple FormLinks across many records, use the `RemapLinks()` method:

```csharp
var oldFormKey = sourceNpc.FormKey;
var duplicatedNpc = sourceNpc.Duplicate(myMod.GetNextFormKey());

// Create a mapping of old FormKeys to new FormKeys
var mapping = new Dictionary<FormKey, FormKey>
{
    { oldFormKey, duplicatedNpc.FormKey }
};

// Remap all FormLinks in the mod that match the mapping
myMod.RemapLinks(mapping);
```

The `RemapLinks()` method will:

- Traverse all records in the mod
- Find all FormLinks that reference any of the old FormKeys in the mapping
- Update those FormLinks to point to the corresponding new FormKeys
