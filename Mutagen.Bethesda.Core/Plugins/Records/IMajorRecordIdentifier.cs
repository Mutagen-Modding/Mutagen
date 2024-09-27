namespace Mutagen.Bethesda.Plugins.Records;

public interface IMajorRecordIdentifier : IFormKeyGetter
{
    /// <summary>
    /// The usually unique string identifier assigned to the Major Record
    /// </summary>
    string? EditorID { get; }
}

public record MajorRecordIdentifier : IMajorRecordIdentifier
{
    public required FormKey FormKey { get; init; }
    public string? EditorID { get; init; }
    
    public virtual bool Equals(MajorRecordIdentifier? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FormKey.Equals(other.FormKey)
               && string.Equals(EditorID, other.EditorID, StringComparison.OrdinalIgnoreCase);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(FormKey, EditorID?.GetHashCode(StringComparison.OrdinalIgnoreCase));
    }
    
    public static IEqualityComparer<IMajorRecordIdentifier> EqualityComparer => _equalityComparer;

    private static readonly Comparer _equalityComparer = new();

    class Comparer : IEqualityComparer<IMajorRecordIdentifier>
    {
        public bool Equals(IMajorRecordIdentifier? x, IMajorRecordIdentifier? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.FormKey.Equals(y.FormKey)
                   && string.Equals(x.EditorID, y.EditorID, StringComparison.OrdinalIgnoreCase);
        }
        
        public int GetHashCode(IMajorRecordIdentifier obj)
        {
            return HashCode.Combine(obj.FormKey, obj.EditorID?.GetHashCode(StringComparison.OrdinalIgnoreCase));
        }
    }
}

public interface IMajorRecordIdentifier<TMajorGetter> : IMajorRecordIdentifier
    where TMajorGetter : class, IMajorRecordQueryableGetter
{
}

public record MajorRecordIdentifier<TMajorGetter> : MajorRecordIdentifier, IMajorRecordIdentifier<TMajorGetter>
    where TMajorGetter : class, IMajorRecordQueryableGetter
{
}