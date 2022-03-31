using Loqui;
using Noggog;

namespace Mutagen.Bethesda.Generation.Tools.XEdit.Enum;

/// <summary>
/// Takes a hand-extracted xEdit snippet of the condition function enum and translates to a C# enum output
/// </summary>
public static class FunctionEnumGenerator
{
    private const string IndexStr = "Index:";
    private const string NameStr = "Name: '";
    
    /// <summary>
    /// Expects files with lines like
    /// (Index:   0; Name: 'GetWantBlocking'),    //   0
    /// </summary>
    public static void Convert(FilePath source, FilePath output)
    {
        FileGeneration fg = new();
        fg.AppendLine("public enum Function");
        using (new BraceWrapper(fg))
        {
            foreach (var line in File.ReadLines(source))
            {
                var span = line.AsSpan();
                span = SkipPast(span, IndexStr);
            
                var semiColonIndex = span.IndexOf(";");
                if (semiColonIndex == -1)
                {
                    throw new ArgumentException();
                }

                if (!int.TryParse(span.Slice(0, semiColonIndex), out var i))
                {
                    throw new ArgumentException();
                }

                span = SkipPast(span, NameStr);

                var name = span.Slice(0, span.IndexOf('\''));

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