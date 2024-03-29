﻿namespace Mutagen.Bethesda.Plugins.Records;

/// <summary>
/// An interface for classes that contain FormKeys and can enumerate them.
/// </summary>
public interface IFormLinkContainer : IFormLinkContainerGetter
{
    /// <summary>
    /// Swaps out all links to point to new FormKeys
    /// </summary>
    void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping);
}

/// <summary>
/// An interface for classes that contain FormKeys and can enumerate them.
/// </summary>
public interface IFormLinkContainerGetter
{
    /// <summary>
    /// Enumerate of all contained FormKeys within object and subobjects
    /// </summary>
    IEnumerable<IFormLinkGetter> EnumerateFormLinks();
}