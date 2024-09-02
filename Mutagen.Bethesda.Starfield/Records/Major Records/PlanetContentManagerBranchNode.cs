namespace Mutagen.Bethesda.Starfield;

public partial class PlanetContentManagerBranchNode
{
    public enum NodeTypeOption
    {
        BranchNode = 0x01,
        ContentNode = 0x02,
    }
    
    public enum ChildSelectionOption
    {
        Random,
        Stacked,
    }
}