using System.Globalization;
using Loqui;
using Noggog;

namespace Mutagen.Bethesda.Generation.Tools.XEdit.Enum;

public static class EnumConverter
{
    public static void Convert(FilePath source, FilePath output)
    {
        FileGeneration fg = new();
        fg.AppendLine("public enum EnumName");
        using (new BraceWrapper(fg))
        {
            foreach (var line in File.ReadLines(source))
            {
                var span = line.AsSpan();
                span = SkipPast(span, "{");

                var numberEndIndex = span.IndexOf("}");
                if (numberEndIndex == -1)
                {
                    throw new ArgumentException();
                }

                int i;

                var numberSpan = span.Slice(0, numberEndIndex).TrimStart().TrimEnd();
                bool hex = false;
                if (numberSpan.StartsWith("0x"))
                {
                    hex = true;
                    numberSpan = numberSpan.Slice(2);
                    if (!int.TryParse(numberSpan, NumberStyles.HexNumber, null, out i))
                    {
                        throw new ArgumentException();
                    }
                }
                else if (!int.TryParse(numberSpan, out i))
                {
                    throw new ArgumentException();
                }

                span = SkipPast(span, "} '");

                var name = span.Slice(0, span.IndexOf('\'')).ToString();

                if (name.Contains("Unknown")) continue;

                fg.AppendLine($"{name} = {(hex ? $"0x{i:x}" : i)},");
            }
        }
        fg.AppendLine();

        if (File.Exists(output))
        {
            File.Delete(output);
        }

        using var outputStream = new StreamWriter(File.OpenWrite(output));
        outputStream.Write(fg.ToString());
    }

    private static ReadOnlySpan<char> SkipPast(ReadOnlySpan<char> str, string target)
    {
        var index = str.IndexOf(target);
        if (index == -1)
        {
            throw new ArgumentException();
        }

        return str.Slice(index + target.Length);
    }
}
