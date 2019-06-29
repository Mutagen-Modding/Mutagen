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

namespace Mutagen.Bethesda
{
    public partial class Group<T> : IEnumerable<T>, IGroupCommon<T>
        where T : IMajorRecordInternal, IXmlItem, IBinaryItem
    {
        private Lazy<IObservableCache<T, string>> _editorIDCache;
        public IObservableCache<T, string> ByEditorID => _editorIDCache.Value;

        public IMod SourceMod { get; private set; }

        public Group(IMod mod)
        {
            this.SourceMod = mod;
        }

        partial void CustomCtor()
        {
            _editorIDCache = new Lazy<IObservableCache<T, string>>(() =>
            {
                return this.Items.Connect()
                    .RemoveKey()
                    .AddKey(m => m.EditorID)
                    .AsObservableCache();
            },
            isThreadSafe: true);
        }

        public override string ToString()
        {
            return $"Group<{typeof(T).Name}>({this.Items.Count})";
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Items.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }
    }

    public static class GroupExt
    {
        public static readonly Group_TranslationMask<MajorRecord_TranslationMask> XmlFolderTranslationMask = new Group_TranslationMask<MajorRecord_TranslationMask>(true)
        {
            Items = new MaskItem<bool, MajorRecord_TranslationMask>(false, default)
        };
        public static readonly TranslationCrystal XmlFolderTranslationCrystal = XmlFolderTranslationMask.GetCrystal();

        public static async Task CreateFromXmlFolder<T>(
            this Group<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : MajorRecord, ILoquiObject<T>, IFormKey, IXmlItem, IBinaryItem
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
                            var items = await Task.WhenAll(tasks);
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
            where T : MajorRecord, ILoquiObject<T>, IFormKey, IXmlItem, IBinaryItem
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
                    await Task.WhenAll(tasks);
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
        public static class GroupRecordTypeGetter<T>
        {
            public static readonly RecordType GRUP_RECORD_TYPE = (RecordType)LoquiRegistration.GetRegister(typeof(T)).ClassType.GetField(Mutagen.Bethesda.Constants.GRUP_RECORDTYPE_MEMBER).GetValue(null);
        }

        public partial class GroupBinaryWriteTranslation
        {
            static partial void WriteBinaryContainedRecordTypeCustom<T>(
                MutagenWriter writer,
                IGroupGetter<T> item,
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
                where T : IMajorRecordInternalGetter, IXmlItem, IBinaryItem
            {
                Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                    writer,
                    GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
            }
        }

        public partial class GroupBinaryCreateTranslation<T>
        {
            static partial void FillBinaryContainedRecordTypeCustom(
                MutagenFrame frame,
                Group<T> item,
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
            {
                frame.Reader.Position += 4;
            }
        }
    }
}
