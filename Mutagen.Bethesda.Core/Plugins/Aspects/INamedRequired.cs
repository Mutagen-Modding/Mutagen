using System;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Aspects;

/// <summary>
/// An interface implemented by Major Records that have names
/// </summary>
public interface INamedRequired : INamedRequiredGetter, IMajorRecordQueryable
{
    /// <summary>
    /// The display name of the record
    /// </summary>
    new String Name { get; set; }
}

/// <summary>
/// An interface implemented by Major Records that have names
/// </summary>
public interface INamedRequiredGetter : IMajorRecordQueryableGetter
{
    /// <summary>
    /// The display name of the record
    /// </summary>
    String Name { get; }
}