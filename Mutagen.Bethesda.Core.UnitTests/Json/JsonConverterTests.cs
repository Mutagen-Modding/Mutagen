using FluentAssertions;
using Mutagen.Bethesda.Json;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Placeholders;
using Newtonsoft.Json;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Json;

public class JsonConverterTests
{
    #region FormKey
    class FormKeyClass
    {
        public FormKey Member { get; set; } = TestConstants.Form1;
    }

    [Fact]
    public void FormKeyConverter_FormKey_Serialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toSerialize = new FormKeyClass()
        {
            Member = TestConstants.Form2
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":\"{toSerialize.Member}\"}}");
    }

    [Fact]
    public void FormKeyConverter_FormKey_Deserialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormKeyClass()
        {
            Member = TestConstants.Form2
        };
        var toDeserialize = $"{{\"Member\":\"{target.Member}\"}}";
        JsonConvert.DeserializeObject<FormKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void FormKeyConverter_FormKey_Deserialize_Missing()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormKeyClass();
        var toDeserialize = $"{{}}";
        JsonConvert.DeserializeObject<FormKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void FormKeyConverter_FormKey_Deserialize_Empty()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormKeyClass()
        {
            Member = FormKey.Null
        };
        var toDeserialize = $"{{\"Member\":\"\"}}";
        JsonConvert.DeserializeObject<FormKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    class NullableFormKeyClass
    {
        public FormKey? Member { get; set; } = TestConstants.Form1;
    }

    [Fact]
    public void FormKeyConverter_NullableFormKey_Serialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toSerialize = new NullableFormKeyClass()
        {
            Member = TestConstants.Form2
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":\"{toSerialize.Member}\"}}");
    }

    [Fact]
    public void FormKeyConverter_NullableFormKey_Serialize_Null()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toSerialize = new NullableFormKeyClass()
        {
            Member = null
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":null}}");
    }

    [Fact]
    public void FormKeyConverter_NullableFormKey_Deserialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new NullableFormKeyClass()
        {
            Member = TestConstants.Form2
        };
        var toDeserialize = $"{{\"Member\":\"{target.Member}\"}}";
        JsonConvert.DeserializeObject<NullableFormKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void FormKeyConverter_NullableFormKey_Deserialize_Missing()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toDeserialize = $"{{}}";
        var target = new NullableFormKeyClass();
        JsonConvert.DeserializeObject<NullableFormKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void FormKeyConverter_NullableFormKey_Deserialize_Null()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toDeserialize = $"{{\"Member\":null}}";
        JsonConvert.DeserializeObject<NullableFormKeyClass>(toDeserialize, settings)!
            .Member
            .Should().BeNull();
    }

    [Fact]
    public void FormKeyConverter_NullableFormKey_Deserialize_Empty()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toDeserialize = $"{{\"Member\":\"\"}}";
        JsonConvert.DeserializeObject<NullableFormKeyClass>(toDeserialize, settings)!
            .Member
            .Should().BeNull();
    }
    #endregion

    #region FormLink
    class FormLinkClass
    {
        public FormLink<ITestMajorRecordGetter> Direct { get; set; } = new(TestConstants.Form1);
        public IFormLink<ITestMajorRecordGetter> Setter { get; set; } = new FormLink<ITestMajorRecordGetter>(TestConstants.Form1);
        public IFormLinkGetter<ITestMajorRecordGetter> Getter { get; set; } = new FormLink<ITestMajorRecordGetter>(TestConstants.Form1);
    }

    [Fact]
    public void FormKeyConverter_FormLink_Serialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toSerialize = new FormLinkClass()
        {
            Direct = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2),
            Setter = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2),
            Getter = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2)
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Direct\":\"{toSerialize.Direct.FormKey}\",\"Setter\":\"{toSerialize.Direct.FormKey}\",\"Getter\":\"{toSerialize.Direct.FormKey}\"}}");
    }

    [Fact]
    public void FormKeyConverter_FormLink_Deserialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkClass()
        {
            Direct = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2),
            Setter = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2),
            Getter = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2)
        };
        var toDeserialize = $"{{\"Direct\":\"{target.Direct.FormKey}\",\"Setter\":\"{target.Direct.FormKey}\",\"Getter\":\"{target.Direct.FormKey}\"}}";
        JsonConvert.DeserializeObject<FormLinkClass>(toDeserialize, settings)!
            .Direct
            .Should().Be(target.Direct);
    }

    [Fact]
    public void FormKeyConverter_FormLink_Deserialize_Missing()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkClass();
        var toDeserialize = $"{{}}";
        JsonConvert.DeserializeObject<FormLinkClass>(toDeserialize, settings)!
            .Direct
            .Should().Be(target.Direct);
    }

    [Fact]
    public void FormKeyConverter_FormLink_Deserialize_Null()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkClass()
        {
            Direct = new FormLink<ITestMajorRecordGetter>(FormKey.Null),
            Setter = new FormLink<ITestMajorRecordGetter>(FormKey.Null),
            Getter = new FormLink<ITestMajorRecordGetter>(FormKey.Null)
        };
        var toDeserialize = $"{{\"Direct\":\"Null\",\"Setter\":\"Null\",\"Getter\":\"Null\"}}";
        JsonConvert.DeserializeObject<FormLinkClass>(toDeserialize, settings)!
            .Direct
            .Should().Be(target.Direct);
    }

    [Fact]
    public void FormKeyConverter_FormLink_Deserialize_Empty()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkClass()
        {
            Direct = new FormLink<ITestMajorRecordGetter>(FormKey.Null),
            Setter = new FormLink<ITestMajorRecordGetter>(FormKey.Null),
            Getter = new FormLink<ITestMajorRecordGetter>(FormKey.Null)
        };
        var toDeserialize = $"{{\"Direct\":\"\",\"Setter\":\"\",\"Getter\":\"\"}}";
        JsonConvert.DeserializeObject<FormLinkClass>(toDeserialize, settings)!
            .Direct
            .Should().Be(target.Direct);
    }

    // This doesn't quite make sense, as the json parser cannot know that it's nullable
    // Might not be able to support fully
    // class NullableFormLinkClass
    // {
    //     public FormLink<ITestMajorRecordGetter>? Member { get; set; } = new(TestConstants.Form1);
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLink_Serialize()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toSerialize = new NullableFormLinkClass()
    //     {
    //         Member = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2)
    //     };
    //     JsonConvert.SerializeObject(toSerialize, settings)
    //         .Should().Be($"{{\"Member\":\"{toSerialize.Member}\"}}");
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLink_Serialize_Null()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toSerialize = new NullableFormLinkClass()
    //     {
    //         Member = null
    //     };
    //     JsonConvert.SerializeObject(toSerialize, settings)
    //         .Should().Be($"{{\"Member\":null}}");
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLink_Serialize_NullFormKey()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toSerialize = new NullableFormLinkClass()
    //     {
    //         Member = new FormLink<ITestMajorRecordGetter>(FormKey.Null)
    //     };
    //     JsonConvert.SerializeObject(toSerialize, settings)
    //         .Should().Be($"{{\"Member\":\"Null\"}}");
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLink_Deserialize()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var target = new NullableFormLinkClass()
    //     {
    //         Member = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2)
    //     };
    //     var toDeserialize = $"{{\"Member\":\"{target.Member.FormKey}\"}}";
    //     JsonConvert.DeserializeObject<NullableFormLinkClass>(toDeserialize, settings)!
    //         .Member
    //         .Should().Be(target.Member);
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLink_Deserialize_Missing()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toDeserialize = $"{{}}";
    //     var target = new NullableFormLinkClass();
    //     JsonConvert.DeserializeObject<NullableFormLinkClass>(toDeserialize, settings)!
    //         .Member
    //         .Should().Be(target.Member);
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLink_Deserialize_Null()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toDeserialize = $"{{\"Member\":null}}";
    //     JsonConvert.DeserializeObject<NullableFormLinkClass>(toDeserialize, settings)!
    //         .Member
    //         .Should().BeNull();
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLink_Deserialize_NullFormKey()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toDeserialize = $"{{\"Member\":\"Null\"}}";
    //     JsonConvert.DeserializeObject<NullableFormLinkClass>(toDeserialize, settings)!
    //         .Member!.IsNull
    //         .Should().BeTrue();
    // }
    #endregion

    #region FormLinkNullable
    class FormLinkNullableClass
    {
        public FormLinkNullable<ITestMajorRecordGetter> Member { get; set; } = new(TestConstants.Form1);
    }

    [Fact]
    public void FormKeyConverter_FormLinkNullable_Serialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toSerialize = new FormLinkNullableClass()
        {
            Member = new FormLinkNullable<ITestMajorRecordGetter>(TestConstants.Form2)
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":\"{toSerialize.Member.FormKey}\"}}");
    }

    [Fact]
    public void FormKeyConverter_FormLinkNullable_Serialize_Null()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toSerialize = new FormLinkNullableClass()
        {
            Member = new FormLinkNullable<ITestMajorRecordGetter>(default(FormKey?))
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":null}}");
    }

    [Fact]
    public void FormKeyConverter_FormLinkNullable_Serialize_NullFormKey()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var toSerialize = new FormLinkNullableClass()
        {
            Member = new FormLinkNullable<ITestMajorRecordGetter>(FormKey.Null)
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":\"Null\"}}");
    }

    [Fact]
    public void FormKeyConverter_FormLinkNullable_Deserialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkNullableClass()
        {
            Member = new FormLinkNullable<ITestMajorRecordGetter>(TestConstants.Form2)
        };
        var toDeserialize = $"{{\"Member\":\"{target.Member.FormKey}\"}}";
        JsonConvert.DeserializeObject<FormLinkNullableClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void FormKeyConverter_FormLinkNullable_Deserialize_Missing()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkNullableClass();
        var toDeserialize = $"{{}}";
        JsonConvert.DeserializeObject<FormLinkNullableClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void FormKeyConverter_FormLinkNullable_Deserialize_Null()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkNullableClass()
        {
            Member = new FormLinkNullable<ITestMajorRecordGetter>(default(FormKey?))
        };
        var toDeserialize = $"{{\"Member\":null}}";
        JsonConvert.DeserializeObject<FormLinkNullableClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void FormKeyConverter_FormLinkNullable_Deserialize_Empty()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkNullableClass()
        {
            Member = new FormLinkNullable<ITestMajorRecordGetter>(default(FormKey?))
        };
        var toDeserialize = $"{{\"Member\":\"\"}}";
        JsonConvert.DeserializeObject<FormLinkNullableClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void FormKeyConverter_FormLinkNullable_Deserialize_FormKeyNull()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkNullableClass()
        {
            Member = new FormLinkNullable<ITestMajorRecordGetter>(FormKey.Null)
        };
        var toDeserialize = $"{{\"Member\":\"Null\"}}";
        JsonConvert.DeserializeObject<FormLinkNullableClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    // This doesn't quite make sense, as the json parser cannot know that it's nullable
    // Might not be able to support fully
    // class NullableFormLinkNullableClass
    // {
    //     public FormLinkNullable<INpcGetter>? Member { get; set; } = new(TestConstants.Form1);
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLinkNullable_Serialize()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toSerialize = new NullableFormLinkNullableClass()
    //     {
    //         Member = new FormLinkNullable<INpcGetter>(TestConstants.Form2)
    //     };
    //     JsonConvert.SerializeObject(toSerialize, settings)
    //         .Should().Be($"{{\"Member\":\"{toSerialize.Member}\"}}");
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLinkNullable_Serialize_Null()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toSerialize = new NullableFormLinkNullableClass()
    //     {
    //         Member = null
    //     };
    //     JsonConvert.SerializeObject(toSerialize, settings)
    //         .Should().Be($"{{\"Member\":null}}");
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLinkNullable_Deserialize()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var target = new NullableFormLinkNullableClass()
    //     {
    //         Member = new FormLinkNullable<INpcGetter>(TestConstants.Form2)
    //     };
    //     var toDeserialize = $"{{\"Member\":\"{target.Member}\"}}";
    //     JsonConvert.DeserializeObject<NullableFormLinkNullableClass>(toDeserialize, settings)!
    //         .Member
    //         .Should().Be(target.Member);
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLinkNullable_Deserialize_Missing()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toDeserialize = $"{{}}";
    //     var target = new NullableFormLinkNullableClass();
    //     JsonConvert.DeserializeObject<NullableFormLinkNullableClass>(toDeserialize, settings)!
    //         .Member
    //         .Should().Be(target.Member);
    // }
    //
    // [Fact]
    // public void FormKeyConverter_NullableFormLinkNullable_Deserialize_Null()
    // {
    //     var settings = new JsonSerializerSettings();
    //     settings.Converters.Add(new FormKeyJsonConverter());
    //     var toDeserialize = $"{{\"Member\":null}}";
    //     JsonConvert.DeserializeObject<NullableFormLinkNullableClass>(toDeserialize, settings)!
    //         .Member
    //         .Should().BeNull();
    // }
    #endregion

    #region FormLinkInformation
    class FormLinkInformationClass
    {
        public IFormLinkGetter Interface { get; set; } = new FormLinkInformation(TestConstants.Form1, typeof(ITestMajorRecordGetter));
        public FormLinkInformation Direct { get; set; } = new FormLinkInformation(TestConstants.Form1, typeof(ITestMajorRecordGetter));
    }
    
    [Fact]
    public void FormLinkInformationConverter_FormLink_Deserialize_Missing()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkInformationClass();
        var toDeserialize = $"{{}}";
        JsonConvert.DeserializeObject<FormLinkInformationClass>(toDeserialize, settings)!
            .Direct
            .Should().Be(target.Direct);
    }
    
    [Fact]
    public void FormLinkInformationConverter_FormLink_Deserialize_Null()
    {
        Warmup.Init();
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkInformationClass()
        {
            Interface = new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter)),
            Direct = new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter)),
        };
        var toDeserialize = $"{{\"Direct\":\"Null\",\"Interface\":\"Null\"}}";
        JsonConvert.DeserializeObject<FormLinkInformationClass>(toDeserialize, settings)!
            .Direct
            .Should().Be(target.Direct);
        JsonConvert.DeserializeObject<FormLinkInformationClass>(toDeserialize, settings)!
            .Interface
            .Should().Be(target.Interface);
    }
    
    [Fact]
    public void FormLinkInformationConverter_FormLink_Deserialize()
    {
        Warmup.Init();
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkInformationClass()
        {
            Interface = new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter)),
            Direct = new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter)),
        };
        var toDeserialize = $"{{\"Interface\":\"Null<Bethesda.MajorRecord>\",\"Direct\":\"Null<Bethesda.MajorRecord>\"}}";
        JsonConvert.DeserializeObject<FormLinkInformationClass>(toDeserialize, settings)!
            .Direct
            .Should().Be(target.Direct);
        JsonConvert.DeserializeObject<FormLinkInformationClass>(toDeserialize, settings)!
            .Interface
            .Should().Be(target.Interface);
    }
    
    [Fact]
    public void FormLinkInformationConverter_FormLink_Serialize_Null()
    {
        Warmup.Init();
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkInformationClass()
        {
            Interface = new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter)),
            Direct = new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter)),
        };
        var toDeserialize = $"{{\"Interface\":\"Null<MajorRecord>\",\"Direct\":\"Null<MajorRecord>\"}}";
        JsonConvert.SerializeObject(target, settings)
            .Should().Be(toDeserialize);
    }
    
    [Fact]
    public void FormLinkInformationConverter_FormLink_Serialize()
    {
        Warmup.Init();
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkInformationClass()
        {
            Interface = new FormLinkInformation(FormKey.Factory("123456:Skyrim.esm"), typeof(IMajorRecordGetter)),
            Direct = new FormLinkInformation(FormKey.Factory("123456:Skyrim.esm"), typeof(IMajorRecordGetter)),
        };
        var toDeserialize = $"{{\"Interface\":\"123456:Skyrim.esm<MajorRecord>\",\"Direct\":\"123456:Skyrim.esm<MajorRecord>\"}}";
        JsonConvert.SerializeObject(target, settings)
            .Should().Be(toDeserialize);
    }
    
    [Fact]
    public void FormLinkInformationConverter_FormLink_Deserialize_Empty()
    {
        Warmup.Init();
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new FormKeyJsonConverter());
        var target = new FormLinkInformationClass()
        {
            Interface = new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter)),
            Direct = new FormLinkInformation(FormKey.Null, typeof(IMajorRecordGetter)),
        };
        var toDeserialize = $"{{\"Interface\":\"\",\"Direct\":\"\"}}";
        JsonConvert.DeserializeObject<FormLinkInformationClass>(toDeserialize, settings)!
            .Direct
            .Should().Be(target.Direct);
        JsonConvert.DeserializeObject<FormLinkInformationClass>(toDeserialize, settings)!
            .Interface
            .Should().Be(target.Interface);
    }
    #endregion

    #region ModKey
    class ModKeyClass
    {
        public ModKey Member { get; set; } = TestConstants.MasterModKey2;
    }

    [Fact]
    public void ModKeyConverter_Serialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ModKeyJsonConverter());
        var toSerialize = new ModKeyClass()
        {
            Member = TestConstants.LightMasterModKey3
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":\"{toSerialize.Member}\"}}");
    }

    [Fact]
    public void ModKeyConverter_Deserialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ModKeyJsonConverter());
        var target = new ModKeyClass()
        {
            Member = TestConstants.LightMasterModKey3
        };
        var toDeserialize = $"{{\"Member\":\"{target.Member}\"}}";
        JsonConvert.DeserializeObject<ModKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void ModKeyConverter_Deserialize_Missing()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ModKeyJsonConverter());
        var target = new ModKeyClass();
        var toDeserialize = $"{{}}";
        JsonConvert.DeserializeObject<ModKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    class NullableModKeyClass
    {
        public ModKey? Member { get; set; } = TestConstants.MasterModKey2;
    }

    [Fact]
    public void ModKeyConverter_Nullable_Serialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ModKeyJsonConverter());
        var toSerialize = new NullableModKeyClass()
        {
            Member = TestConstants.LightMasterModKey3
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":\"{toSerialize.Member}\"}}");
    }

    [Fact]
    public void ModKeyConverter_Nullable_Serialize_Null()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ModKeyJsonConverter());
        var toSerialize = new NullableModKeyClass()
        {
            Member = null
        };
        JsonConvert.SerializeObject(toSerialize, settings)
            .Should().Be($"{{\"Member\":null}}");
    }

    [Fact]
    public void ModKeyConverter_Nullable_Deserialize()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ModKeyJsonConverter());
        var target = new NullableModKeyClass()
        {
            Member = TestConstants.LightMasterModKey3
        };
        var toDeserialize = $"{{\"Member\":\"{target.Member}\"}}";
        JsonConvert.DeserializeObject<NullableModKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void ModKeyConverter_Nullable_Deserialize_Missing()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ModKeyJsonConverter());
        var toDeserialize = $"{{}}";
        var target = new NullableModKeyClass();
        JsonConvert.DeserializeObject<NullableModKeyClass>(toDeserialize, settings)!
            .Member
            .Should().Be(target.Member);
    }

    [Fact]
    public void ModKeyConverter_Nullable_Deserialize_Null()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ModKeyJsonConverter());
        var toDeserialize = $"{{\"Member\":null}}";
        JsonConvert.DeserializeObject<NullableModKeyClass>(toDeserialize, settings)!
            .Member
            .Should().BeNull();
    }
    #endregion
}