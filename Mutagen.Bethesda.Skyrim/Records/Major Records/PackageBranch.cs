using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class PackageBranch
    {
        [Flags]
        public enum Flag
        {
            SuccessCompletesPackage = 0x01
        }
    }

    namespace Internals
    {
        public partial class PackageBranchBinaryCreateTranslation
        {
            public static partial void FillBinaryFlagsOverrideCustom(MutagenFrame frame, IPackageBranch item)
            {
                item.FlagsOverride = PackageFlagsOverride.CreateFromBinary(frame);
                if (frame.Reader.TryGetSubrecord(RecordTypes.PFO2, out var rec))
                {
                    item.FlagsOverrideUnused = PackageFlagsOverride.CreateFromBinary(frame);
                }
            }
        }

        public partial class PackageBranchBinaryWriteTranslation
        {
            public static partial void WriteBinaryFlagsOverrideCustom(MutagenWriter writer, IPackageBranchGetter item)
            {
                item.FlagsOverride?.WriteToBinary(writer);
                item.FlagsOverrideUnused?.WriteToBinary(writer);
            }
        }

        public partial class PackageBranchBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            public IPackageFlagsOverrideGetter? FlagsOverrideUnused { get; private set; }

            private IPackageFlagsOverrideGetter? _flagsOverride;
            public IPackageFlagsOverrideGetter? GetFlagsOverrideCustom() => _flagsOverride;

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayCountedList(stream, _package);
            }

            partial void FlagsOverrideCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _flagsOverride = PackageFlagsOverride.CreateFromBinary(new MutagenFrame(
                    new MutagenInterfaceReadStream(stream, _package.MetaData)));
                if (stream.TryGetSubrecord(RecordTypes.PFO2, out var rec))
                {
                    FlagsOverrideUnused = PackageFlagsOverride.CreateFromBinary(new MutagenFrame(
                        new MutagenInterfaceReadStream(stream, _package.MetaData)));
                }
            }
        }
    }
}
