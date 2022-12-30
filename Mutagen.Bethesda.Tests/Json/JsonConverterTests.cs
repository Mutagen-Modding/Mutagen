using FluentAssertions;
using Mutagen.Bethesda.Json;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Mutagen.Bethesda.Tests.Json;

public class JsonConverterTests
{
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
            .Should().Be($"{{\"Interface\":\"{toSerialize.Direct.FormKey}<Skyrim.Book>\",\"Direct\":\"{toSerialize.Direct.FormKey}<Skyrim.Book>\"}}");
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
            .Should().Be(target.Direct);
    }
    
    #endregion
}