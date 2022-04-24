using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins;

public interface ILinkIdentifier
{
    /// <summary>
    /// The MajorRecord Type that the link is associated with
    /// </summary>
    Type Type { get; }
}
    
/// <summary>
/// An interface for an object that is able to resolve against a LinkCache
/// </summary>
public interface ILink : ILinkIdentifier
{
    /// <summary>
    /// Attempts to locate an associated ModKey from the link
    /// </summary>
    /// <param name="modKey">ModKey if found</param>
    /// <returns>True of ModKey information was located</returns>
    bool TryGetModKey([MaybeNullWhen(false)] out ModKey modKey);

    /// <summary>
    /// Attempts to locate an associated FormKey from the link
    /// </summary>
    /// <param name="cache">Link Cache to resolve against</param>
    /// <param name="formKey">FormKey if found</param>
    /// <returns>True if FormKey found</returns>
    bool TryResolveFormKey(ILinkCache cache, out FormKey formKey);
        
    /// <summary>
    /// Attempts to locate link target in given Link Cache.
    /// </summary>
    /// <param name="cache">Link Cache to resolve against</param>
    /// <param name="majorRecord">Located record if successful</param>
    /// <returns>True if link was resolved and a record was retrieved</returns>
    bool TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)] out IMajorRecordGetter majorRecord);
}

/// <summary>
/// An interface for an object that is able to resolve against a LinkCache
/// </summary>
/// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
public interface ILink<out TMajor> : ILink
    where TMajor : IMajorRecordGetter
{
    /// <summary>
    /// Attempts to locate link target in given Link Cache.
    /// </summary>
    /// <param name="cache">Link Cache to resolve against</param>
    /// <returns>TryGet object with located record if successful</returns>
    TMajor? TryResolve(ILinkCache cache);
}