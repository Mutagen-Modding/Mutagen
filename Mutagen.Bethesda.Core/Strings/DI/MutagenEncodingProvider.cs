using System;
using System.Text;

namespace Mutagen.Bethesda.Strings.DI
{
    public interface IMutagenEncodingProvider
    {
        IMutagenEncoding GetEncoding(GameRelease release, Language language);
    }

    public class MutagenEncodingProvider : IMutagenEncodingProvider
    {
        public static readonly IMutagenEncoding _1250;
        public static readonly IMutagenEncoding _1251;
        public static readonly IMutagenEncoding _1252;
        public static readonly IMutagenEncoding _utf8;

        public IMutagenEncoding Default => _1252;
        
        static MutagenEncodingProvider()
        {
            _1250 = new MutagenEncodingWrapper(
                CodePagesEncodingProvider.Instance.GetEncoding(1250)!);
            _1251 = new MutagenEncodingWrapper(
                CodePagesEncodingProvider.Instance.GetEncoding(1251)!);
            _1252 = new MutagenEncodingWrapper(
                CodePagesEncodingProvider.Instance.GetEncoding(1252)!);
            _utf8 = new MutagenEncodingWrapper(Encoding.UTF8);
        }
        
        public IMutagenEncoding GetEncoding(GameRelease release, Language language)
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

        private IMutagenEncoding GetSkyrimLeEncoding(Language language)
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
                    return _utf8;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }

        private IMutagenEncoding GetSkyrimSeEncoding(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return _1252;
                default:
                    return _utf8;
            }
        }
    }
}