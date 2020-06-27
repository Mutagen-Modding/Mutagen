using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A string that can be represented in multiple different languages.<br/>
    /// Threadsafe.
    /// </summary>
    public class TranslatedString : ITranslatedString
    {
        /// <summary>
        /// The language a directly matched string is to be considered
        /// </summary>
        public static Language DefaultLanguage = Language.English;

        private string? _directString;
        private readonly object _lock = new object();
        private Dictionary<Language, string?>? _localization;
        internal uint Key;
        internal IStringsFolderLookup? StringsLookup;
        internal StringsSource StringsSource;

        /// <inheritdoc />
        public string String
        {
            get
            {
                lock (_lock)
                {
                    if (_directString != null) return _directString;
                    if (TryLookup(DefaultLanguage, out var str))
                    {
                        return str;
                    }
                    return string.Empty;
                }
            }
            set => Set(DefaultLanguage, value);
        }

        /// <summary>
        /// Creates a translated string with empty string set for the default language
        /// </summary>
        public TranslatedString()
        {
        }

        /// <summary>
        /// Creates a translated string with a value for the default language
        /// </summary>
        /// <param name="directString">String to register for the default language</param>
        public TranslatedString(string directString)
        {
            _directString = directString;
        }

        /// <summary>
        /// Creates a translated string with a number of strings for languages.
        /// If no string is provided for the default language, string.Empty will be assigned.
        /// </summary>
        /// <param name="strs">Language string pairs to register</param>
        public TranslatedString(IEnumerable<KeyValuePair<Language, string>> strs)
        {
            _localization = new Dictionary<Language, string?>();
            foreach (var str in strs)
            {
                _localization[str.Key] = str.Value;
            }
        }

        /// <summary>
        /// Creates a translated string with a number of strings for languages.
        /// If no string is provided for the default language, string.Empty will be assigned.
        /// </summary>
        /// <param name="strs">Language string pairs to register</param>
        public TranslatedString(params KeyValuePair<Language, string>[] strs)
            : this((IEnumerable<KeyValuePair<Language, string>>)strs)
        {
        }

        /// <inheritdoc />
        public bool TryLookup(Language language, [MaybeNullWhen(false)] out string str)
        {
            if (DefaultLanguage == language
                && _directString != null)
            {
                str = _directString;
                return true;
            }

            lock (_lock)
            {
                if (_localization != null
                    && _localization.TryGetValue(language, out str))
                {
                    return str != null;
                }
                else if (StringsLookup != null)
                {
                    if (_localization == null)
                    {
                        _localization = new Dictionary<Language, string?>();
                    }
                    if (StringsLookup.TryLookup(this.StringsSource, language, this.Key, out str))
                    {
                        _localization[language] = str;
                        return true;
                    }
                    else
                    {
                        _localization[language] = null;
                        return false;
                    }
                }
            }

            str = default;
            return false;
        }

        /// <summary>
        /// Sets the string for a specific language
        /// </summary>
        /// <param name="language">Language to register the string under</param>
        /// <param name="str">String to register</param>
        public void Set(Language language, string str)
        {
            lock (_lock)
            {
                if (_localization == null)
                {
                    if (language == DefaultLanguage)
                    {
                        _directString = str;
                        return;
                    }

                    _localization = new Dictionary<Language, string?>();

                    // If we already have a direct string, swap to the internal setup where it's stored in the dictionary
                    if (_directString != null)
                    {
                        _localization[DefaultLanguage] = _directString;
                        _directString = null;
                    }
                }
                _localization[language] = str;
            }
        }

        /// <inheritdoc />
        public bool RemoveNonDefault(Language language)
        {
            if (language == DefaultLanguage) return false;
            if (_localization == null) return false;
            return _localization.Remove(language);
        }

        /// <inheritdoc />
        public void ClearNonDefault()
        {
            if (_localization == null) return;
            lock (_lock)
            {
                if (!_localization.TryGetValue(DefaultLanguage, out _directString))
                {
                    _directString = string.Empty;
                }
                _localization = null;
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            ClearNonDefault();
            this._directString = null;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<KeyValuePair<Language, string>> GetEnumerator()
        {
            if (_localization == null)
            {
                if (_directString != null)
                {
                    yield return new KeyValuePair<Language, string>(DefaultLanguage, _directString);
                }
                yield break;
            }
            else
            {
                lock (_lock)
                {
                    if (_directString != null && !_localization.ContainsKey(DefaultLanguage))
                    {
                        yield return new KeyValuePair<Language, string>(DefaultLanguage, _directString);
                    }
                    foreach (var item in _localization)
                    {
                        if (item.Value == null) continue;
                        yield return new KeyValuePair<Language, string>(item.Key, item.Value);
                    }
                }
            }
        }

        public static implicit operator TranslatedString(string str)
        {
            return new TranslatedString(str);
        }

        public override string ToString()
        {
            return this.String;
        }
    }
}
