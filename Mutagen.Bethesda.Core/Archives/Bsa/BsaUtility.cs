using Noggog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Bsa
{
    static class BsaUtility
    {
        private static readonly Lazy<Encoding> Windows1252 = new Lazy<Encoding>(() =>
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(1252);
        });

        private static Encoding GetEncoding(BsaVersionType version)
        {
            return version switch
            {
                BsaVersionType.SSE => Windows1252.Value,
                _ => Encoding.UTF7
            };
        }

        public static string ReadStringLenTerm(this ReadOnlyMemorySlice<byte> bytes, BsaVersionType version)
        {
            if (bytes.Length <= 1) return string.Empty;
            return GetEncoding(version).GetString(bytes.Slice(1, bytes[0]));
        }

        public static string ReadStringTerm(this ReadOnlyMemorySlice<byte> bytes, BsaVersionType version)
        {
            if (bytes.Length <= 1) return string.Empty;
            return GetEncoding(version).GetString(bytes[0..^1]);
        }

        public static void CopyToLimit(this Stream from, Stream to, long limit)
        {
            var buff = new byte[1024];
            while (limit > 0)
            {
                var to_read = (int)Math.Min(buff.Length, limit);
                var read = from.Read(buff, 0, to_read);
                if (read == 0)
                    throw new Exception("End of stream before end of limit");
                to.Write(buff, 0, read);
                limit -= read;
            }

            to.Flush();
        }

        public static async Task CopyToLimitAsync(this Stream from, Stream to, long limit)
        {
            var buff = new byte[1024];
            while (limit > 0)
            {
                var to_read = Math.Min(buff.Length, limit);
                var read = await from.ReadAsync(buff, 0, (int)to_read).ConfigureAwait(false);
                if (read == 0)
                    throw new Exception("End of stream before end of limit");
                await to.WriteAsync(buff, 0, read);
                limit -= read;
            }

            to.Flush();
        }
    }
}
