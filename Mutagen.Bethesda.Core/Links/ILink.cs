using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for an object that is able to resolve against a LinkCache
    /// </summary>
    public interface ILink
    {
        /// <summary>
        /// The MajorRecord Type that the link is associated with
        /// </summary>
        Type TargetType { get; }
        
        /// <summary>
        /// Attempts to locate an associated ModKey from the link
        /// </summary>
        /// <param name="modKey">ModKey if found</param>
        /// <returns>True of ModKey information was located</returns>
        bool TryGetModKey([MaybeNullWhen(false)] out ModKey modKey);

        /// <summary>
        /// Attempts to locate an associated FormKey from the link
        /// </summary>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="formKey">FormKey if found</param>
        /// <returns>True if FormKey found</returns>
        bool TryResolveFormKey(ILinkCache package, out FormKey formKey);
        
        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="majorRecord">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        bool TryResolveCommon(ILinkCache package, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRecord);
    }

    /// <summary>
    /// An interface for an object that is able to resolve against a LinkCache
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface ILink<out TMajor> : ILink
        where TMajor : IMajorRecordCommonGetter
    {
        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="package">Link Cache to resolve against</param>
        /// <returns>TryGet object with located record if successful</returns>
        ITryGetter<TMajor> TryResolve(ILinkCache package);
    }
}
