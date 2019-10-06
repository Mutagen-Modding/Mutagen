using Loqui;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Internals;
using Noggog;
using Loqui.Internal;
using System.Collections;
using Noggog.Utility;
using Mutagen.Bethesda.Folder;
using System.Xml.Linq;
using DynamicData;
using Loqui.Xml;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Group<T> : GroupAbstract<T>
    {
        public Group(IModGetter getter) : base(getter)
        {
        }

        public Group(IMod mod) : base(mod)
        {
        }

        protected override IObservableCache<T, FormKey> InternalItems => throw new NotImplementedException();
    }

    public static class GroupExt
    {
        public static readonly Group_TranslationMask<OblivionMajorRecord_TranslationMask> XmlFolderTranslationMask = new Group_TranslationMask<OblivionMajorRecord_TranslationMask>(true)
        {
            Items = new MaskItem<bool, OblivionMajorRecord_TranslationMask>(false, default)
        };
        public static readonly TranslationCrystal XmlFolderTranslationCrystal = XmlFolderTranslationMask.GetCrystal();

        public static async Task CreateFromXmlFolder<T>(
            this Group<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : OblivionMajorRecord, ILoquiObject<T>, IFormKey, IXmlItem, IBinaryItem
        {
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.Items))
                {
                    try
                    {
                        var path = Path.Combine(dir.Path, $"{name}.xml");
                        if (!File.Exists(path))
                        {
                            group.Clear();
                            return;
                        }
                        XElement elem = XElement.Load(path);
                        if (elem.Name != "Group")
                        {
                            throw new ArgumentException("XML file did not have \"Group\" top node.");
                        }
                        GroupXmlCreateTranslation<T>.FillPublicXml(
                            group,
                            elem,
                            errorMask,
                            translationMask: GroupExt.XmlFolderTranslationCrystal);
                        var itemsNode = elem.Element("Items");
                        if (itemsNode != null)
                        {
                            List<Task<T>> tasks = new List<Task<T>>();
                            foreach (var itemNode in itemsNode.Elements())
                            {
                                tasks.Add(
                                    LoquiXmlFolderTranslation<T>.CREATE(
                                        node: itemNode,
                                        path: dir.Path,
                                        errorMask: errorMask,
                                        translationMask: null));
                            }
                            var items = await Task.WhenAll(tasks).ConfigureAwait(false);
                            group.Items.Set(items);
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
        }

        public static async Task WriteToXmlFolder<T, T_ErrMask>(
            this Group<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : OblivionMajorRecord, ILoquiObject<T>, IFormKey, IXmlItem, IBinaryItem
            where T_ErrMask : MajorRecord_ErrorMask, IErrorMask<T_ErrMask>, new()
        {
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.Items))
                {
                    if (group.Items.Count == 0) return;
                    XElement topNode = new XElement("Group");
                    GroupXmlWriteTranslation.WriteToNodeXml(
                        group,
                        topNode,
                        errorMask,
                        translationMask: GroupExt.XmlFolderTranslationCrystal);
                    int counter = 0;
                    XElement items = new XElement("Items");
                    List<Task> tasks = new List<Task>();
                    foreach (var item in group.Items.Items)
                    {
                        tasks.Add(
                            item.WriteToXmlFolder(
                                node: items,
                                name: name,
                                counter: counter++,
                                dir: dir,
                                errorMask: errorMask));
                    }
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    if (items.HasElements)
                    {
                        topNode.Add(items);
                    }
                    if (topNode.HasElements)
                    {
                        dir.Create();
                        topNode.SaveIfChanged(Path.Combine(dir.Path, $"{name}.xml"));
                    }
                }
            }
        }
    }

    namespace Internals
    {
        public partial class GroupBinaryWriteTranslation
        {
            static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
                MutagenWriter writer,
                IGroupGetter<T> item,
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
                where T : IOblivionMajorRecordGetter, IXmlItem, IBinaryItem
            {
                Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                    writer,
                    GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
            }
        }

        public partial class GroupBinaryCreateTranslation<T>
        {
            static partial void FillBinaryContainedRecordTypeParseCustom(
                MutagenFrame frame,
                Group<T> item,
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
            {
                frame.Reader.Position += 4;
            }
        }

        public partial class GroupBinaryWrapper<T>
        {
            private GroupMajorRecordCacheWrapper<T> _Items;
            public IReadOnlyCache<T, FormKey> Items => _Items;

            partial void CustomCtor(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _Items = GroupMajorRecordCacheWrapper<T>.Factory(
                    stream,
                    _data,
                    _package,
                    offset);
            }
        }
    }
}
