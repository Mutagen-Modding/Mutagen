#nullable enable
using System.Text;
using CommandLine;

namespace Mutagen.Bethesda.Tests.CLI;

[Verb("inspect-bytes", HelpText = "Hex dump at a binary position")]
public class InspectBytesCommand
{
    [Option('p', "path", Required = true, HelpText = "Path to the binary file")]
    public string Path { get; set; } = string.Empty;

    [Option("pos", Required = true, HelpText = "Position in file (hex 0x... or decimal)")]
    public string Position { get; set; } = string.Empty;

    [Option('c', "count", Default = 64, HelpText = "Number of bytes to dump")]
    public int Count { get; set; }

    public int Run()
    {
        try
        {
            var pos = CliHelpers.ParsePosition(Position);
            using var stream = File.OpenRead(Path);
            stream.Seek(pos, SeekOrigin.Begin);
            var buf = new byte[Count];
            var bytesRead = stream.Read(buf, 0, buf.Length);

            for (int i = 0; i < bytesRead; i += 16)
            {
                var lineLen = Math.Min(16, bytesRead - i);
                var sb = new StringBuilder();
                sb.Append($"  {pos + i:X8}: ");

                for (int j = 0; j < 16; j++)
                {
                    if (j < lineLen)
                        sb.Append($"{buf[i + j]:X2} ");
                    else
                        sb.Append("   ");
                    if (j == 7) sb.Append(' ');
                }

                sb.Append(" |");
                for (int j = 0; j < lineLen; j++)
                {
                    var b = buf[i + j];
                    sb.Append(b is >= 0x20 and <= 0x7E ? (char)b : '.');
                }
                sb.Append('|');

                Console.WriteLine(sb.ToString());
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return -1;
        }
    }
}
