using System;

namespace Mutagen.Bethesda.Pex.Binary.Translations
{
    public static class PexTranslationExt
    {
        public static bool IsBigEndian(this GameCategory gameCategory)
        {
            return gameCategory != GameCategory.Fallout4;
        }

        public static DateTime ToDateTime(this ulong value)
        {
            return DateTime.UnixEpoch.AddSeconds(value);
        }

        public static ulong ToUInt64(this DateTime dateTime)
        {
            var timeSpan = dateTime - DateTime.UnixEpoch;
            var elapsed = timeSpan.TotalSeconds;
            return (ulong)elapsed;
        }
    }
}