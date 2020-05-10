using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that physical bounds
    /// </summary>
    public interface IObjectBounded : IObjectBoundedGetter, IObjectBoundedOptional
    {
        new ObjectBounds ObjectBounds { get; set; }
    }

    /// <summary>
    /// Common interface for records that physical bounds
    /// </summary>
    public interface IObjectBoundedGetter : IObjectBoundedOptionalGetter
    {
        new IObjectBoundsGetter? ObjectBounds { get; }
    }

    /// <summary>
    /// Common interface for records that physical bounds
    /// </summary>
    public interface IObjectBoundedOptional : IObjectBoundedOptionalGetter
    {
        new ObjectBounds? ObjectBounds { get; set; }
    }

    /// <summary>
    /// Common interface for records that physical bounds
    /// </summary>
    public interface IObjectBoundedOptionalGetter
    {
        IObjectBoundsGetter? ObjectBounds { get; }
    }
}
