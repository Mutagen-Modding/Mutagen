using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Used for specifying which records can be specified in a Linked Reference
    /// Implemented by: [Keyword]
    /// </summary>
    public interface IKeywordLinkedReference : IKeywordLinkedReferenceGetter, ILinkedReference
    {
    }

    /// <summary>
    /// Used for specifying which records can be specified in a Linked Reference
    /// Implemented by: [Keyword]
    /// </summary>
    public interface IKeywordLinkedReferenceGetter : ILinkedReferenceGetter
    {
    }
}
