using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Pex;

public static class PexMixIn
{
    /// <summary>
    /// Write to a .pex file.
    /// </summary>
    /// <param name="pexFile">The pex file to write.</param>
    /// <param name="outputPath">Output path.</param>
    /// <param name="gameCategory">Game Category of the pex file.</param>
    public static void WritePexFile(this PexFile pexFile, string outputPath, GameCategory gameCategory)
    {
        var dirName = Path.GetDirectoryName(outputPath);
        Directory.CreateDirectory(dirName ?? string.Empty);
            
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        using var fs = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        using var bw = new PexWriter(fs, Encoding.UTF8, gameCategory.IsBigEndian());
            
        pexFile.Write(bw);
    }
}