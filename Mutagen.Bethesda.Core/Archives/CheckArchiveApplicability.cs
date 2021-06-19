using System;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Archives
{
    public interface ICheckArchiveApplicability
    {
        /// <summary>
        /// Analyzes whether an Archive would typically apply to a given ModKey. <br />
        ///  <br />
        /// - Is extension of the proper type <br />
        /// - Does the name match <br />
        /// - Does the name match, with an extra ` - AssetType` suffix considered
        /// </summary>
        /// <param name="release">Game Release of mod</param>
        /// <param name="modKey">ModKey to check applicability for</param>
        /// <param name="archiveFileName">Filename of the Archive, with extension</param>
        /// <returns>True if Archive is typically applicable to the given ModKey</returns>
        bool IsApplicable(GameRelease release, ModKey modKey, FileName archiveFileName);
    }

    public class CheckArchiveApplicability : ICheckArchiveApplicability
    {

        /// <inheritdoc />
        public bool IsApplicable(GameRelease release, ModKey modKey, FileName archiveFileName)
        {
            if (!archiveFileName.Extension.Equals(Archive.GetExtension(release), StringComparison.OrdinalIgnoreCase)) return false;
            var nameWithoutExt = archiveFileName.NameWithoutExtension.AsSpan();

            // See if the name matches straight up
            if (modKey.Name.AsSpan().Equals(nameWithoutExt, StringComparison.OrdinalIgnoreCase)) return true;

            // Trim ending "type" information and try again
            var delimIndex = nameWithoutExt.LastIndexOf(" - ");
            if (delimIndex == -1) return false;

            return modKey.Name.AsSpan().Equals(nameWithoutExt.Slice(0, delimIndex), StringComparison.OrdinalIgnoreCase);
        }
    }
}