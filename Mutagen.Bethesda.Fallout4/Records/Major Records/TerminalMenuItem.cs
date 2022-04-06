using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Fallout4;

public partial class TerminalMenuItem
{
    public enum Types
    { 
        SubmenuTerminal = 4,
        SubmenuReturnToTopLevel = 5,
        SubmenuForceRedraw = 6,
        DisplayText = 8,
        DisplayImage = 9,
    }

}
    
partial class TerminalMenuItemBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = Array.Empty<IConditionGetter>();

    partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
    }
}