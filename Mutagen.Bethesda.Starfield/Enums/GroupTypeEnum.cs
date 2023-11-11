namespace Mutagen.Bethesda.Starfield;

/// <summary>
/// Different categories of Group records
/// </summary>
public enum GroupTypeEnum
{
    Type = 0,
    WorldChildren = 1,
    InteriorCellBlock = 2,
    InteriorCellSubBlock = 3,
    ExteriorCellBlock = 4,
    ExteriorCellSubBlock = 5,
    CellChildren = 6,
    TopicChildren = 7,
    CellPersistentChildren = 8,
    CellTemporaryChildren = 9,
    QuestChildren = 10,
}