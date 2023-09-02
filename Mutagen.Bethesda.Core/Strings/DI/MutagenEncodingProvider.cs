using System.Text;

namespace Mutagen.Bethesda.Strings.DI;

public interface IMutagenEncodingProvider
{
    IMutagenEncoding GetEncoding(GameRelease release, Language language);
}

public sealed class MutagenEncodingProvider : IMutagenEncodingProvider
{
    public static readonly MutagenEncodingProvider Instance = new();
    public static readonly IMutagenEncoding _932;
    public static readonly IMutagenEncoding _1250;
    public static readonly IMutagenEncoding _1251;
    public static readonly IMutagenEncoding _1252;
    public static readonly IMutagenEncoding _1253;
    public static readonly IMutagenEncoding _1254;
    public static readonly IMutagenEncoding _1256;
    public static readonly IMutagenEncoding _utf8;

    public static readonly IMutagenEncoding _utf8_932;
    public static readonly IMutagenEncoding _utf8_1250;
    public static readonly IMutagenEncoding _utf8_1251;
    public static readonly IMutagenEncoding _utf8_1252;
    public static readonly IMutagenEncoding _utf8_1253;
    public static readonly IMutagenEncoding _utf8_1254;
    public static readonly IMutagenEncoding _utf8_1256;
        
    static MutagenEncodingProvider()
    {
        _932 = new MutagenEncodingWrapper(
            CodePagesEncodingProvider.Instance.GetEncoding(932)!);
        _1250 = new MutagenEncodingWrapper(
            CodePagesEncodingProvider.Instance.GetEncoding(1250)!);
        _1251 = new MutagenEncodingWrapper(
            CodePagesEncodingProvider.Instance.GetEncoding(1251)!);
        _1252 = new MutagenEncodingWrapper(
            CodePagesEncodingProvider.Instance.GetEncoding(1252)!);
        _1253 = new MutagenEncodingWrapper(
            CodePagesEncodingProvider.Instance.GetEncoding(1253)!);
        _1254 = new MutagenEncodingWrapper(
            CodePagesEncodingProvider.Instance.GetEncoding(1254)!);
        _1256 = new MutagenEncodingWrapper(
            CodePagesEncodingProvider.Instance.GetEncoding(1256)!);
        _utf8 = new MutagenEncodingWrapper(Encoding.UTF8);
        _utf8_1250 = new MutagenEncodingFallbackWrapper(
            _utf8,
            _1250);
        _utf8_932 = new MutagenEncodingFallbackWrapper(
            _utf8,
            _932);
        _utf8_1251 = new MutagenEncodingFallbackWrapper(
            _utf8,
            _1251);
        _utf8_1252 = new MutagenEncodingFallbackWrapper(
            _utf8,
            _1252);
        _utf8_1253 = new MutagenEncodingFallbackWrapper(
            _utf8,
            _1253);
        _utf8_1254 = new MutagenEncodingFallbackWrapper(
            _utf8,
            _1254);
        _utf8_1256 = new MutagenEncodingFallbackWrapper(
            _utf8,
            _1256);
    }
        
    public IMutagenEncoding GetEncoding(GameRelease release, Language language)
    {
        switch (release)
        {
            case GameRelease.EnderalLE:
            case GameRelease.SkyrimLE:
                return GetSkyrimLeEncoding(language);
            case GameRelease.SkyrimSE:
            case GameRelease.SkyrimSEGog:
            case GameRelease.SkyrimVR:
            case GameRelease.EnderalSE:
            case GameRelease.Fallout4:
            case GameRelease.Starfield:
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
            case Language.Hungarian:
            case Language.Czech:
                return _1250;
            case Language.Russian:
                return _1251;
            case Language.English:
            case Language.French:
            case Language.German:
            case Language.Spanish:
            case Language.Spanish_Mexico:
            case Language.Finnish:
            case Language.Danish:
            case Language.Norwegian:
            case Language.Swedish:
            case Language.Portuguese_Brazil:
            case Language.Italian:
                return _1252;
            case Language.Greek:
                return _1253;
            case Language.Turkish:
                return _1254;
            case Language.Arabic:
                return _1256;
            case Language.Korean:
            case Language.Chinese:
            case Language.Japanese:
            case Language.Thai:
                return _utf8;
            default:
                throw new ArgumentOutOfRangeException(nameof(language), language, null);
        }
    }

    private IMutagenEncoding GetSkyrimSeEncoding(Language language)
    {
        switch (language)
        {
            case Language.Japanese:
                return _utf8_932;
            case Language.Czech:
            case Language.Hungarian:
            case Language.Polish:
                return _utf8_1250;
            case Language.Russian:
                return _utf8_1251;
            case Language.English:
                return _1252;
            case Language.French:
            case Language.German:
            case Language.Italian:
            case Language.Spanish:
            case Language.Spanish_Mexico:
            case Language.Danish:
            case Language.Finnish:
            case Language.Norwegian:
            case Language.Swedish:
            case Language.Portuguese_Brazil:
                return _utf8_1252;
            case Language.Greek:
                return _utf8_1253;
            case Language.Turkish:
                return _utf8_1254;
            case Language.Arabic:
                return _utf8_1256;
            case Language.Chinese:
            case Language.Korean:
            case Language.Thai:
            default:
                return _utf8;
        }
    }
}