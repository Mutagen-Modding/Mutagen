using Noggog;

namespace Mutagen.Bethesda.Archives.Exceptions;

public class ArchiveException : Exception
{
    public FilePath? ArchiveFilePath { get; set; }
    public string? InternalFolderAccessed { get; set; }
    public string? InternalFileAccessed { get; set; }


    public ArchiveException(FilePath? archiveFilePath, string? folderAccessed, string? fileAccessed)
    {
        ArchiveFilePath = archiveFilePath;
        InternalFolderAccessed = folderAccessed;
        InternalFileAccessed = fileAccessed;
    }

    public ArchiveException(FilePath? archiveFilePath, string? folderAccessed, string? fileAccessed, string message)
        : base(message)
    {
        ArchiveFilePath = archiveFilePath;
        InternalFolderAccessed = folderAccessed;
        InternalFileAccessed = fileAccessed;
    }

    public ArchiveException(FilePath? archiveFilePath, string? folderAccessed, string? fileAccessed, string message, Exception? innerException)
        : base(message, innerException)
    {
        ArchiveFilePath = archiveFilePath;
        InternalFolderAccessed = folderAccessed;
        InternalFileAccessed = fileAccessed;
    }

    public ArchiveException(FilePath? archiveFilePath, string? folderAccessed, string? fileAccessed, Exception innerException)
        : base(innerException.Message, innerException)
    {
        ArchiveFilePath = archiveFilePath;
        InternalFolderAccessed = folderAccessed;
        InternalFileAccessed = fileAccessed;
    }

    #region Enrich

    public static ArchiveException EnrichWithFileAccessed(Exception ex, string fileAccessed)
    {
        if (ex is ArchiveException archiveException)
        {
            archiveException.InternalFileAccessed = fileAccessed;
            return archiveException;
        }

        return new ArchiveException(
            archiveFilePath: null,
            folderAccessed: null,
            fileAccessed: fileAccessed,
            innerException: ex);
    }

    public static ArchiveException EnrichWithFileAccessed(string message, Exception ex, string fileAccessed)
    {
        if (ex is ArchiveException archiveException)
        {
            archiveException.InternalFileAccessed = fileAccessed;
            return archiveException;
        }

        return new ArchiveException(
            archiveFilePath: null,
            folderAccessed: null,
            fileAccessed: fileAccessed,
            message: message,
            innerException: ex);
    }

    public static ArchiveException EnrichWithFolderAccessed(Exception ex, string folderAccessed)
    {
        if (ex is ArchiveException archiveException)
        {
            archiveException.InternalFolderAccessed = folderAccessed;
            return archiveException;
        }

        return new ArchiveException(
            archiveFilePath: null,
            folderAccessed: folderAccessed,
            fileAccessed: null,
            innerException: ex);
    }

    public static ArchiveException EnrichWithFolderAccessed(string message, Exception ex, string folderAccessed)
    {
        if (ex is ArchiveException archiveException)
        {
            archiveException.InternalFolderAccessed = folderAccessed;
            return archiveException;
        }

        return new ArchiveException(
            archiveFilePath: null,
            folderAccessed: folderAccessed,
            fileAccessed: null,
            message: message,
            innerException: ex);
    }

    public static ArchiveException EnrichWithArchivePath(Exception ex, FilePath path)
    {
        if (ex is ArchiveException archiveException)
        {
            archiveException.ArchiveFilePath = path;
            return archiveException;
        }

        return new ArchiveException(
            archiveFilePath: path,
            folderAccessed: null,
            fileAccessed: null,
            innerException: ex);
    }

    public static ArchiveException EnrichWithArchivePath(string message, Exception ex, FilePath path)
    {
        if (ex is ArchiveException archiveException)
        {
            archiveException.ArchiveFilePath = path;
            return archiveException;
        }

        return new ArchiveException(
            archiveFilePath: path,
            folderAccessed: null,
            fileAccessed: null,
            message: message,
            innerException: ex);
    }

    public static ArchiveException Enrich(Exception ex, FilePath? archiveFilePath, string? folderAccessed, string? fileAccessed)
    {
        if (ex is ArchiveException archiveException)
        {
            if (archiveFilePath != null)
            {
                archiveException.ArchiveFilePath = archiveFilePath;
            }
            if (folderAccessed != null)
            {
                archiveException.InternalFolderAccessed = folderAccessed;
            }
            if (fileAccessed != null)
            {
                archiveException.InternalFileAccessed = fileAccessed;
            }
            return archiveException;
        }

        return new ArchiveException(
            archiveFilePath: archiveFilePath,
            folderAccessed: folderAccessed,
            fileAccessed: fileAccessed,
            innerException: ex);
    }

    public static ArchiveException Enrich(string message, Exception ex, FilePath? archiveFilePath, string? folderAccessed, string? fileAccessed)
    {
        if (ex is ArchiveException archiveException)
        {
            if (archiveFilePath != null)
            {
                archiveException.ArchiveFilePath = archiveFilePath;
            }
            if (folderAccessed != null)
            {
                archiveException.InternalFolderAccessed = folderAccessed;
            }
            if (fileAccessed != null)
            {
                archiveException.InternalFileAccessed = fileAccessed;
            }
            return archiveException;
        }

        return new ArchiveException(
            archiveFilePath: archiveFilePath,
            folderAccessed: folderAccessed,
            fileAccessed: fileAccessed,
            message: message,
            innerException: ex);
    }

    #endregion

    public override string ToString()
    {
        return $"{nameof(ArchiveException)} {ArchiveFilePath}{InternalFolderAccessed?.Decorate(x => $"=>{x}")}{InternalFileAccessed?.Decorate(x => $"=>{x}")}: {this.Message} {this.InnerException}{this.StackTrace}";
    }
}