using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mutagen.Bethesda.Binary;
using Loqui.Internal;
using ReactiveUI;
using System.Reactive.Linq;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial interface ISoundDataInternalGetter
    {
        ReadOnlySpan<byte> Marker { get; }
    }

    public partial class SoundData
    {
        [Flags]
        public enum Flag
        {
            RandomFrequencyShift = 0x01,
            PlayAtRandom = 0x02,
            EnvironmentIgnored = 0x04,
            RandomLocation = 0x08,
            Loop = 0x10,
            MenuSound = 0x20,
            TwoDimensional = 0x40,
            LFE360 = 0x80,
        }

        #region MinimumAttenuationDistance
        private UInt16 _MinimumAttenuationDistance;
        public UInt16 MinimumAttenuationDistance
        {
            get => _MinimumAttenuationDistance;
            set
            {
                if (value % MinAttenuationDistanceMultiplier != 0)
                {
                    throw new ArgumentException($"{nameof(MinimumAttenuationDistance)} must be divisible by {MinAttenuationDistanceMultiplier}");
                }
                _MinimumAttenuationDistance = value;
            }
        }
        #endregion
        #region MaximumAttenuationDistance
        private UInt16 _MaximumAttenuationDistance;
        public UInt16 MaximumAttenuationDistance
        {
            get => _MaximumAttenuationDistance;
            set
            {
                if (value % MaxAttenuationDistanceMultiplier != 0)
                {
                    throw new ArgumentException($"{nameof(MaximumAttenuationDistance)} must be divisible by {MaxAttenuationDistanceMultiplier}");
                }
                _MaximumAttenuationDistance = value;
            }
        }
        #endregion

        public const int MinAttenuationDistanceMultiplier = 5;
        public const int MaxAttenuationDistanceMultiplier = 100;

        private static byte[] _marker = new byte[] { 1 };
        public static ReadOnlySpan<byte> SoundDataMarker => _marker;
        public virtual ReadOnlySpan<byte> Marker => SoundDataMarker;
    }

    namespace Internals
    {
        public partial class SoundDataBinaryWriteTranslation
        {
            static partial void WriteBinaryMinimumAttenuationDistanceCustom(MutagenWriter writer, ISoundDataInternalGetter item, MasterReferences masterReferences)
            {
                var val = (byte)(item.MinimumAttenuationDistance / SoundData.MinAttenuationDistanceMultiplier);
                ByteBinaryTranslation.Instance.Write(
                    writer: writer,
                    item: val);
            }

            static partial void WriteBinaryMaximumAttenuationDistanceCustom(MutagenWriter writer, ISoundDataInternalGetter item, MasterReferences masterReferences)
            {
                var val = (byte)(item.MaximumAttenuationDistance / SoundData.MaxAttenuationDistanceMultiplier);
                ByteBinaryTranslation.Instance.Write(
                    writer: writer,
                    item: val);
            }
        }

        public partial class SoundDataBinaryCreateTranslation
        {
            static partial void FillBinaryMinimumAttenuationDistanceCustom(MutagenFrame frame, ISoundDataInternal item, MasterReferences masterReferences)
            {
                if (ByteBinaryTranslation.Instance.Parse(
                    frame: frame,
                    item: out var b))
                {
                    item.MinimumAttenuationDistance = (ushort)(b * SoundData.MinAttenuationDistanceMultiplier);
                }
            }

            static partial void FillBinaryMaximumAttenuationDistanceCustom(MutagenFrame frame, ISoundDataInternal item, MasterReferences masterReferences)
            {
                if (ByteBinaryTranslation.Instance.Parse(
                    frame: frame,
                    item: out var b))
                {
                    item.MaximumAttenuationDistance = (ushort)(b * SoundData.MaxAttenuationDistanceMultiplier);
                }
            }
        }

        public partial class SoundDataBinaryOverlay
        {
            public virtual ReadOnlySpan<byte> Marker => SoundData.SoundDataMarker;

            public ushort GetMinimumAttenuationDistanceCustom(int location)
            {
                return (ushort)(_data.Span[location] * SoundData.MinAttenuationDistanceMultiplier);
            }

            public ushort GetMaximumAttenuationDistanceCustom(int location)
            {
                return (ushort)(_data.Span[location] * SoundData.MaxAttenuationDistanceMultiplier);
            }
        }
    }
}
