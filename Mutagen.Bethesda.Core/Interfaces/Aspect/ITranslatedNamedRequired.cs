using Mutagen.Bethesda.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface implemented by Major Records that have names
    /// </summary>
    public interface ITranslatedNamedRequired : ITranslatedNamedRequiredGetter, INamedRequired
    {
        /// <summary>
        /// The display name of the record
        /// </summary>
        new TranslatedString Name { get; set; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have names
    /// </summary>
    public interface ITranslatedNamedRequiredGetter : INamedRequiredGetter
    {
        /// <summary>
        /// The display name of the record
        /// </summary>
        new ITranslatedStringGetter Name { get; }
    }
}
