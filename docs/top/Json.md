# Json
Currently, the libraries that Mutagen offers related to Json targets `Newtonsoft.Json`.  Alternatives can be added for `System.Text.Json` if there's demand.

## Json Converters
There are some built in converters to allow for FormKeys/FormLinks/ModKeys to be included in a Json DTO.

```cs
public class MyDto
{
    public FormKey MyFormKey { get; set; }
    public IFormLinkGetter<IArmorGetter> MyArmorLink { get; set; } = FormLink<IArmorGetter>.Null;
    public IFormLinkIdentifier MyRuntimeLink { get; set; } = FormLink<IMajorRecordGetter>.Null;
    public ModKey MyModKey { get; set; }
}

// ..

var settings = new JsonSerializerSettings();
// Convenience function to add all converters
settings.AddMutagenConverters();

var someDto = new MyDto()
{
    MyFormKey = FormKey.Factory("123456:MyMod.esp"),
    MyArmorLink = FormKey.Factory("000123:MyMod.esp").ToLink<IArmorGetter>(),
    MyRuntimeLink = new FormLinkInformation(FormKey.Factory("000456:MyMod.esp"), typeof(IWeaponGetter)),
    MyModKey = "MyMod.esp",
};

var str = JsonConvert.SerializeObject(someDto);
Console.WriteLine(str);
```
Prints
```
{
  "MyFormKey": "123456:MyMod.esp",
  "MyArmorLink": "000123:MyMod.esp",
  "MyRuntimeLink": "000456:MyMod.esp<Skyrim.Weapon>",
  "MyModKey ": "MyMod.esp"
}
```
