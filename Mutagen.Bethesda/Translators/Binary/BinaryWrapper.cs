using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public abstract class BinaryWrapper
    {
        protected ReadOnlyMemorySlice<byte> _data;
        protected BinaryWrapperFactoryPackage _package;

        protected BinaryWrapper(
            ReadOnlyMemorySlice<byte> bytes,
            BinaryWrapperFactoryPackage package)
        {
            this._data = bytes;
            this._package = package;
        }
    }
}
