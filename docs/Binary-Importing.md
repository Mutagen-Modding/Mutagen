All Mods are generated with the ability to create themselves from their binary format.
## Basic Importing
```cs
string path = "C:/Steam/blah/blah/Oblivion.esm";
OblivionMod mod = OblivionMod.CreateFromBinary(path);
```
At this point, the entire mod will have been loaded into memory via classes created to store all the various fields and records.   You can then begin to interact with the mod object.  

## Group Masks
Normally, users are not interested in ALL records that a mod contains.  Group Masks are an optional parameter that allows you to specify which record types you are interested in importing.
```cs
string path = "C:/Steam/blah/blah/Oblivion.esm";
var mod = OblivionMod.CreateFromBinary(
    path,
    importMask: new GroupMask()
    {
        Potions = true,
        NPCs = true,
    }); 
```
This import call will only process and import Potions and NPCs.

## Notes
While these basic import features sounds fundamental, they are overshadowed and depreciated somewhat by the [[Binary Overlay]] concepts.