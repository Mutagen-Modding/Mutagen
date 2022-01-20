using System;
using System.Data.SqlTypes;
using System.IO;

namespace Mutagen.Bethesda.Assets;

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLinkGetter<TAssetType> : IComparable<AssetLinkGetter<TAssetType>>, INullable, IAssetLinkGetter
    where TAssetType : IAssetType
{
    public AssetLinkGetter(string RawPath, IAssetType AssetType)
    {
        this.RawPath = RawPath;
        this.AssetType = AssetType;
    }

    public string RawPath { get; }
    public IAssetType AssetType { get; }

    public string DataRelativePath => Path.Combine(AssetType.BaseFolder, RawPath);

    public string Extension => Path.GetExtension(RawPath).TrimStart('.');

    public override string ToString()
    {
        return DataRelativePath;
    }

    public virtual bool Equals(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return DataRelativePath == other.DataRelativePath;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RawPath, AssetType);
    }

    public int CompareTo(AssetLinkGetter<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;

        return ReferenceEquals(null, other) ? 1 : string.Compare(RawPath, other.RawPath, StringComparison.Ordinal);
    }
    
    public bool IsNull => RawPath == string.Empty;
}

/// <summary>
/// Asset referenced by a record
/// </summary>
public class AssetLink<TAssetType> : AssetLinkGetter<TAssetType>, IComparable<AssetLink<TAssetType>>, IAssetLink
    where TAssetType : IAssetType
{
    public AssetLink(string RawPath, IAssetType AssetType) : base(RawPath, AssetType)
    {
        this.RawPath = RawPath;
        this.AssetType = AssetType;
    }

    public new string RawPath { get; set; }

    public new IAssetType AssetType { get; set; }

    public new string DataRelativePath => RawPath.StartsWith(AssetType.BaseFolder)
        ? RawPath
        : Path.Combine(AssetType.BaseFolder, RawPath);

    public new string Extension => Path.GetExtension(RawPath).TrimStart('.');

    public void SetTo(string? path)
    {
        RawPath = path ?? string.Empty;
    }

    public void SetToNull()
    {
        RawPath = string.Empty;
    }

    public override string ToString()
    {
        return DataRelativePath;
    }

    public virtual bool Equals(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return DataRelativePath == other.DataRelativePath;
    }

    public int CompareTo(AssetLink<TAssetType>? other)
    {
        if (ReferenceEquals(this, other)) return 0;

        return ReferenceEquals(null, other) ? 1 : string.Compare(RawPath, other.RawPath, StringComparison.Ordinal);
    }
}