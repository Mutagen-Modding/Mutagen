using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class BsaException : Exception
    {
        public string? BsaFilePath { get; set; }
        public string? InternalFolderAccessed { get; set; }
        public string? InternalFileAccessed { get; set; }

        private BsaException(string message, Exception ex, string? bsaFilePath, string? folderAccessed, string? fileAccessed)
            : base(message, ex)
        {
            BsaFilePath = bsaFilePath;
            InternalFolderAccessed = folderAccessed;
            InternalFileAccessed = fileAccessed;
        }

        public static BsaException FileError(string message, Exception ex, string bsaFilePath, string fileAccessed)
        {
            if (ex is BsaException bsa) return bsa;
            return new BsaException(
                message,
                ex,
                bsaFilePath: bsaFilePath,
                folderAccessed: null,
                fileAccessed: fileAccessed);
        }

        public static BsaException FolderError(string message, Exception ex, string bsaFilePath, string folderAccessed)
        {
            if (ex is BsaException bsa) return bsa;
            return new BsaException(
                message,
                ex,
                bsaFilePath: bsaFilePath,
                folderAccessed: folderAccessed,
                fileAccessed: null);
        }

        public static BsaException OverallError(string message, Exception ex, string bsaFilePath)
        {
            if (ex is BsaException bsa) return bsa;
            return new BsaException(
                message,
                ex,
                bsaFilePath: bsaFilePath,
                folderAccessed: null,
                fileAccessed: null);
        }

        public override string ToString()
        {
            return $"{nameof(BsaException)} {BsaFilePath}{InternalFolderAccessed.Decorate(x => $"=>{x}")}{InternalFileAccessed.Decorate(x => $"=>{x}")}: {this.Message} {this.InnerException}{this.StackTrace}";
        }
    }
}
