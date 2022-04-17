using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Records;

public partial interface IMajorRecord : IFormLinkContainer, IMajorRecordQueryable
{
    new FormKey FormKey { get; }
        
    /// <summary>
    /// Marker of whether the content is compressed
    /// </summary>
    new bool IsCompressed { get; set; }

    /// <summary>
    /// Marker of whether the content is deleted
    /// </summary>
    new bool IsDeleted { get; set; }

    /// <summary>
    /// Disables the record by setting the RecordFlag to Initially Disabled.
    /// <returns>Returns true if the disable was successful.</returns>
    /// </summary>
    bool Disable();
}
    
public partial interface IMajorRecordInternal
{
    new FormKey FormKey { get; set; }
}

public partial interface IMajorRecordGetter : 
    IFormVersionGetter, 
    IMajorRecordIdentifier,
    IFormLinkContainerGetter,
    IFormLinkIdentifier,
    IEquatable<IFormLinkGetter>,
    IMajorRecordQueryableGetter
{
    /// <summary>
    /// Marker of whether the content is compressed
    /// </summary>
    bool IsCompressed { get; }

    /// <summary>
    /// Marker of whether the content is deleted
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Form Version of the record
    /// </summary>
    new ushort? FormVersion { get; }
}

[DebuggerDisplay("{GetType().Name} {this.EditorID?.ToString()} {this.FormKey.ToString()}")]
public partial class MajorRecord : IFormLinkContainer
{
    #region EditorID
    public virtual String? EditorID { get; set; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    String? IMajorRecordGetter.EditorID => this.EditorID;
    #endregion

    /// <summary>
    /// A convenience property to print "EditorID - FormKey"
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string TitleString => $"{this.EditorID} - {this.FormKey}";

    public bool IsCompressed
    {
        get => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Constants.CompressedFlag);
        set => this.MajorRecordFlagsRaw = EnumExt.SetFlag(this.MajorRecordFlagsRaw, Constants.CompressedFlag, value);
    }

    public bool IsDeleted
    {
        get => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Constants.DeletedFlag);
        set => this.MajorRecordFlagsRaw = EnumExt.SetFlag(this.MajorRecordFlagsRaw, Constants.DeletedFlag, value);
    }

    protected abstract ushort? FormVersionAbstract { get; }
    ushort? IMajorRecordGetter.FormVersion => FormVersionAbstract;
    ushort? IFormVersionGetter.FormVersion => FormVersionAbstract;

    public virtual bool Disable()
    {
        if (this.IsDeleted) return false;
        MajorRecordFlagsRaw = EnumExt.SetFlag(MajorRecordFlagsRaw, (int)Constants.InitiallyDisabled, true);
        return true;
    }

    #region Comparers
    public static IEqualityComparer<IMajorRecordGetter> FormKeyEqualityComparer => _formKeyEqualityComparer;

    private static readonly MajorRecordFormKeyComparer _formKeyEqualityComparer = new();

    class MajorRecordFormKeyComparer : IEqualityComparer<IMajorRecordGetter>
    {
        public bool Equals(IMajorRecordGetter? x, IMajorRecordGetter? y)
        {
            return x?.FormKey == y?.FormKey;
        }

        public int GetHashCode(IMajorRecordGetter obj)
        {
            return obj.FormKey.GetHashCode();
        }
    }
    #endregion

    public bool Equals(IFormLinkGetter? other)
    {
        if (other == null) return false;
        return other.Equals(this);
    }

    Type ILinkIdentifier.Type => LinkType;
    protected abstract Type LinkType { get; }
}

public static class IMajorRecordGetterExt
{
    [Obsolete("Major records implement IFormLinkIdentifier which should be used instead")]
    public static IFormLinkGetter ToFormLinkInformation(this IMajorRecordGetter majorRec)
    {
        return FormLinkInformation.Factory(majorRec);
    }
}
    
[DebuggerDisplay("{GetType().Name} {this.EditorID?.ToString()} {this.FormKey.ToString()}")]
internal abstract partial class MajorRecordBinaryOverlay : IMajorRecordGetter
{
    public bool IsCompressed => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Constants.CompressedFlag);
    public bool IsDeleted => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Constants.DeletedFlag);

    protected abstract ushort? FormVersionAbstract { get; }
    ushort? IMajorRecordGetter.FormVersion => FormVersionAbstract;
    ushort? IFormVersionGetter.FormVersion => FormVersionAbstract;

    Type ILinkIdentifier.Type => LinkType;
    protected abstract Type LinkType { get; }

    public bool Equals(IFormLinkGetter? other)
    {
        if (other == null) return false;
        return other.Equals(this);
    }
}