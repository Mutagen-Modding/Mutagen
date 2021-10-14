using FluentAssertions;
using Mutagen.Bethesda.Core.UnitTests.Placeholders;
using Mutagen.Bethesda.Json;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Json
{
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

        // ToDo
        // Re-enable after FormLink refactor
        // class NullableFormLinkClass
        // {
        //     public FormLink<ITestMajorRecordGetter>? Member { get; set; } = new FormLink<ITestMajorRecordGetter>(TestConstants.Form1);
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
        // public void FormKeyConverter_NullableFormLink_Deserialize()
        // {
        //     var settings = new JsonSerializerSettings();
        //     settings.Converters.Add(new FormKeyJsonConverter());
        //     var target = new NullableFormLinkClass()
        //     {
        //         Member = new FormLink<ITestMajorRecordGetter>(TestConstants.Form2)
        //     };
        //     var toDeserialize = $"{{\"Member\":\"{target.Member}\"}}";
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

        // ToDo
        // Re-enable after FormLinkNullable refactor
        //class NullableFormLinkNullableClass
        //{
        //    public FormLinkNullable<INpcGetter>? Member { get; set; } = new FormLinkNullable<INpcGetter>(TestConstants.Form1);
        //}

        //[Fact]
        //public void FormKeyConverter_NullableFormLinkNullable_Serialize()
        //{
        //    var settings = new JsonSerializerSettings();
        //    settings.Converters.Add(new FormKeyJsonConverter());
        //    var toSerialize = new NullableFormLinkNullableClass()
        //    {
        //        Member = new FormLinkNullable<INpcGetter>(TestConstants.Form2)
        //    };
        //    JsonConvert.SerializeObject(toSerialize, settings)
        //        .Should().Be($"{{\"Member\":\"{toSerialize.Member}\"}}");
        //}

        //[Fact]
        //public void FormKeyConverter_NullableFormLinkNullable_Serialize_Null()
        //{
        //    var settings = new JsonSerializerSettings();
        //    settings.Converters.Add(new FormKeyJsonConverter());
        //    var toSerialize = new NullableFormLinkNullableClass()
        //    {
        //        Member = null
        //    };
        //    JsonConvert.SerializeObject(toSerialize, settings)
        //        .Should().Be($"{{\"Member\":null}}");
        //}

        //[Fact]
        //public void FormKeyConverter_NullableFormLinkNullable_Deserialize()
        //{
        //    var settings = new JsonSerializerSettings();
        //    settings.Converters.Add(new FormKeyJsonConverter());
        //    var target = new NullableFormLinkNullableClass()
        //    {
        //        Member = new FormLinkNullable<INpcGetter>(TestConstants.Form2)
        //    };
        //    var toDeserialize = $"{{\"Member\":\"{target.Member}\"}}";
        //    JsonConvert.DeserializeObject<NullableFormLinkNullableClass>(toDeserialize, settings)!
        //        .Member
        //        .Should().Be(target.Member);
        //}

        //[Fact]
        //public void FormKeyConverter_NullableFormLinkNullable_Deserialize_Missing()
        //{
        //    var settings = new JsonSerializerSettings();
        //    settings.Converters.Add(new FormKeyJsonConverter());
        //    var toDeserialize = $"{{}}";
        //    var target = new NullableFormLinkNullableClass();
        //    JsonConvert.DeserializeObject<NullableFormLinkNullableClass>(toDeserialize, settings)!
        //        .Member
        //        .Should().Be(target.Member);
        //}

        //[Fact]
        //public void FormKeyConverter_NullableFormLinkNullable_Deserialize_Null()
        //{
        //    var settings = new JsonSerializerSettings();
        //    settings.Converters.Add(new FormKeyJsonConverter());
        //    var toDeserialize = $"{{\"Member\":null}}";
        //    JsonConvert.DeserializeObject<NullableFormLinkNullableClass>(toDeserialize, settings)!
        //        .Member
        //        .Should().BeNull();
        //}
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
}
