using Shouldly;
using Mutagen.Bethesda.Json;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Mutagen.Bethesda.Tests.Json;

public class JsonConverterTests
{
    #region FormLink with known generic type

    class WeaponOverridesContainer
    {
        public Dictionary<string, List<IFormLinkGetter<IWeaponGetter>>> WeaponOverrides { get; set; } = new();
    }

    [Fact]
    public void FormLinkConverter_WithKnownGenericType_DeserializesWithoutTypeAnnotation()
    {
        Warmup.Init();
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());

        // This is the format the user is providing - no type annotation, just FormKey
        var json = """
        {
          "WeaponOverrides": {
            "WeaponsSignalGrenades": [
              "12E2CA:Skyrim.esm",
              "065DEC:Skyrim.esm"
            ]
          }
        }
        """;

        var result = JsonConvert.DeserializeObject<WeaponOverridesContainer>(json, settings);

        result.ShouldNotBeNull();
        result.WeaponOverrides.ShouldContainKey("WeaponsSignalGrenades");
        result.WeaponOverrides["WeaponsSignalGrenades"].Count.ShouldBe(2);
        result.WeaponOverrides["WeaponsSignalGrenades"][0].FormKey.ToString().ShouldBe("12E2CA:Skyrim.esm");
    }

    [Fact]
    public void FormLinkConverter_WithKnownGenericType_StillWorksWithTypeAnnotation()
    {
        Warmup.Init();
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());

        // Type annotation is still supported for backward compatibility
        var json = """
        {
          "WeaponOverrides": {
            "WeaponsSignalGrenades": [
              "12E2CA:Skyrim.esm<Skyrim.Weapon>"
            ]
          }
        }
        """;

        var result = JsonConvert.DeserializeObject<WeaponOverridesContainer>(json, settings);

        result.ShouldNotBeNull();
        result.WeaponOverrides["WeaponsSignalGrenades"].Count.ShouldBe(1);
        result.WeaponOverrides["WeaponsSignalGrenades"][0].FormKey.ToString().ShouldBe("12E2CA:Skyrim.esm");
    }

    #endregion

    #region FormLinkInformation

    class FormLinkInformationClass
    {
        public IFormLinkGetter Interface { get; set; } = null!;
        public FormLinkInformation Direct { get; set; } = null!;
    }

    [Fact]
    public void FormLinkInformationConverter_FormLink_Serialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toSerialize = new FormLinkInformationClass()
        {
            Direct = new FormLinkInformation(TestConstants.Form2, typeof(IBookGetter)),
            Interface = new FormLinkInformation(TestConstants.Form2, typeof(IBookGetter)),
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .ShouldBe($"{{\"Interface\":\"{toSerialize.Direct.FormKey}<Skyrim.Book>\",\"Direct\":\"{toSerialize.Direct.FormKey}<Skyrim.Book>\"}}");
    }

    [Fact]
    public void FormLinkInformationConverter_FormLink_Deserialize()
    {
        Warmup.Init();
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkInformationClass()
        {
            Direct = new FormLinkInformation(TestConstants.Form2, typeof(IBookGetter)),
            Interface = new FormLinkInformation(TestConstants.Form2, typeof(IBookGetter)),
        };
        var toDeserialize = $"{{\"Interface\":\"{target.Direct.FormKey}<Skyrim.Book>\",\"Direct\":\"{target.Direct.FormKey}<Skyrim.Book>\"}}";
        JsonConvert.DeserializeObject<FormLinkInformationClass>(toDeserialize, settings)!
            .Direct
            .ShouldBe(target.Direct);
    }
    
    #endregion
}