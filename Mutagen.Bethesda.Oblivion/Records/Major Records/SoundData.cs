using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Loqui;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
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

        public const int MinAttenuationDistanceMultiplier = 5;
        public const int MaxAttenuationDistanceMultiplier = 100;

        private static byte[] _marker = new byte[] { 1 };
        public virtual byte[] Marker => _marker;

        partial void CustomCtor()
        {
            this.MinimumAttenuationDistance_Property.Subscribe((change) =>
            {
                if (change.New % MinAttenuationDistanceMultiplier != 0)
                {
                    throw new ArgumentException($"{nameof(MinimumAttenuationDistance)} must be divisible by {MinAttenuationDistanceMultiplier}");
                }
            });
            this.MaximumAttenuationDistance_Property.Subscribe((change) =>
            {
                if (change.New % MaxAttenuationDistanceMultiplier != 0)
                {
                    throw new ArgumentException($"{nameof(MaximumAttenuationDistance)} must be divisible by {MaxAttenuationDistanceMultiplier}");
                }
            });
        }

        static partial void FillBinary_MinimumAttenuationDistance_Custom(MutagenFrame frame, SoundData item, int fieldIndex, Func<SoundData_ErrorMask> errorMask)
        {
            var ret = ByteBinaryTranslation.Instance.Parse(
                frame: frame.Spawn(snapToFinalPosition: false),
                fieldIndex: fieldIndex,
                errorMask: errorMask);
            if (ret.Succeeded)
            {
                item.MinimumAttenuationDistance = (ushort)(ret.Value * MinAttenuationDistanceMultiplier);
            }
        }

        static partial void WriteBinary_MinimumAttenuationDistance_Custom(MutagenWriter writer, SoundData item, int fieldIndex, Func<SoundData_ErrorMask> errorMask)
        {
            var val = (byte)(item.MinimumAttenuationDistance / MinAttenuationDistanceMultiplier);
            ByteBinaryTranslation.Instance.Write(
                writer: writer,
                item: val,
                fieldIndex: fieldIndex,
                errorMask: errorMask);
        }

        static partial void FillBinary_MaximumAttenuationDistance_Custom(MutagenFrame frame, SoundData item, int fieldIndex, Func<SoundData_ErrorMask> errorMask)
        {
            var ret = ByteBinaryTranslation.Instance.Parse(
                frame: frame.Spawn(snapToFinalPosition: false),
                fieldIndex: fieldIndex,
                errorMask: errorMask);
            if (ret.Succeeded)
            {
                item.MaximumAttenuationDistance = (ushort)(ret.Value * MaxAttenuationDistanceMultiplier);
            }
        }

        static partial void WriteBinary_MaximumAttenuationDistance_Custom(MutagenWriter writer, SoundData item, int fieldIndex, Func<SoundData_ErrorMask> errorMask)
        {
            var val = (byte)(item.MaximumAttenuationDistance / MaxAttenuationDistanceMultiplier);
            ByteBinaryTranslation.Instance.Write(
                writer: writer,
                item: val,
                fieldIndex: fieldIndex,
                errorMask: errorMask);
        }
    }
}
