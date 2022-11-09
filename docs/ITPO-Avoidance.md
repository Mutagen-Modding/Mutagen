# What is an ITPO
A very typical thing that can happen during processing mods is exporting a record that doesn't have any changes compared to the original.  This is known as an ITPO (Identical to Previous Override), or sometimes ITM (Idential to Master).

Mutagen may or may not get ITPO removal tooling in the future.  This would be powered by Equality/HashCode concepts, which already exist, but are not heavily tested for accuracy yet.  If/when ITPO removal calls do get implemented, it will mostly be for feature completeness, rather than necessity.

How does one avoid/deal with ITPOs in the meantime, then?

# Avoiding ITPOs

Typically, the best patterns avoid making ITPOs in the first place, rather than cleaning them up after they exist.

For example, this code can make some ITPOs:
```cs
foreach (var weapon in loadOrder.PriorityOrder.Weapon().WinningOverrides())
{
   var weaponOverride = outgoingMod.Weapons.GetOrAddAsOverride(weapon);
   weaponOverride.Weight = 0;
}
```
If a weapon already has a weight of zero, this will cause an ITPO.  We could add some code after to clean them up by looping over our outgoing mod, finding the previous override, checking equality, and then removing them from the outgoing patch if they're identical.

Alternatively, we can adjust the logic to just never make an ITPO in the first place:
```cs
foreach (var weapon in loadOrder.PriorityOrder.Weapon().WinningOverrides())
{
   // If what we wanted to change is already as we want it, skip
   if (weaponOverride.Weight == 0) continue;

   var weaponOverride = outgoingMod.Weapons.GetOrAddAsOverride(weapon);
   weaponOverride.Weight = 0;
}
```
As you can see, the call to `GetOrAddAsOverride` is delayed until we know there's a meaningful modification to do.

Another common pattern for doing this looks like this:
```cs
foreach (var weapon in loadOrder.PriorityOrder.Weapon().WinningOverrides())
{
   // Make a mutable copy, but don't add it to the mod yet
   var weaponOverride = weapon.DeepCopy();
   
   bool madeModification = false;

   if (weaponOverride.Weight != 0)
   {
       weaponOverride.Weight = 0;
       madeModification = true;
   }

   if (weaponOverride.Value != 100)
   {
       weaponOverride.Weight = 100;
       madeModification = true;
   }

   // If we didn't do anything, skip
   if (!madeModification) continue;

   outgoingMod.Weapons.Set(weaponOverride);
}
```

There are many ways to achieve the same goal.  The important takeaway is that code should first try to not make any ITPOs in the first place by only adding it to the outgoing mod once/if modifications have been made to the record.  Depending on the complexity of what you're doing, it may require different patterns than the ones outlined above.  No matter what though, if you find yourself reaching for ITPO tooling, take a step back and see if you can refactor it to process a different way to avoid them altogether.