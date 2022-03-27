using Loqui;
using Noggog;

namespace Mutagen.Bethesda.XEdit.Generation;

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

                if (!int.TryParse(span.Slice(0, numberEndIndex).TrimStart().TrimEnd(), out var i))
                {
                    throw new ArgumentException();
                }

                span = SkipPast(span, "} '");

                var name = span.Slice(0, span.IndexOf('\'')).ToString();

                if (name.Contains("Unknown")) continue;

                fg.AppendLine($"{name} = {i},");
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
