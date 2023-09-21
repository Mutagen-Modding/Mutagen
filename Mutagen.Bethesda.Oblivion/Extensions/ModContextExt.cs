using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Oblivion;

internal static class ModContextExt
{
    public static readonly DialogTopic.TranslationMask DialogResponsesCopyMask = new(true)
    {
        Items = false
    };

    public static readonly Cell.TranslationMask CellCopyMask = new(true)
    {
        Persistent = false,
        Temporary = false,
        Landscape = false,
        VisibleWhenDistant = false,
        PathGrid = false
    };

    public static readonly Worldspace.TranslationMask WorldspaceCopyMask = new(true)
    {
        SubCells = false,
        TopCell = false,
        Road = false,
    };

    public static readonly Landscape.TranslationMask? LandscapeCopyMask = null;
    public static readonly Road.TranslationMask? RoadCopyMask = null;
    public static readonly PathGrid.TranslationMask? PathGridCopyMask = null;

    private static readonly string CellObjectKey =
        LoquiRegistration.StaticRegister.GetRegister(typeof(ICell)).FullName; 

    public static IEnumerable<IModContext<IOblivionMod, IOblivionModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts(
        this IOblivionListGroupGetter<ICellBlockGetter> cellBlocks,
        ILinkCache linkCache,
        ModKey modKey,
        IModContext? parent)
    {
        return EnumerateMajorRecordContexts(
            cellBlocks,
            linkCache,
            type: typeof(IMajorRecordGetter),
            modKey: modKey,
            parent: parent,
            throwIfUnknown: true);
    }

    public static IEnumerable<IModContext<IOblivionMod, IOblivionModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts(
        this IOblivionListGroupGetter<ICellBlockGetter> cellBlocks,
        ILinkCache linkCache,
        Type type,
        ModKey modKey,
        IModContext? parent,
        bool throwIfUnknown)
    {
        foreach (var readOnlyBlock in cellBlocks.Records)
        {
            var blockNum = readOnlyBlock.BlockNumber;
            var blockModified = readOnlyBlock.LastModified;
            var blockContext = new ModContext<ICellBlockGetter>(
                modKey: modKey,
                parent: parent,
                record: readOnlyBlock);
            foreach (var readOnlySubBlock in readOnlyBlock.SubBlocks)
            {
                var subBlockNum = readOnlySubBlock.BlockNumber;
                var subBlockModified = readOnlySubBlock.LastModified;
                var subBlockContext = new ModContext<ICellSubBlockGetter>(
                    modKey: modKey,
                    parent: blockContext,
                    record: readOnlySubBlock);
                foreach (var readOnlyCell in readOnlySubBlock.Cells)
                {
                    Func<IOblivionMod, ICellGetter, bool, string?, FormKey?, ICell> cellGetter = (mod, copyCell, dup, edid, newFormKey) =>
                    {
                        var formKey = copyCell.FormKey;
                        var retrievedBlock = mod.Cells.Records.FirstOrDefault(x => x.BlockNumber == blockNum);
                        if (retrievedBlock == null)
                        {
                            retrievedBlock = new CellBlock()
                            {
                                BlockNumber = blockNum,
                                GroupType = GroupTypeEnum.InteriorCellBlock,
                                LastModified = blockModified,
                            };
                            mod.Cells.Records.Add(retrievedBlock);
                        }
                        var subBlock = retrievedBlock.SubBlocks.FirstOrDefault(x => x.BlockNumber == subBlockNum);
                        if (subBlock == null)
                        {
                            subBlock = new CellSubBlock()
                            {
                                BlockNumber = subBlockNum,
                                GroupType = GroupTypeEnum.InteriorCellSubBlock,
                                LastModified = subBlockModified,
                            };
                            retrievedBlock.SubBlocks.Add(subBlock);
                        }
                        var cell = subBlock.Cells.FirstOrDefault(cell => cell.FormKey == formKey);
                        if (cell == null)
                        {
                            if (dup)
                            {
                                cell = copyCell.Duplicate(newFormKey ?? mod.GetNextFormKey(edid), CellCopyMask);
                            }
                            else
                            {
                                cell = copyCell.DeepCopy(CellCopyMask);
                            }
                            subBlock.Cells.Add(cell);
                        }
                        return cell;
                    };
                    
                    var match = MajorRecordContextEnumerableUtility.GetMatch(type, CellObjectKey);
                    if (match != MajorRecordContextEnumerableUtility.TypeMatch.NotMatch)
                    {
                        yield return new ModContext<IOblivionMod, IOblivionModGetter, IMajorRecord, IMajorRecordGetter>(
                            modKey: modKey,
                            record: readOnlyCell,
                            getOrAddAsOverride: (m, r) => cellGetter(m, (ICellGetter)r, false, default(string?), default(FormKey?)),
                            duplicateInto: (m, r, e, f) => cellGetter(m, (ICellGetter)r, true, e, f),
                            parent: subBlockContext);
                    }
                    
                    if (match != MajorRecordContextEnumerableUtility.TypeMatch.Match)
                    {
                        foreach (var con in CellCommon.Instance.EnumerateMajorRecordContexts(
                                     readOnlyCell, 
                                     linkCache,
                                     type,
                                     modKey, 
                                     subBlockContext, 
                                     throwIfUnknown, 
                                     (m, c) => cellGetter(m, c, false, default(string?), default(FormKey?)),
                                     (m, c, e, f) => cellGetter(m, c, true, e, f)))
                        {
                            yield return con;
                        }
                    }
                }
            }
        }
    }

    public static IEnumerable<IModContext<IOblivionMod, IOblivionModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts(
        this IReadOnlyList<IWorldspaceBlockGetter> worldspaceBlocks,
        IWorldspaceGetter worldspace,
        ILinkCache linkCache,
        Type type,
        ModKey modKey,
        IModContext? parent,
        bool throwIfUnknown,
        Func<IOblivionMod, IWorldspaceGetter, IWorldspace> getOrAddAsOverride)
    {
        foreach (var readOnlyBlock in worldspaceBlocks)
        {
            var blockNumX = readOnlyBlock.BlockNumberX;
            var blockNumY = readOnlyBlock.BlockNumberY;
            var blockModified = readOnlyBlock.LastModified;
            var blockContext = new ModContext<IWorldspaceBlockGetter>(
                modKey: modKey,
                parent: parent,
                record: readOnlyBlock);
            foreach (var readOnlySubBlock in readOnlyBlock.Items)
            {
                var subBlockNumY = readOnlySubBlock.BlockNumberY;
                var subBlockNumX = readOnlySubBlock.BlockNumberX;
                var subBlockModified = readOnlySubBlock.LastModified;
                var subBlockContext = new ModContext<IWorldspaceSubBlockGetter>(
                    modKey: modKey,
                    parent: blockContext,
                    record: readOnlySubBlock);
                foreach (var readOnlyCell in readOnlySubBlock.Items)
                {
                    Func<IOblivionMod, ICellGetter, bool, string?, FormKey?, ICell> cellGetter = (mod, copyCell, dup, edid, newFormKey) =>
                    {
                        var worldspaceCopy = getOrAddAsOverride(mod, worldspace);
                        var formKey = copyCell.FormKey;
                        var retrievedBlock = worldspaceCopy.SubCells.FirstOrDefault(x => x.BlockNumberX == blockNumX && x.BlockNumberY == blockNumY);
                        if (retrievedBlock == null)
                        {
                            retrievedBlock = new WorldspaceBlock()
                            {
                                BlockNumberX = blockNumX,
                                BlockNumberY = blockNumY,
                                GroupType = GroupTypeEnum.ExteriorCellBlock,
                                LastModified = blockModified,
                            };
                            worldspaceCopy.SubCells.Add(retrievedBlock);
                        }
                        var subBlock = retrievedBlock.Items.FirstOrDefault(x => x.BlockNumberX == subBlockNumX && x.BlockNumberY == subBlockNumY);
                        if (subBlock == null)
                        {
                            subBlock = new WorldspaceSubBlock()
                            {
                                BlockNumberX = subBlockNumX,
                                BlockNumberY = subBlockNumY,
                                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
                                LastModified = readOnlySubBlock.LastModified,
                            };
                            retrievedBlock.Items.Add(subBlock);
                        }
                        var cell = subBlock.Items.FirstOrDefault(cell => cell.FormKey == formKey);
                        if (cell == null)
                        {
                            if (dup)
                            {
                                cell = copyCell.Duplicate(newFormKey ?? mod.GetNextFormKey(edid), CellCopyMask);
                            }
                            else
                            {
                                cell = copyCell.DeepCopy(CellCopyMask);
                            }
                            subBlock.Items.Add(cell);
                        }
                        return cell;
                    };
                    
                    var match = MajorRecordContextEnumerableUtility.GetMatch(type, CellObjectKey);
                    if (match != MajorRecordContextEnumerableUtility.TypeMatch.NotMatch)
                    {
                        yield return new ModContext<IOblivionMod, IOblivionModGetter, IMajorRecord, IMajorRecordGetter>(
                            modKey: modKey,
                            record: readOnlyCell,
                            getOrAddAsOverride: (m, r) => cellGetter(m, (ICellGetter)r, false, default(string?), default(FormKey?)),
                            duplicateInto: (m, r, e, f) => cellGetter(m, (ICellGetter)r, true, e, f),
                            parent: subBlockContext);
                    }
                    
                    if (match != MajorRecordContextEnumerableUtility.TypeMatch.Match)
                    {
                        foreach (var con in CellCommon.Instance.EnumerateMajorRecordContexts(
                                     readOnlyCell, 
                                     linkCache,
                                     type,
                                     modKey, 
                                     subBlockContext, 
                                     throwIfUnknown, 
                                     (m, c) => cellGetter(m, c, false, default(string?), default(FormKey?)),
                                     (m, c, e, f) => cellGetter(m, c, true, e, f)))
                        {
                            yield return con;
                        }
                    }
                }
            }
        }
    }
}