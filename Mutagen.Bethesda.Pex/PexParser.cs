using System;
using System.IO;
using System.Text;
using Mutagen.Bethesda.Pex.DataTypes;
using Mutagen.Bethesda.Pex.Exceptions;
using Mutagen.Bethesda.Pex.Interfaces;

namespace Mutagen.Bethesda.Pex
{
    public static class PexParser
    {
        public static IPexFile ParsePexFile(string file)
        {
            if (!File.Exists(file))
                throw new ArgumentException($"Input file does not exist {file}!", nameof(file));
            
            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var br = new BinaryReader(fs, Encoding.UTF8);

            //https://en.uesp.net/wiki/Skyrim_Mod:Compiled_Script_File_Format
            var pexFile = new PexFile(br);

            if (fs.Position != fs.Length)
                throw new PexParsingException("Finished reading but end of the stream was not reached! " +
                                              $"Current position: {fs.Position} " +
                                              $"Stream length: {fs.Length} " +
                                              $"Missing: {fs.Length - fs.Position}");
            
            return pexFile;
        }

        public static void WritePexFile(this IPexFile pexFile, string outputPath)
        {
            var dirName = Path.GetDirectoryName(outputPath);
            Directory.CreateDirectory(dirName ?? string.Empty);
            
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            using var fs = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            using var bw = new BinaryWriter(fs, Encoding.UTF8);
            
            pexFile.Write(bw);
        }
    }
}
