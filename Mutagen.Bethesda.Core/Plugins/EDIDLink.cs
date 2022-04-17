using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A specialized link for Oblivion Magic Effects, which use 4 character EditorIDs rather than FormIDs to link.
/// This class stores the target EDID as RecordType, as that is a convenient 4 character struct
/// </summary>
/// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
public class EDIDLink<TMajor> : IEDIDLink<TMajor>, IEquatable<IEDIDLink<TMajor>>
    where TMajor : class, IMajorRecordGetter
{
    /// <summary>
    /// A readonly singleton representing an unlinked EDIDLink
    /// </summary>
    public static readonly IEDIDLinkGetter<TMajor> Empty = new EDIDLink<TMajor>();
        
    /// <summary>
    /// A readonly singleton representing a "null" record type
    /// </summary>
    public static readonly RecordType Null = new RecordType("\0\0\0\0");
        
    /// <summary>
    /// Record type representing the target EditorID to link against
    /// </summary>
    public RecordType EDID { get; set; }
        
    Type ILinkIdentifier.Type => typeof(TMajor);

    public EDIDLink()
    {
        this.EDID = Null;
    }

    /// <summary>
    /// Default constructor that creates an EDIDLink linked to the target EditorID
    /// </summary>
    public EDIDLink(RecordType edid)
        : this()
    {
        this.EDID = edid;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not IEDIDLink<TMajor> rhs) return false;
        return this.Equals(rhs);
    }

    /// <summary>
    /// Compares equality of two links.
    /// </summary>
    /// <param name="other">Other link to compare to</param>
    /// <returns>True if EDID members are equal</returns>
    public bool Equals(IEDIDLink<TMajor>? other) => this.EDID.Equals(other?.EDID);

    /// <summary>
    /// Returns hash code
    /// </summary>
    /// <returns>Hash code evaluated from EDID member</returns>
    public override int GetHashCode() => this.EDID.GetHashCode();

    /// <summary>
    /// Returns string representation of link
    /// </summary>
    /// <returns>Returns EDID RecordType string</returns>
    public override string ToString() => this.EDID.ToString();

    private bool TryLinkToMod(
        IModGetter mod,
        [MaybeNullWhen(false)]out TMajor item)
    {
        if (this.EDID == Null)
        {
            item = default!;
            return false;
        }
        // ToDo
        // Improve to not be a forloop
        var group = mod.GetTopLevelGroup<TMajor>();
        foreach (var rec in group)
        {
            if (this.EDID.Type.Equals(rec.EditorID))
            {
                item = rec;
                return true;
            }
        }
        item = default!;
        return false;
    }

    /// <summary>
    /// Attempts to locate link target in given Link Cache.
    /// </summary>
    /// <param name="cache">Link Cache to resolve against</param>
    /// <param name="major">Located record if successful</param>
    /// <returns>True if link was resolved and a record was retrieved</returns>
    public bool TryResolve(ILinkCache cache, out TMajor major)
    {
        if (this.EDID == Null)
        {
            major = default!;
            return false;
        }
        foreach (var mod in cache.PriorityOrder)
        {
            if (TryLinkToMod(mod, out var item))
            {
                major = item;
                return true;
            }
        }
        major = default!;
        return false;
    }

    /// <summary>
    /// Attempts to locate link target's FormKey in given Link Cache.
    /// </summary>
    /// <param name="cache">Link Cache to resolve against</param>
    /// <param name="formKey">Located FormKey if successful</param>
    /// <returns>True if link was resolved and a record was retrieved</returns>
    public bool TryResolveFormKey(ILinkCache cache, [MaybeNullWhen(false)]out FormKey formKey)
    {
        if (TryResolve(cache, out var rec))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default!;
        return false;
    }

    bool ILink.TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)]out IMajorRecordGetter formKey)
    {
        if (TryResolve(cache, out TMajor rec))
        {
            formKey = rec;
            return true;
        }
        formKey = default!;
        return false;
    }

    /// <summary>
    /// Attempts to locate link target in given Link Cache.
    /// </summary>
    /// <param name="cache">Link Cache to resolve against</param>
    /// <returns>TryGet object with located record if successful</returns>
    public TMajor? TryResolve(ILinkCache cache) 
    {
        if (TryResolve(cache, out TMajor rec))
        {
            return rec;
        }
        return null;
    }

    bool ILink.TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
    {
        modKey = default!;
        return false;
    }

    public void SetTo(RecordType type)
    {
        this.EDID = type;
    }

    public void SetTo(IEDIDLinkGetter<TMajor> link)
    {
        this.EDID = link.EDID;
    }

    public void Clear()
    {
        this.EDID = Null;
    }

    public static implicit operator EDIDLink<TMajor>(RecordType recordType)
    {
        return new EDIDLink<TMajor>(recordType);
    }

    public static implicit operator EDIDLink<TMajor>(TMajor major)
    {
        if (major.EditorID == null)
        {
            return EDIDLink<TMajor>.Null;
        }
        else
        {
            return new EDIDLink<TMajor>(new RecordType(major.EditorID));
        }
    }
}