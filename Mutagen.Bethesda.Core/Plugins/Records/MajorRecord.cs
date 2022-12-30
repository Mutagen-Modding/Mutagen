using Noggog;
using System.Diagnostics;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Records;

public partial interface IMajorRecord : IFormLinkContainer, IAssetLinkContainer, IMajorRecordQueryable
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
    IAssetLinkContainerGetter,
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
    String? IMajorRecordGetter.EditorID => EditorID;
    #endregion

    /// <summary>
    /// A convenience property to print "EditorID - FormKey"
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string TitleString => $"{EditorID} - {FormKey}";

    public bool IsCompressed
    {
        get => Enums.HasFlag(MajorRecordFlagsRaw, Constants.CompressedFlag);
        set => MajorRecordFlagsRaw = Enums.SetFlag(MajorRecordFlagsRaw, Constants.CompressedFlag, value);
    }

    public bool IsDeleted
    {
        get => Enums.HasFlag(MajorRecordFlagsRaw, Constants.DeletedFlag);
        set => MajorRecordFlagsRaw = Enums.SetFlag(MajorRecordFlagsRaw, Constants.DeletedFlag, value);
    }

    protected abstract ushort? FormVersionAbstract { get; }
    ushort? IMajorRecordGetter.FormVersion => FormVersionAbstract;
    ushort? IFormVersionGetter.FormVersion => FormVersionAbstract;

    public virtual bool Disable()
    {
        if (IsDeleted) return false;
        MajorRecordFlagsRaw = Enums.SetFlag(MajorRecordFlagsRaw, (int)Constants.InitiallyDisabled, true);
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
    public bool IsCompressed => Enums.HasFlag(MajorRecordFlagsRaw, Constants.CompressedFlag);
    public bool IsDeleted => Enums.HasFlag(MajorRecordFlagsRaw, Constants.DeletedFlag);

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