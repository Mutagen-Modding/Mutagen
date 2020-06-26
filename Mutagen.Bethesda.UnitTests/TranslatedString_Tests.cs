using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class TranslatedString_Tests
    {
        public const string EnglishString = "Hello";
        public const string FrenchString = "Bonjour";
        public const string SpainishString = "Hola";
        public const uint LookupKey = 1234;

        [Fact]
        public void TypicalStraight()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(EnglishString);
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(TranslatedString.DefaultLanguage, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(TranslatedString.DefaultLanguage));
        }

        [Fact]
        public void TypicalMultilanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));

            // English string
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(Language.English, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(Language.English));

            // French string
            Assert.True(str.TryLookup(Language.French, out outStr));
            Assert.Equal(FrenchString, outStr);
            Assert.Equal(FrenchString, str.Lookup(Language.French));
        }

        [Fact]
        public void MissingDefaultLanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.French, FrenchString));

            // English string
            Assert.Equal(string.Empty, str.String);
            Assert.False(str.TryLookup(Language.English, out var outStr));

            // French string
            Assert.True(str.TryLookup(Language.French, out outStr));
            Assert.Equal(FrenchString, outStr);
            Assert.Equal(FrenchString, str.Lookup(Language.French));
        }

        [Fact]
        public void Set()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(EnglishString);
            str.Set(Language.French, FrenchString);

            // English string
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(Language.English, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(Language.English));

            // French string
            Assert.True(str.TryLookup(Language.French, out outStr));
            Assert.Equal(FrenchString, outStr);
            Assert.Equal(FrenchString, str.Lookup(Language.French));
        }

        [Fact]
        public void SetDefaultOntoNonLocalized()
        {
            ITranslatedString str = new TranslatedString(EnglishString);
            str.Set(TranslatedString.DefaultLanguage, FrenchString);
            Assert.Equal(FrenchString, str.String);
        }

        [Fact]
        public void Remove()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            Assert.True(str.RemoveNonDefault(Language.French));

            // English string
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(Language.English, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(Language.English));

            // French string
            Assert.False(str.TryLookup(Language.French, out _));
            Assert.Null(str.Lookup(Language.French));
        }

        [Fact]
        public void RemoveDefault()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            Assert.False(str.RemoveNonDefault(Language.English));

            // English string
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(Language.English, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(Language.English));

            // French string
            Assert.True(str.TryLookup(Language.French, out outStr));
            Assert.Equal(FrenchString, outStr);
            Assert.Equal(FrenchString, str.Lookup(Language.French));
        }

        [Fact]
        public void RemoveDirect()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(EnglishString);
            Assert.False(str.RemoveNonDefault(Language.French));

            // English string
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(Language.English, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(Language.English));
        }

        [Fact]
        public void GetAfterDefaultLanguageSwap()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(EnglishString);
            TranslatedString.DefaultLanguage = Language.French;
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(TranslatedString.DefaultLanguage, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(TranslatedString.DefaultLanguage));
        }

        [Fact]
        public void GetAfterDefaultLanguageSwapWithRegister()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            TranslatedString.DefaultLanguage = Language.Spanish;
            Assert.Equal(string.Empty, str.String);
            Assert.False(str.TryLookup(Language.Spanish, out var _));
            Assert.Equal(string.Empty, str.Lookup(Language.Spanish));
        }

        [Fact]
        public void LookupUnregisteredLanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            Assert.False(str.TryLookup(Language.Spanish, out var _));
            Assert.Null(str.Lookup(Language.Spanish));
        }

        [Fact]
        public void LookupUnregisteredLanguage_WithLookup()
        {
            TranslatedString.DefaultLanguage = Language.English;
            TranslatedString str = new TranslatedString(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            str.StringsLookup = new ManualStringsLookup();
            str.Key = LookupKey;
            str.StringsSource = StringsSource.DL;
            Assert.True(str.TryLookup(Language.Spanish, out var spain));
            Assert.Equal(SpainishString, spain);
            var spainNullable = str.Lookup(Language.Spanish);
            Assert.Equal(SpainishString, spainNullable);
        }

        [Fact]
        public void LookupUnregisteredDefault_WithLookup()
        {
            TranslatedString.DefaultLanguage = Language.Spanish;
            TranslatedString str = new TranslatedString();
            str.StringsLookup = new ManualStringsLookup();
            str.Key = LookupKey;
            str.StringsSource = StringsSource.DL;
            Assert.Equal(SpainishString, str.String);
        }

        public class ManualStringsLookup : IStringsFolderLookup
        {
            public TranslatedString CreateString(StringsSource source, uint key)
            {
                throw new NotImplementedException();
            }

            public bool TryLookup(StringsSource source, Language language, uint key, [MaybeNullWhen(false)] out string str)
            {
                Assert.Equal(StringsSource.DL, source);
                Assert.Equal(Language.Spanish, language);
                Assert.Equal(LookupKey, key);
                str = SpainishString;
                return true;
            }
        }
    }
}
