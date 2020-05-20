using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class NavigationMapInfoBinaryCreateTranslation
        {
            static partial void FillBinaryIslandCustom(MutagenFrame frame, INavigationMapInfo item)
            {
                if (frame.ReadUInt8() > 0)
                {
                    item.Island = IslandData.CreateFromBinary(frame);
                }
            }

            static partial void FillBinaryParentParseLogicCustom(MutagenFrame frame, INavigationMapInfo item)
            {
                if (item.ParentWorldspace.IsNull)
                {
                    item.ParentCell = FormKeyBinaryTranslation.Instance.Parse(frame);
                }
                else
                {
                    item.ParentWorldspaceCoord = P2Int16BinaryTranslation.Instance.Parse(frame);
                }
            }
        }

        public partial class NavigationMapInfoBinaryWriteTranslation
        {
            static partial void WriteBinaryIslandCustom(MutagenWriter writer, INavigationMapInfoGetter item)
            {
                if (item.Island.TryGet(out var island))
                {
                    writer.Write((byte)1);
                    island.WriteToBinary(writer);
                }
                else
                {
                    writer.Write(default(byte));
                }
            }

            static partial void WriteBinaryParentParseLogicCustom(MutagenWriter writer, INavigationMapInfoGetter item)
            {
                if (item.ParentWorldspace.IsNull)
                {
                    FormKeyBinaryTranslation.Instance.Write(writer, item.ParentCell.FormKey);
                }
                else
                {
                    P2Int16BinaryTranslation.Instance.Write(writer, item.ParentWorldspaceCoord);
                }
            }
        }

        public partial class NavigationMapInfoBinaryOverlay
        {
            IIslandDataGetter? _island;
            IIslandDataGetter? GetIslandCustom(int location) => _island;

            public P2Int16 ParentWorldspaceCoord
            {
                get
                {
                    return new P2Int16(
                        BinaryPrimitives.ReadInt16LittleEndian(_data.Span.Slice(IslandEndingPos + 0x8)),
                        BinaryPrimitives.ReadInt16LittleEndian(_data.Span.Slice(IslandEndingPos + 0xA)));
                }
            }

            public IFormLinkGetter<ICellGetter> ParentCell
            {
                get
                {
                    return new FormLink<ICellGetter>(FormKey.Factory(_package.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Span.Slice(IslandEndingPos + 0x8, 0x4))));
                }
            }

            partial void CustomCtor(IBinaryReadStream stream, int finalPos, int offset)
            {
                if (_data[LinkedDoorsEndingPos] > 0)
                {
                    using var islandStream = new BinaryMemoryReadStream(_data.Slice(LinkedDoorsEndingPos + 1));
                    this._island =  IslandDataBinaryOverlay.IslandDataFactory(
                        islandStream,
                        _package);
                    this.IslandEndingPos = LinkedDoorsEndingPos + 1 + islandStream.Position;
                }
                else
                {
                    this._island = null;
                    this.IslandEndingPos = LinkedDoorsEndingPos + 1;
                }
            }
        }
    }
}
