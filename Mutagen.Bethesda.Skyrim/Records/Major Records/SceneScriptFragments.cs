using Mutagen.Bethesda.Plugins.Binary.Overlay;

namespace Mutagen.Bethesda.Skyrim;

internal partial class SceneScriptFragmentsBinaryOverlay
{
    public IReadOnlyList<IScenePhaseFragmentGetter> PhaseFragments => BinaryOverlayList.FactoryByLazyParse<IScenePhaseFragmentGetter>(_structData.Slice(FlagsEndingPos), _package, countLength: 2, (s, p) => ScenePhaseFragmentBinaryOverlay.ScenePhaseFragmentFactory(s, p));
}