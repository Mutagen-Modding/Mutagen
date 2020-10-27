using Loqui;
using Mutagen.Bethesda.Skyrim.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public static class ModContextExt
    {
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

        internal static IEnumerable<ModContext<ISkyrimMod, IMajorRecordCommon, IMajorRecordCommonGetter>> EnumerateMajorRecordContexts(
            this IListGroupGetter<ICellBlockGetter> cellBlocks, 
            ILinkCache linkCache, 
            Type type,
            ModKey modKey,
            bool throwIfUnknown)
        {
            foreach (var readOnlyBlock in cellBlocks.Records)
            {
                var blockNum = readOnlyBlock.BlockNumber;
                foreach (var readOnlySubBlock in readOnlyBlock.SubBlocks)
                {
                    var subBlockNum = readOnlySubBlock.BlockNumber;
                    foreach (var readOnlyCell in readOnlySubBlock.Cells)
                    {
                        Func<ISkyrimMod, ICellGetter, ICell> cellGetter = (m, r) =>
                        {
                            var formKey = r.FormKey;
                            var retrievedBlock = m.Cells.Records.FirstOrDefault(x => x.BlockNumber == blockNum);
                            if (retrievedBlock == null)
                            {
                                retrievedBlock = new CellBlock()
                                {
                                    BlockNumber = blockNum,
                                    GroupType = GroupTypeEnum.InteriorCellBlock,
                                };
                                m.Cells.Records.Add(retrievedBlock);
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
                                cell = readOnlyCell.DeepCopy(CellCopyMask);
                                subBlock.Cells.Add(cell);
                            }
                            return cell;
                        };

                        if (LoquiRegistration.TryGetRegister(type, out var regis)
                            && regis.ClassType == typeof(Cell))
                        {
                            yield return new ModContext<ISkyrimMod, IMajorRecordCommon, IMajorRecordCommonGetter>(
                                modKey: modKey,
                                record: readOnlyCell,
                                getter: (m, r) => cellGetter(m, (ICellGetter)r));
                        }
                        else
                        {
                            foreach (var con in CellCommon.Instance.EnumerateMajorRecordContexts(readOnlyCell, linkCache, type, modKey, throwIfUnknown, cellGetter))
                            {
                                yield return con;
                            }
                        }
                    }
                }
            }
        }

        internal static IEnumerable<ModContext<ISkyrimMod, IMajorRecordCommon, IMajorRecordCommonGetter>> EnumerateMajorRecordContexts(
            this IReadOnlyList<IWorldspaceBlockGetter> worldspaceBlocks,
            IWorldspaceGetter worldspace,
            ILinkCache linkCache,
            Type type,
            ModKey modKey,
            bool throwIfUnknown, 
            Func<ISkyrimMod, IWorldspaceGetter, IWorldspace> getter)
        {
            foreach (var readOnlyBlock in worldspaceBlocks)
            {
                var blockNumX = readOnlyBlock.BlockNumberX;
                var blockNumY = readOnlyBlock.BlockNumberY;
                foreach (var readOnlySubBlock in readOnlyBlock.Items)
                {
                    var subBlockNumY = readOnlySubBlock.BlockNumberY;
                    var subBlockNumX = readOnlySubBlock.BlockNumberX;
                    foreach (var readOnlyCell in readOnlySubBlock.Items)
                    {
                        Func<ISkyrimMod, ICellGetter, ICell> cellGetter = (m, r) =>
                        {
                            var worldspaceCopy = getter(m, worldspace);
                            var formKey = r.FormKey;
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
                                cell = r.DeepCopy(CellCopyMask);
                                subBlock.Items.Add(cell);
                            }
                            return cell;
                        };

                        if (LoquiRegistration.TryGetRegister(type, out var regis)
                            && regis.ClassType == typeof(Cell))
                        {
                            yield return new ModContext<ISkyrimMod, IMajorRecordCommon, IMajorRecordCommonGetter>(
                                modKey: modKey,
                                record: readOnlyCell,
                                getter: (m, r) => cellGetter(m, (ICellGetter)r));
                        }
                        else
                        {
                            foreach (var con in CellCommon.Instance.EnumerateMajorRecordContexts(readOnlyCell, linkCache, type, modKey, throwIfUnknown, cellGetter))
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
