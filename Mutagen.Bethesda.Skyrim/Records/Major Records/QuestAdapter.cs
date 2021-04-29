using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class QuestAdapterBinaryCreateTranslation
        {
            public static partial void FillBinaryFragmentCountCustom(MutagenFrame frame, IQuestAdapter item)
            {
                var count = frame.ReadUInt16();
                item.FileName = StringBinaryTranslation.Instance.Parse(frame, stringBinaryType: StringBinaryType.PrependLengthUShort);
                item.Fragments.SetTo(
                    ListBinaryTranslation<QuestScriptFragment>.Instance.Parse(
                        frame,
                        amount: count,
                        transl: (MutagenFrame r, out QuestScriptFragment listSubItem) =>
                        {
                            listSubItem = QuestScriptFragment.CreateFromBinary(frame);
                            return true;
                        }));
                var aliasCount = frame.ReadUInt16();
                item.Aliases.SetTo(
                    ListBinaryTranslation<QuestFragmentAlias>.Instance.Parse(
                        frame,
                        amount: aliasCount,
                        transl: (MutagenFrame r, out QuestFragmentAlias listSubItem) =>
                        {
                            listSubItem = QuestFragmentAlias.CreateFromBinary(frame);
                            return true;
                        }));
            }

            public static partial void FillBinaryAliasesCustom(MutagenFrame frame, IQuestAdapter item)
            {
            }

            public static partial void FillBinaryFileNameCustom(MutagenFrame frame, IQuestAdapter item)
            {
            }

            public static partial void FillBinaryFragmentsCustom(MutagenFrame frame, IQuestAdapter item)
            {
            }
        }

        public partial class QuestAdapterBinaryWriteTranslation
        {
            public static partial void WriteBinaryFragmentCountCustom(MutagenWriter writer, IQuestAdapterGetter item)
            {
                var frags = item.Fragments;
                writer.Write(checked((ushort)frags.Count));
                writer.Write(item.FileName, StringBinaryType.PrependLengthUShort);
                ListBinaryTranslation<IQuestScriptFragmentGetter>.Instance.Write(
                    writer,
                    frags,
                    transl: (MutagenWriter subWriter, IQuestScriptFragmentGetter subItem) =>
                    {
                        subItem.WriteToBinary(subWriter);
                    });
                var alias = item.Aliases;
                writer.Write((ushort)alias.Count);
                ListBinaryTranslation<IQuestFragmentAliasGetter>.Instance.Write(
                    writer,
                    alias,
                    transl: (MutagenWriter subWriter, IQuestFragmentAliasGetter subItem) =>
                    {
                        subItem.WriteToBinary(subWriter);
                    });
            }

            public static partial void WriteBinaryAliasesCustom(MutagenWriter writer, IQuestAdapterGetter item)
            {
            }

            public static partial void WriteBinaryFileNameCustom(MutagenWriter writer, IQuestAdapterGetter item)
            {
            }

            public static partial void WriteBinaryFragmentsCustom(MutagenWriter writer, IQuestAdapterGetter item)
            {
            }
        }

        public partial class QuestAdapterBinaryOverlay
        {
            private string _filename = string.Empty;
            public String GetFileNameCustom(int location) => _filename;

            public IReadOnlyList<IQuestScriptFragmentGetter> Fragments { get; private set; } = ListExt.Empty<IQuestScriptFragmentGetter>();

            public IReadOnlyList<IQuestFragmentAliasGetter> Aliases { get; private set; } = ListExt.Empty<IQuestFragmentAliasGetter>();

            partial void CustomFileNameEndPos()
            {
                if (this._data.Length <= this.ScriptsEndingPos) return;
                var frame = new MutagenFrame(
                    new MutagenInterfaceReadStream(
                        new BinaryMemoryReadStream(_data.Slice(ScriptsEndingPos)),
                        _package.MetaData));
                // Skip unknown
                frame.Position += 1;
                var count = frame.ReadUInt16();
                _filename = StringBinaryTranslation.Instance.Parse(frame, stringBinaryType: StringBinaryType.PrependLengthUShort);
                Fragments = 
                    ListBinaryTranslation<QuestScriptFragment>.Instance.Parse(
                        frame,
                        amount: count,
                        transl: (MutagenFrame r, out QuestScriptFragment listSubItem) =>
                        {
                            listSubItem = QuestScriptFragment.CreateFromBinary(frame);
                            return true;
                        })
                    .ToList();
                var aliasCount = frame.ReadUInt16();
                Aliases = 
                    ListBinaryTranslation<QuestFragmentAlias>.Instance.Parse(
                        frame,
                        amount: aliasCount,
                        transl: (MutagenFrame r, out QuestFragmentAlias listSubItem) =>
                        {
                            listSubItem = QuestFragmentAlias.CreateFromBinary(frame);
                            return true;
                        })
                    .ToList();
            }
        }
    }
}
