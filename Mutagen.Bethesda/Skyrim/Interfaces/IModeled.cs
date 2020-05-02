using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that have a model
    /// </summary>
    public interface IModeled : IModeledGetter
    {
        new Model? Model { get; set; }
    }

    /// <summary>
    /// Common interface for records that have a model
    /// </summary>
    public interface IModeledGetter
    {
        IModelGetter? Model { get; }
    }
}
