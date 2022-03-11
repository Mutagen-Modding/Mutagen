using System.Collections;
using System.Collections.Generic;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

public static class BinaryOverlayArray2d
{
    public static IReadOnlyArray2d<T> Factory<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        P2Int size,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        return new BinaryOverlayArray2dTypical<T>(mem, package, itemLength, size, getter);
    }

    private class BinaryOverlayArray2dTypical<T> : IReadOnlyArray2d<T>
    {
        private readonly ReadOnlyMemorySlice<byte> _mem;
        private readonly BinaryOverlayFactoryPackage _package;
        private readonly int _itemLength;
        private readonly PluginBinaryOverlay.SpanFactory<T> _getter;

        public int Width { get; }
        public int Height { get; }

        public BinaryOverlayArray2dTypical(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            int itemLength,
            P2Int size,
            PluginBinaryOverlay.SpanFactory<T> getter)
        {
            _mem = mem;
            _package = package;
            _itemLength = itemLength;
            _getter = getter;
            Width = size.X;
            Height = size.Y;
        }

        public T this[int xIndex, int yIndex]
        {
            get
            {
                var loc = (Width * yIndex + xIndex) * _itemLength;
                return _getter(_mem.Slice(loc), _package);
            }
        }

        public T this[P2Int index] => this[index.X, index.Y];

        public IEnumerator<KeyValuePair<P2Int, T>> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return new KeyValuePair<P2Int, T>(new P2Int(x, y), this[x, y]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IArray2d<T> ShallowClone()
        {
            var ret = new Array2d<T>(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    ret[x, y] = this[x, y];
                }
            }

            return ret;
        }

        object IShallowCloneable.ShallowClone()
        {
            return ShallowClone();
        }
    }
}