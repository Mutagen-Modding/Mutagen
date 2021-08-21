using System;
using System.Text;

namespace Mutagen.Bethesda.Strings
{
    internal static class Encodings
    {
        private static readonly Encoding _1250;
        private static readonly Encoding _1251;
        private static readonly Encoding _1252;

        public static Encoding Default => _1252;
        
        static Encodings()
        {
            _1250 = CodePagesEncodingProvider.Instance.GetEncoding(1250)!;
            _1251 = CodePagesEncodingProvider.Instance.GetEncoding(1251)!;
            _1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252)!;
        }
        
        public static Encoding Get(GameRelease release, Language language)
        {
            switch (release)
            {
                case GameRelease.EnderalLE:
                case GameRelease.SkyrimLE:
                    return GetSkyrimLeEncoding(language);
                case GameRelease.SkyrimSE:
                case GameRelease.SkyrimVR:
                case GameRelease.EnderalSE:
                case GameRelease.Fallout4:
                    return GetSkyrimSeEncoding(language);
                default:
                    throw new NotImplementedException();
            }
        }

        public static Encoding GetSkyrimLeEncoding(Language language)
        {
            switch (language)
            {
                case Language.Polish:
                    return _1250;
                case Language.Russian:
                    return _1251;
                case Language.English:
                case Language.French:
                case Language.German:
                case Language.Spanish:
                case Language.Portuguese_Brazil:
                case Language.Spanish_Mexico:
                case Language.Italian:
                    return _1252;
                case Language.Chinese:
                case Language.Japanese:
                    return Encoding.UTF8;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }

        public static Encoding GetSkyrimSeEncoding(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return Encoding.ASCII;
                default:
                    return Encoding.UTF8;
            }
        }
    }
}