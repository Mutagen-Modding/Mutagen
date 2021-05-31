using Noggog;
using System;

namespace Mutagen.Bethesda.Archives.Exceptions
{
    public class ArchiveException : Exception
    {
        public FilePath? ArchiveFilePath { get; set; }
        public DirectoryPath? InternalFolderAccessed { get; set; }
        public FilePath? InternalFileAccessed { get; set; }

        private ArchiveException(string message, Exception ex, FilePath? bsaFilePath, DirectoryPath? folderAccessed, FilePath? fileAccessed)
            : base(message, ex)
        {
            ArchiveFilePath = bsaFilePath;
            InternalFolderAccessed = folderAccessed;
            InternalFileAccessed = fileAccessed;
        }

        public static ArchiveException FileError(string message, Exception ex, FilePath bsaFilePath, FilePath fileAccessed)
        {
            if (ex is ArchiveException bsa) return bsa;
            return new ArchiveException(
                message,
                ex,
                bsaFilePath: bsaFilePath,
                folderAccessed: null,
                fileAccessed: fileAccessed);
        }

        public static ArchiveException FolderError(string message, Exception ex, FilePath bsaFilePath, DirectoryPath folderAccessed)
        {
            if (ex is ArchiveException bsa) return bsa;
            return new ArchiveException(
                message,
                ex,
                bsaFilePath: bsaFilePath,
                folderAccessed: folderAccessed,
                fileAccessed: null);
        }

        public static ArchiveException OverallError(string message, Exception ex, FilePath bsaFilePath)
        {
            if (ex is ArchiveException bsa) return bsa;
            return new ArchiveException(
                message,
                ex,
                bsaFilePath: bsaFilePath,
                folderAccessed: null,
                fileAccessed: null);
        }

        public override string ToString()
        {
            return $"{nameof(ArchiveException)} {ArchiveFilePath}{InternalFolderAccessed?.Path.Decorate(x => $"=>{x}")}{InternalFileAccessed?.Path.Decorate(x => $"=>{x}")}: {this.Message} {this.InnerException}{this.StackTrace}";
        }
    }
}
