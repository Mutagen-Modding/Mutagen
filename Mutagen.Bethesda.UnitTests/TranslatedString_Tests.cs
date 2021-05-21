using FluentAssertions;
using Mutagen.Bethesda.Strings;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public abstract class ATranslatedString_Tests
    {
        public const string EnglishString = "Hello";
        public const string FrenchString = "Bonjour";
        public const string SpainishString = "Hola";
        public const uint LookupKey = 1234;
        public const Language UnusedLanguage = Language.Italian;

        public abstract TranslatedString Create(string str);

        public abstract TranslatedString Create(params KeyValuePair<Language, string>[] strs);

        [Fact]
        public void TypicalStraight()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(EnglishString);
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(Language.English, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(Language.English));
        }

        [Fact]
        public void TypicalMultilanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(
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
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.French, FrenchString));

            // English string
            Assert.Null(str.String);
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
            ITranslatedString str = Create(EnglishString);
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
            ITranslatedString str = Create(EnglishString);
            str.Set(Language.English, FrenchString);
            Assert.Equal(FrenchString, str.String);
        }

        [Fact]
        public void Remove()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            str.RemoveNonDefault(Language.French);

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
        public void RemoveThenAdd()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            str.RemoveNonDefault(Language.French);
            str.Set(Language.French, "Hmm");
            Assert.True(str.TryLookup(Language.French, out var outStr));
            Assert.Equal("Hmm", outStr);
        }

        [Fact]
        public void RemoveDefault()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            str.RemoveNonDefault(Language.English);

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
            ITranslatedString str = Create(EnglishString);
            str.RemoveNonDefault(Language.French);

            // English string
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(Language.English, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(Language.English));
        }

        // ToDo
        // Seems to fail on occasion
        [Fact]
        public void GetAfterDefaultLanguageSwap()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(EnglishString);
            TranslatedString.DefaultLanguage = Language.French;
            Assert.Equal(EnglishString, str.String);
            Assert.True(str.TryLookup(Language.English, out var outStr));
            Assert.Equal(EnglishString, outStr);
            Assert.Equal(EnglishString, str.Lookup(Language.English));
        }

        [Fact]
        public void GetAfterDefaultLanguageSwapWithRegister()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            TranslatedString.DefaultLanguage = Language.Spanish;
            Assert.Equal(EnglishString, str.String);
            Assert.False(str.TryLookup(Language.Spanish, out var _));
            Assert.Null(str.Lookup(Language.Spanish));
        }

        [Fact]
        public void LookupUnregisteredLanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            Assert.False(str.TryLookup(Language.Spanish, out var _));
            Assert.Null(str.Lookup(Language.Spanish));
        }

        [Fact]
        public void ToArray_Straight()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(EnglishString);
            var arr = str.ToArray();
            Assert.Single(arr);
            Assert.Equal(Language.English, arr[0].Key);
            Assert.Equal(EnglishString, arr[0].Value);
        }

        [Fact]
        public void ToArray_MultiLanguage()
        {
            TranslatedString.DefaultLanguage = Language.English;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));

            var arr = str.ToArray();
            Assert.Equal(2, arr.Length);
            Assert.Equal(Language.English, arr[0].Key);
            Assert.Equal(EnglishString, arr[0].Value);
            Assert.Equal(Language.French, arr[1].Key);
            Assert.Equal(FrenchString, arr[1].Value);
        }

        [Fact]
        public void Equality_OnlyDefault_Self()
        {
            var comp = TranslatedString.OnlyDefaultComparer;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            comp.Equals(str, str).Should().BeTrue();
        }

        [Fact]
        public void Equality_OnlyDefault_EqualMany()
        {
            var comp = TranslatedString.OnlyDefaultComparer;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            ITranslatedString str2 = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            comp.Equals(str, str2).Should().BeTrue();
        }

        [Fact]
        public void Equality_OnlyDefault_Single()
        {
            var comp = TranslatedString.OnlyDefaultComparer;
            ITranslatedString str = Create(EnglishString);
            ITranslatedString str2 = Create(EnglishString);
            comp.Equals(str, str2).Should().BeTrue();
        }

        [Fact]
        public void Equality_OnlyDefault_NotEqual()
        {
            var comp = TranslatedString.OnlyDefaultComparer;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, FrenchString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            ITranslatedString str2 = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            comp.Equals(str, str2).Should().BeFalse();
        }

        [Fact]
        public void Equality_All_Self()
        {
            var comp = TranslatedString.AllLanguageComparer;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            comp.Equals(str, str).Should().BeTrue();
        }

        [Fact]
        public void Equality_All_EqualMany()
        {
            var comp = TranslatedString.AllLanguageComparer;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            ITranslatedString str2 = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            comp.Equals(str, str2).Should().BeTrue();
        }

        [Fact]
        public void Equality_All_Single()
        {
            var comp = TranslatedString.AllLanguageComparer;
            ITranslatedString str = Create(EnglishString);
            ITranslatedString str2 = Create(EnglishString);
            comp.Equals(str, str2).Should().BeTrue();
        }

        [Fact]
        public void Equality_All_NotEqual()
        {
            var comp = TranslatedString.AllLanguageComparer;
            ITranslatedString str = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, EnglishString));
            ITranslatedString str2 = Create(
                new KeyValuePair<Language, string>(Language.English, EnglishString),
                new KeyValuePair<Language, string>(Language.French, FrenchString));
            comp.Equals(str, str2).Should().BeFalse();
        }
    }

    public class TranslatedString_ByCtor : ATranslatedString_Tests
    {
        public override TranslatedString Create(string str)
        {
            return new TranslatedString(str);
        }

        public override TranslatedString Create(params KeyValuePair<Language, string>[] strs)
        {
            return new TranslatedString(strs);
        }
    }

    public class TranslatedString_ByLookup : ATranslatedString_Tests
    {
        public class ManualStringsLookup : IStringsFolderLookup
        {
            private readonly StringsSource _source;
            private readonly Dictionary<Language, Dictionary<uint, string>> _dict = new Dictionary<Language, Dictionary<uint, string>>();

            public ManualStringsLookup(
                StringsSource source,
                params (Language Language, uint Key, string Str)[] strs)
            {
                _source = source;
                foreach (var kv in strs)
                {
                    _dict.GetOrAdd(kv.Language, () => new Dictionary<uint, string>())[kv.Key] = kv.Str;
                }
                _dict.Add(UnusedLanguage, new Dictionary<uint, string>());
            }

            public IEnumerable<Language> AvailableLanguages(StringsSource source)
            {
                return _dict.Keys;
            }

            public TranslatedString CreateString(StringsSource source, uint key)
            {
                return new TranslatedString()
                {
                    StringsLookup = this,
                    Key = key,
                    StringsSource = source
                };
            }

            public bool TryLookup(StringsSource source, Language language, uint key, [MaybeNullWhen(false)] out string str)
            {
                str = null;
                if (source != _source) return false;
                if (!_dict.TryGetValue(language, out var subDict)) return false;
                return subDict.TryGetValue(key, out str);
            }
        }

        public override TranslatedString Create(string str)
        {
            var lookup = new ManualStringsLookup(
                StringsSource.DL, 
                (TranslatedString.DefaultLanguage, LookupKey, str));
            return new TranslatedString()
            {
                StringsSource = StringsSource.DL,
                Key = LookupKey,
                StringsLookup = lookup
            };
        }

        public override TranslatedString Create(params KeyValuePair<Language, string>[] strs)
        {
            var lookup = new ManualStringsLookup(
                StringsSource.DL,
                strs.Select(kv =>
                {
                    return (kv.Key, LookupKey, kv.Value);
                }).ToArray());
            return new TranslatedString()
            {
                StringsSource = StringsSource.DL,
                Key = LookupKey,
                StringsLookup = lookup
            };
        }
    }
}
