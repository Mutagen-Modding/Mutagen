using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mutagen.Bethesda.Skyrim
{
    public static class ModContextExt
    {
        public static readonly DialogTopic.TranslationMask DialogTopicCopyMask = new DialogTopic.TranslationMask(true)
        {
            Responses = false
        };

        public static readonly Cell.TranslationMask CellCopyMask = new Cell.TranslationMask(true)
        {
            Persistent = false,
            Temporary = false,
            Landscape = false,
            NavigationMeshes = false,
            Timestamp = false,
            PersistentTimestamp = false,
            TemporaryTimestamp = false,
            UnknownGroupData = false,
            PersistentUnknownGroupData = false,
            TemporaryUnknownGroupData = false,
        };

        public static readonly Worldspace.TranslationMask WorldspaceCopyMask = new Worldspace.TranslationMask(true)
        {
            SubCells = false,
            TopCell = false,
            LargeReferences = false,
            OffsetData = false,
            SubCellsUnknown = false,
            SubCellsTimestamp = false,
        };

        public static readonly Landscape.TranslationMask? LandscapeCopyMask = null;

        internal static IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts(
            this ISkyrimListGroupGetter<ICellBlockGetter> cellBlocks,
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

        internal static IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts(
            this ISkyrimListGroupGetter<ICellBlockGetter> cellBlocks,
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
                        Func<ISkyrimMod, ICellGetter, bool, string?, ICell> cellGetter = (mod, copyCell, dup, edid) =>
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
                            yield return new ModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(
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

        internal static IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateMajorRecordContexts(
            this IReadOnlyList<IWorldspaceBlockGetter> worldspaceBlocks,
            IWorldspaceGetter worldspace,
            ILinkCache linkCache,
            Type type,
            ModKey modKey,
            IModContext? parent,
            bool throwIfUnknown,
            Func<ISkyrimMod, IWorldspaceGetter, IWorldspace> getOrAddAsOverride)
        {
            foreach (var readOnlyBlock in worldspaceBlocks)
            {
                var blockNumX = readOnlyBlock.BlockNumberX;
                var blockNumY = readOnlyBlock.BlockNumberY;
                var blockContext = new ModContext<IWorldspaceBlockGetter>(
                    modKey: modKey,
                    parent: parent,
                    record: readOnlyBlock);
                foreach (var readOnlySubBlock in readOnlyBlock.Items)
                {
                    var subBlockNumY = readOnlySubBlock.BlockNumberY;
                    var subBlockNumX = readOnlySubBlock.BlockNumberX;
                    var subBlockContext = new ModContext<IWorldspaceSubBlockGetter>(
                        modKey: modKey,
                        parent: blockContext,
                        record: readOnlySubBlock);
                    foreach (var readOnlyCell in readOnlySubBlock.Items)
                    {
                        Func<ISkyrimMod, ICellGetter, bool, string?, ICell> cellGetter = (mod, copyCell, dup, edid) =>
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
                                };
                                retrievedBlock.Items.Add(subBlock);
                            }
                            var cell = subBlock.Items.FirstOrDefault(cell => cell.FormKey == formKey);
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
                                subBlock.Items.Add(cell);
                            }
                            return cell;
                        };

                        if (LoquiRegistration.TryGetRegister(type, out var regis)
                            && regis.ClassType == typeof(Cell))
                        {
                            yield return new ModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(
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
}
