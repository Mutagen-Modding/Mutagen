using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class TranslatedString_Tests
    {
        public const string SomeString = "Hello";
        public const string SomeAltString = "Bonjour";

        [Fact]
        public void TypicalStraight()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(SomeString);
            Assert.Equal(SomeString, str.String);
            Assert.True(str.Lookup(TranslatedString.DefaultLanguage, out var outStr));
            Assert.Equal(SomeString, outStr);
            Assert.Equal(SomeString, str.Lookup(TranslatedString.DefaultLanguage));
        }

        [Fact]
        public void TypicalMultilanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, SomeString),
                new KeyValuePair<Language, string>(Language.French, SomeAltString));

            // English string
            Assert.Equal(SomeString, str.String);
            Assert.True(str.Lookup(Language.English, out var outStr));
            Assert.Equal(SomeString, outStr);
            Assert.Equal(SomeString, str.Lookup(Language.English));

            // French string
            Assert.True(str.Lookup(Language.French, out outStr));
            Assert.Equal(SomeAltString, outStr);
            Assert.Equal(SomeAltString, str.Lookup(Language.French));
        }

        [Fact]
        public void MissingDefaultLanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.French, SomeAltString));

            // English string
            Assert.Equal(string.Empty, str.String);
            Assert.True(str.Lookup(Language.English, out var outStr));
            Assert.Equal(string.Empty, outStr);
            Assert.Equal(string.Empty, str.Lookup(Language.English));

            // French string
            Assert.True(str.Lookup(Language.French, out outStr));
            Assert.Equal(SomeAltString, outStr);
            Assert.Equal(SomeAltString, str.Lookup(Language.French));
        }

        [Fact]
        public void Set()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(SomeString);
            str.Set(Language.French, SomeAltString);

            // English string
            Assert.Equal(SomeString, str.String);
            Assert.True(str.Lookup(Language.English, out var outStr));
            Assert.Equal(SomeString, outStr);
            Assert.Equal(SomeString, str.Lookup(Language.English));

            // French string
            Assert.True(str.Lookup(Language.French, out outStr));
            Assert.Equal(SomeAltString, outStr);
            Assert.Equal(SomeAltString, str.Lookup(Language.French));
        }

        [Fact]
        public void SetDefaultOntoNonLocalized()
        {
            ITranslatedString str = new TranslatedString(SomeString);
            str.Set(TranslatedString.DefaultLanguage, SomeAltString);
            Assert.Equal(SomeAltString, str.String);
        }

        [Fact]
        public void Remove()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, SomeString),
                new KeyValuePair<Language, string>(Language.French, SomeAltString));
            Assert.True(str.RemoveNonDefault(Language.French));

            // English string
            Assert.Equal(SomeString, str.String);
            Assert.True(str.Lookup(Language.English, out var outStr));
            Assert.Equal(SomeString, outStr);
            Assert.Equal(SomeString, str.Lookup(Language.English));

            // French string
            Assert.False(str.Lookup(Language.French, out _));
            Assert.Null(str.Lookup(Language.French));
        }

        [Fact]
        public void RemoveDefault()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, SomeString),
                new KeyValuePair<Language, string>(Language.French, SomeAltString));
            Assert.False(str.RemoveNonDefault(Language.English));

            // English string
            Assert.Equal(SomeString, str.String);
            Assert.True(str.Lookup(Language.English, out var outStr));
            Assert.Equal(SomeString, outStr);
            Assert.Equal(SomeString, str.Lookup(Language.English));

            // French string
            Assert.True(str.Lookup(Language.French, out outStr));
            Assert.Equal(SomeAltString, outStr);
            Assert.Equal(SomeAltString, str.Lookup(Language.French));
        }

        [Fact]
        public void RemoveDirect()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(SomeString);
            Assert.False(str.RemoveNonDefault(Language.French));

            // English string
            Assert.Equal(SomeString, str.String);
            Assert.True(str.Lookup(Language.English, out var outStr));
            Assert.Equal(SomeString, outStr);
            Assert.Equal(SomeString, str.Lookup(Language.English));
        }

        [Fact]
        public void GetAfterDefaultLanguageSwap()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(SomeString);
            TranslatedString.DefaultLanguage = Language.French;
            Assert.Equal(SomeString, str.String);
            Assert.True(str.Lookup(TranslatedString.DefaultLanguage, out var outStr));
            Assert.Equal(SomeString, outStr);
            Assert.Equal(SomeString, str.Lookup(TranslatedString.DefaultLanguage));
        }

        [Fact]
        public void GetAfterDefaultLanguageSwapWithRegister()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, SomeString),
                new KeyValuePair<Language, string>(Language.French, SomeAltString));
            TranslatedString.DefaultLanguage = Language.Spanish;
            Assert.Equal(string.Empty, str.String);
            Assert.False(str.Lookup(TranslatedString.DefaultLanguage, out var _));
            Assert.Equal(string.Empty, str.Lookup(TranslatedString.DefaultLanguage));
        }

        [Fact]
        public void LookupUnregisteredLanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, SomeString),
                new KeyValuePair<Language, string>(Language.French, SomeAltString));
            Assert.False(str.Lookup(Language.Spanish, out var _));
            Assert.Null(str.Lookup(Language.Spanish));
        }
    }
}
