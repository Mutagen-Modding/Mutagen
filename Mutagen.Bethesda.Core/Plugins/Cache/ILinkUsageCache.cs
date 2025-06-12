using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache;

public interface ILinkUsageCache
{
    IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> GetUsagesOf<TReferencedBy>(
        IFormLinkIdentifier identifier)
        where TReferencedBy : class, IMajorRecordGetter;
    
    IReadOnlyCollection<FormKey> GetUsagesOf(
        IFormLinkIdentifier identifier);

    IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> GetUsagesOf<TReferencedBy>(
        IMajorRecordGetter majorRecord)
        where TReferencedBy : class, IMajorRecordGetter;

    IReadOnlyCollection<FormKey> GetUsagesOf(
        IMajorRecordGetter majorRecord);

    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    IReadOnlyCollection<FormKey> GetUsagesOf(FormKey formKey);
}