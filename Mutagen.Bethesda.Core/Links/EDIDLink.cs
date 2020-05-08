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
    /// A specialized link for Oblivion Magic Effects, which use 4 character EditorIDs rather than FormIDs to link.
    /// This class stores the target EDID as RecordType, as that is a convenient 4 character struct
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public struct EDIDLink<TMajor> : IEDIDLink<TMajor>, IEquatable<IEDIDLinkGetter<TMajor>>
       where TMajor : class, IMajorRecordCommonGetter
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
        
        Type ILinkGetter.TargetType => typeof(TMajor);

        /// <summary>
        /// Default constructor that creates an EDIDLink linked to the target EditorID
        /// </summary>
        public EDIDLink(RecordType edid)
            : this()
        {
            this.EDID = edid;
        }

        /// <summary>
        /// Sets the link to the target EditorID
        /// </summary>
        /// <param name="type">Target EditorID to link to</param>
        public void Set(RecordType type)
        {
            this.EDID = type;
        }

        /// <summary>
        /// Sets the link to the target Major Record
        /// </summary>
        /// <param name="value">Target record to link to</param>
        /// <exception cref="ArgumentException">If EditorID of target record is not 4 characters</exception>
        public void Set(TMajor? value)
        {
            if (value?.EditorID == null)
            {
                this.EDID = Null;
            }
            else
            {
                this.Set(new RecordType(value.EditorID));
            }
        }

        /// <summary>
        /// Compares equality of two links.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if EDID members are equal</returns>
        public bool Equals(IEDIDLinkGetter<TMajor> other) => this.EDID.Equals(other.EDID);

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
            var group = mod.GetGroupGetter<TMajor>();
            foreach (var rec in group.Items)
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
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        public bool TryResolve(ILinkCache package, out TMajor major)
        {
            if (this.EDID == Null)
            {
                major = default!;
                return false;
            }
            foreach (var mod in package)
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
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located FormKey if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        public bool TryResolveFormKey(ILinkCache package, [MaybeNullWhen(false)]out FormKey formKey)
        {
            if (TryResolve(package, out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }
            formKey = default!;
            return false;
        }

        bool ILinkGetter.TryResolveCommon(ILinkCache package, [MaybeNullWhen(false)]out IMajorRecordCommonGetter formKey)
        {
            if (TryResolve(package, out TMajor rec))
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
        /// <param name="package">Link Cache to resolve against</param>
        /// <returns>TryGet object with located record if successful</returns>
        public ITryGetter<TMajor> TryResolve(ILinkCache package) 
        {
            if (TryResolve(package, out TMajor rec))
            {
                return TryGet<TMajor>.Succeed(rec);
            }
            return TryGet<TMajor>.Failure;
        }

        /// <summary>
        /// Resets to an unlinked state
        /// </summary>
        public void Unset()
        {
            this.EDID = Null;
        }

        bool ILinkGetter.TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
        {
            modKey = default!;
            return false;
        }

        public static implicit operator EDIDLink<TMajor>(RecordType recordType)
        {
            return new EDIDLink<TMajor>(recordType);
        }
    }
}
