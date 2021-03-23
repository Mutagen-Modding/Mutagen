using System;

namespace Mutagen.Bethesda.Pex.Extensions
{
    internal static class NumericExtensions
    {
        internal static DateTime ToDateTime(this ulong value)
        {
            return DateTime.UnixEpoch.AddSeconds(value);
        }

        internal static ulong ToUInt64(this DateTime dateTime)
        {
            var timeSpan = dateTime - DateTime.UnixEpoch;
            var elapsed = timeSpan.TotalSeconds;
            return (ulong) elapsed;
        }
    }
}
