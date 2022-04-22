using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// An interface for a FormLink.
/// FormKey is allowed to be null to communicate absence of a record.
/// </summary>
public interface IFormLinkIdentifier : IFormKeyGetter, ILinkIdentifier
{
}
    
/// <summary>
/// An interface for a FormLink.
/// FormKey is allowed to be null to communicate absence of a record.
/// </summary>
public interface IFormLinkGetter : ILink, IFormLinkIdentifier
{
    /// <summary>
    /// FormKey to link against
    /// </summary>
    FormKey? FormKeyNullable { get; }

    /// <summary>
    /// True if FormKey points to a null ID
    /// </summary>
    bool IsNull { get; }
}

/// <summary>
/// An interface for a FormLink, with a Major Record type constraint
/// </summary>
/// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
public interface IFormLinkGetter<out TMajorGetter> : ILink<TMajorGetter>, IFormLinkGetter
    where TMajorGetter : class, IMajorRecordGetter
{
    /// <summary>
    /// Creates a new FormLink with the given type, with the same FormKey.
    /// Does no safety checking to make sure the new target type is appropriate
    /// </summary>
    /// <typeparam name="TMajorRet">Type to cast FormLink to</typeparam>
    /// <returns>new FormLink with the given type, with the same FormKey</returns>
    IFormLink<TMajorRet> Cast<TMajorRet>()
        where TMajorRet : class, IMajorRecordGetter;
}

public interface IFormLink<out TMajorGetter> : IFormLinkGetter<TMajorGetter>, IClearable
    where TMajorGetter : class, IMajorRecordGetter
{
    /// <summary>
    /// FormKey to link against
    /// </summary>
    new FormKey? FormKeyNullable { get; set; }

    /// <summary>
    /// FormKey to link against
    /// </summary>
    new FormKey FormKey { get; set; }

    void SetTo(FormKey? formKey);

    void SetToNull();
}

/// <summary>
/// An interface for a FormLink, with a Major Record type constraint 
/// FormKey is allowed to be null to communicate absence of a record.
/// </summary>
/// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
public interface IFormLinkNullableGetter<out TMajorGetter> : ILink<TMajorGetter>, IFormLinkGetter, IFormLinkGetter<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    /// <summary>
    /// Creates a new FormLink with the given type, with the same FormKey.
    /// Does no safety checking to make sure the new target type is appropriate
    /// </summary>
    /// <typeparam name="TMajorRet">Type to cast FormLink to</typeparam>
    /// <returns>new FormLink with the given type, with the same FormKey</returns>
    new IFormLinkNullable<TMajorRet> Cast<TMajorRet>()
        where TMajorRet : class, IMajorRecordGetter;
}

public interface IFormLinkNullable<out TMajorGetter> : IFormLink<TMajorGetter>, IFormLinkNullableGetter<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
}