The Global and Gamesetting records contain many different types of data while each having their own unique rules of communicating what type of data they contain.  For example, `Global` records have a special subrecord `FNAM` with a single char `i`, `f`, `s` to symbolize the float field it contains should be interpreted as an `int`, `float`, or `short`.  `GameSetting` on the other hand prepends a character to its EditorID to communicate what type of data is stored in its `DATA` subrecord.

This complexity is abstracted away by Mutagen by offering strongly typed subclasses for Globals and Gamesettings.  `GlobalInt`, `GlobalFloat`, `GameSettingInt`, `GameSettingString`, `GameSettingBool`, etc.  These subclasses expose a strongly typed member of the correct type to the user while internally handling the most of the details.

[:octicons-arrow-right-24: Abstract Subclassing](../Abstract-Subclassing.md)

## Construction

=== "Constructor"
    ```cs
    mod.Globals.Add(
        new GlobalInt(mod.GetNextFormKey())
        {
            EditorID = "MyIntGlobal",
            Data = 1234
        });

    mod.GameSettings.Add(
        new GameSettingFloat(mod.GetNextFormKey())
        {
            EditorID = "MyFloatGameSetting",
            Data = 1234.5f
        });
    ```
=== "AddNew"
    ``` cs
    ISkyrimMod mod = ...;

    var global = mod.Globals.AddNewInt("MyIntGlobal");
    global.Data = 1234;

    var gameSetting = mod.GameSettings.AddNewFloat("MyFloatGameSetting");
    global.Data = 1234f;
    ```

!!! info "Editor ID Processed Automatically"
    For GameSettings, the EditorID stores the type data as its first character.  Any EditorID you set will automatically be processed to have the correct starting character.  As such, all GameSettings should have their EditorIDs set.

## Reading
Reading Globals/GameSettings consists of checking/casting the records to their correct subtype as per [Abstract Subclass](../Abstract-Subclassing.md) patterns.  One easy way to do this is to use a switch:
```cs
foreach (GameSetting setting in mod.GameSettings.Records)
{
   switch (setting)
   {
       case GameSettingString stringSetting:
           System.Console.WriteLine($"Found a string setting: {stringSetting.EditorID}");
           System.Console.WriteLine($"   Value: {stringSetting.Data}");
           break;
       case GameSettingBool boolSetting:
           System.Console.WriteLine($"Found a bool setting: {stringSetting.EditorID}");
           System.Console.WriteLine($"   Value: {(stringSetting.Data ? "ON!" : "OFF!")}");
           break;
   }
}
```
