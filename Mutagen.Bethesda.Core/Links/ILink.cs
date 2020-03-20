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
    public interface ILinkGetter
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
        /// <param name="formKey">FormKey if found</param>
        /// <returns>True if FormKey found</returns>
        /// <typeparam name="TMod">Mod type</typeparam>
        bool TryResolveFormKey<TMod>(ILinkCache<TMod> package, out FormKey formKey) where TMod : IModGetter;
        
        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="majorRecord">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TMod">Mod type</typeparam>
        bool TryResolveCommon<TMod>(ILinkCache<TMod> package, out IMajorRecordCommonGetter majorRecord) where TMod : IModGetter;
    }

    /// <summary>
    /// An interface for an object that is able to resolve against a LinkCache
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface ILinkGetter<out TMajor> : ILinkGetter
        where TMajor : IMajorRecordCommonGetter
    {
        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="package">Link Cache to resolve against</param>
        /// <returns>TryGet object with located record if successful</returns>
        /// <typeparam name="TMod">Mod type</typeparam>
        ITryGetter<TMajor> TryResolve<TMod>(ILinkCache<TMod> package) where TMod : IModGetter;
    }

    /// <summary>
    /// A static class with extension methods for ILink interfaces
    /// </summary>
    public static class ILinkExt
    {
        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="majorRecord">Major Record if located</param>
        /// <returns>True if successful in linking to record</returns>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMod, TMajor>(this ILinkGetter<TMajor> link, ILinkCache<TMod> package, out TMajor majorRecord)
            where TMod : IModGetter
            where TMajor : IMajorRecordCommonGetter
        {
            var ret = link.TryResolve<TMod>(package);
            if (ret.Succeeded)
            {
                majorRecord = ret.Value;
                return true;
            }
            majorRecord = default;
            return false;
        }

        /// <summary>
        /// Locates link target in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="package">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static TMajor Resolve<TMod, TMajor>(this ILinkGetter<TMajor> link, ILinkCache<TMod> package)
            where TMod : IModGetter
            where TMajor : IMajorRecordCommonGetter
        {
            return link.TryResolve<TMod>(package).Value;
        }
    }
}
