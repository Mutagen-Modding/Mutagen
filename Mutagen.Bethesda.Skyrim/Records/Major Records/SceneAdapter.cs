using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class SceneAdapterBinaryCreateTranslation
        {
            public static partial void FillBinaryScriptFragmentsCustom(MutagenFrame frame, ISceneAdapter item)
            {
                item.ScriptFragments = Mutagen.Bethesda.Skyrim.SceneScriptFragments.CreateFromBinary(frame: frame);
            }
        }

        public partial class SceneAdapterBinaryWriteTranslation
        {
            public static partial void WriteBinaryScriptFragmentsCustom(MutagenWriter writer, ISceneAdapterGetter item)
            {
                if (!item.ScriptFragments.TryGet(out var frags)) return;
                frags.WriteToBinary(writer);
            }
        }

        public partial class SceneAdapterBinaryOverlay
        {
            ISceneScriptFragmentsGetter? GetScriptFragmentsCustom(int location)
            {
                if (this.ScriptsEndingPos == _data.Length) return null;
                return SceneScriptFragmentsBinaryOverlay.SceneScriptFragmentsFactory(
                    _data.Slice(this.ScriptsEndingPos),
                    _package);
            }
        }
    }
}
