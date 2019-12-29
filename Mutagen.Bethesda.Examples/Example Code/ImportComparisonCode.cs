using Mutagen.Bethesda.Oblivion;
using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    /// This code will import the file in various ways, with the goal of only importing
    /// and processing NPCs.  Each snippet is set up to exclude other types of records.
    public static class ImportComparisonCode
    {
        /// This route will do minimal processing up front.  It will parse records as you access fields in the object.
        /// As such, it does not need any hints of what types of records you are interested in.
        /// 
        /// Downsides to this method is that each field is re-parsed on every access, which may be wasted work if you're
        /// accessing the same field multitudes of times.  IF you're accessing any given field only once, or even just a few
        /// times, this does not matter, and will be the fastest route.
        public static IOblivionModGetter ImportViaBinaryOverlay(string pathToMod)
        {
            return OblivionMod.CreateFromBinaryOverlay(pathToMod);
        }

        /// This route will load a mod's contents, and fill an object's fields in memory with the parsed content.
        /// As such, it needs a hint of what you're interested in importing, so it can skip unrelated record types.
        /// 
        /// Downsides to this method is that all the records need to live in memory as parsed and filled objects.  This can
        /// be a surprising amount in some cases.  Unless the fields are going to be accessed many times each, this import
        /// method should be avoided.
        public static async Task<IOblivionModGetter> ImportIntoInMemoryObject(string pathToMod)
        {
            return await OblivionMod.CreateFromBinary(
                pathToMod,
                importMask: new GroupMask()
                {
                    NPCs = true
                });
        }
    }
}
