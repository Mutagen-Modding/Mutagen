using Mutagen.Bethesda.Records.Binary.Overlay;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class SceneScriptFragmentsBinaryOverlay
        {
            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                Initialize(stream);
            }
        }
    }
}
