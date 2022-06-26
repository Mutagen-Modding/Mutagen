using Mutagen.Bethesda.Plugins.Binary.Overlay;

namespace Mutagen.Bethesda.Skyrim;

partial class SceneScriptFragmentsBinaryOverlay
{
    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        Initialize(stream);
    }
}