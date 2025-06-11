using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache;

public interface IReferencedByCache
{
    IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> GetReferencedBy<TReferencedBy>(
        IFormLinkIdentifier identifier)
        where TReferencedBy : class, IMajorRecordGetter;

    IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> GetReferencedBy<TReferencedBy>(
        IMajorRecordGetter majorRecord)
        where TReferencedBy : class, IMajorRecordGetter;

    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    IReadOnlyCollection<FormKey> GetReferencedBy(FormKey formKey);
}