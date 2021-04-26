using System;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda
{
    namespace Plugins.Aspects
    {
        /// <summary>
        /// An interface implemented by Major Records that have names
        /// </summary>
        public interface INamed : INamedGetter, INamedRequired
        {
            /// <summary>
            /// The display name of the record
            /// </summary>
            new String? Name { get; set; }
        }

        /// <summary>
        /// An interface implemented by Major Records that have names
        /// </summary>
        public interface INamedGetter : INamedRequiredGetter
        {
            /// <summary>
            /// The display name of the record
            /// </summary>
            new String? Name { get; }
        }
    }

    public static class INamedExt
    {
        /// <summary>
        /// Convenience method to check if either EditorID OR Name field contains a given string
        /// </summary>
        /// <param name="named">named record to check</param>
        /// <param name="str">String to check that either EditorID or Name contains</param>
        /// <returns>True if EITHER EditorID or Name contained string</returns>
        public static bool NamedFieldsContain<TMajor>(this TMajor named, string str)
            where TMajor : INamedGetter, IMajorRecordCommonGetter
        {
            if (named.EditorID?.Contains(str) ?? false) return true;
            if (named.Name?.Contains(str) ?? false) return true;
            return false;
        }

        /// <summary>
        /// Convenience method to check if either EditorID OR Name field contains a given string
        /// </summary>
        /// <param name="named">named record to check</param>
        /// <param name="str">String to check that either EditorID or Name contains</param>
        /// <param name="stringComparison">String comparison style to use</param>
        /// <returns>True if EITHER EditorID or Name contained string</returns>
        public static bool NamedFieldsContain<TMajor>(this TMajor named, string str, StringComparison stringComparison)
            where TMajor : INamedGetter, IMajorRecordCommonGetter
        {
            if (named.EditorID?.Contains(str, stringComparison) ?? false) return true;
            if (named.Name?.Contains(str, stringComparison) ?? false) return true;
            return false;
        }
    }
}
