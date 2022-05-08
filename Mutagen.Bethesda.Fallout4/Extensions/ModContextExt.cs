using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout4;

internal static class ModContextExt
{
    public static readonly Cell.TranslationMask CellCopyMask = new(true)
    {
        // Persistent = false,
        // Temporary = false,
        // Landscape = false,
        // NavigationMeshes = false,
        // Timestamp = false,
        // PersistentTimestamp = false,
        // TemporaryTimestamp = false,
        // UnknownGroupData = false,
        // PersistentUnknownGroupData = false,
        // TemporaryUnknownGroupData = false,
    };

    // public static readonly Landscape.TranslationMask? LandscapeCopyMask = null;

    public static IEnumerable<IModContext<IFallout4Mod, IFallout4ModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts(
        this IFallout4ListGroupGetter<ICellBlockGetter> cellBlocks,
        ILinkCache linkCache,
        ModKey modKey,
        IModContext? parent)
    {
        return EnumerateMajorRecordContexts(
            cellBlocks: cellBlocks,
            linkCache: linkCache,
            type: typeof(IMajorRecordGetter),
            modKey: modKey,
            parent: parent,
            throwIfUnknown: true);
    }

    public static IEnumerable<IModContext<IFallout4Mod, IFallout4ModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts(
        this IFallout4ListGroupGetter<ICellBlockGetter> cellBlocks,
        ILinkCache linkCache,
        Type type,
        ModKey modKey,
        IModContext? parent,
        bool throwIfUnknown)
    {
        foreach (var readOnlyBlock in cellBlocks.Records)
        {
            var blockNum = readOnlyBlock.BlockNumber;
            var blockContext = new ModContext<ICellBlockGetter>(
                modKey: modKey,
                parent: parent,
                record: readOnlyBlock);
            foreach (var readOnlySubBlock in readOnlyBlock.SubBlocks)
            {
                var subBlockNum = readOnlySubBlock.BlockNumber;
                var subBlockContext = new ModContext<ICellSubBlockGetter>(
                    modKey: modKey,
                    parent: blockContext,
                    record: readOnlySubBlock);
                foreach (var readOnlyCell in readOnlySubBlock.Cells)
                {
                    Func<IFallout4Mod, ICellGetter, bool, string?, ICell> cellGetter = (mod, copyCell, dup, edid) =>
                    {
                        var formKey = copyCell.FormKey;
                        var retrievedBlock = mod.Cells.Records.FirstOrDefault(x => x.BlockNumber == blockNum);
                        if (retrievedBlock == null)
                        {
                            retrievedBlock = new CellBlock()
                            {
                                BlockNumber = blockNum,
                                GroupType = GroupTypeEnum.InteriorCellBlock,
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
                            };
                            retrievedBlock.SubBlocks.Add(subBlock);
                        }
                        var cell = subBlock.Cells.FirstOrDefault(cell => cell.FormKey == formKey);
                        if (cell == null)
                        {
                            if (dup)
                            {
                                cell = copyCell.Duplicate(mod.GetNextFormKey(edid), CellCopyMask);
                            }
                            else
                            {
                                cell = copyCell.DeepCopy(CellCopyMask);
                            }
                            subBlock.Cells.Add(cell);
                        }
                        return cell;
                    };

                    if (LoquiRegistration.TryGetRegister(type, out var regis)
                        && regis.ClassType == typeof(Cell))
                    {
                        yield return new ModContext<IFallout4Mod, IFallout4ModGetter, IMajorRecord, IMajorRecordGetter>(
                            modKey: modKey,
                            record: readOnlyCell,
                            getOrAddAsOverride: (m, r) => cellGetter(m, (ICellGetter)r, false, default(string?)),
                            duplicateInto: (m, r, e) => cellGetter(m, (ICellGetter)r, true, e),
                            parent: subBlockContext);
                    }
                    else
                    {
                        foreach (var con in CellCommon.Instance.EnumerateMajorRecordContexts(
                                     readOnlyCell,
                                     linkCache, 
                                     type, 
                                     modKey,
                                     subBlockContext, 
                                     throwIfUnknown,
                                     (m, c) => cellGetter(m, c, false, default(string?)),
                                     (m, c, e) => cellGetter(m, c, true, e)))
                        {
                            yield return con;
                        }
                    }
                }
            }
        }
    }
}