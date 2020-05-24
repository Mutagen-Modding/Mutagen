using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public class TranslatedString : ITranslatedString
    {
        /// <summary>
        /// The language a directly matched string is to be considered
        /// </summary>
        public static Language DefaultLanguage = Language.English;

        private string? _directString;
        private Dictionary<Language, string>? _localization;

        /// <inheritdoc />
        public string String 
        {
            get
            {
                if (_directString != null) return _directString;
                if (_localization!.TryGetValue(DefaultLanguage, out var str)) return str;
                return string.Empty;
            }
            set => Set(DefaultLanguage, value);
        }

        /// <summary>
        /// Creates a translated string with empty string set for the default language
        /// </summary>
        public TranslatedString()
        {
            _directString = string.Empty;
            _localization = null;
        }

        /// <summary>
        /// Creates a translated string with a value for the default language
        /// </summary>
        /// <param name="directString">String to register for the default language</param>
        public TranslatedString(string directString)
        {
            _directString = directString;
            _localization = null;
        }

        /// <summary>
        /// Creates a translated string with a number of strings for languages.
        /// If no string is provided for the default language, string.Empty will be assigned.
        /// </summary>
        /// <param name="strs">Language string pairs to register</param>
        public TranslatedString(IEnumerable<KeyValuePair<Language, string>> strs)
        {
            _localization = new Dictionary<Language, string>(strs);
            if (!_localization.TryGetValue(DefaultLanguage, out _))
            {
                _localization[DefaultLanguage] = string.Empty;
            }
        }

        /// <summary>
        /// Creates a translated string with a number of strings for languages.
        /// If no string is provided for the default language, string.Empty will be assigned.
        /// </summary>
        /// <param name="strs">Language string pairs to register</param>
        public TranslatedString(params KeyValuePair<Language, string>[] strs)
            : this ((IEnumerable<KeyValuePair<Language, string>>)strs)
        {
        }

        /// <inheritdoc />
        public bool Lookup(Language language, [MaybeNullWhen(false)] out string str)
        {
            if (_localization != null)
            {
                return _localization.TryGetValue(language, out str);
            }

            if (DefaultLanguage == language)
            {
                str = _directString;
                return str != null;
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
            if (_localization == null)
            {
                _localization = new Dictionary<Language, string>();

                // If we already have a direct string, swap to the internal setup where it's stored in the dictionary
                if (_directString != null)
                {
                    _localization[DefaultLanguage] = _directString;
                    _directString = null;
                }
            }
            _localization[language] = str;
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
            if (!_localization.TryGetValue(DefaultLanguage, out _directString))
            {
                _directString = string.Empty;
            }
            _localization = null;
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
                if (_directString != null && !_localization.ContainsKey(DefaultLanguage))
                {
                    yield return new KeyValuePair<Language, string>(DefaultLanguage, _directString);
                }
                foreach (var item in _localization)
                {
                    yield return item;
                }
            }
        }

        public static implicit operator TranslatedString(string str)
        {
            return new TranslatedString(str);
        }
    }
}
