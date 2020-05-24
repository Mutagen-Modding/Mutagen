using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Class to specify Strings file usage when importing a mod
    /// </summary>
    public class StringsReadParameters
    {
        /// <summary>
        /// If specified, normal string folder path locations will be ignored in favor of the path provided.
        /// </summary>
        public DirectoryPath? StringsFolderOverride;
    }
}
