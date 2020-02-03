using Loqui;
using Loqui.Internal;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Folder;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Oblivion
{
    public static class ListGroupExt
    {
        public static readonly ListGroup_TranslationMask<CellBlock_TranslationMask> XmlFolderTranslationMask = new ListGroup_TranslationMask<CellBlock_TranslationMask>(true)
        {
            Records = new MaskItem<bool, CellBlock_TranslationMask?>(false, default)
        };

        public static async Task CreateFromXmlFolder<T>(
            this ListGroup<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder? errorMask,
            int index)
            where T : class, ICellBlock, IXmlItem, IBinaryItem, ILoquiObjectSetter<T>
        {
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)ListGroup_FieldIndex.Records))
                {
                    try
                    {
                        dir = new DirectoryPath(Path.Combine(dir.Path, name));
                        group.Clear();
                        var path = Path.Combine(dir.Path, $"Group.xml");
                        if (File.Exists(path))
                        {
                            XElement elem = XElement.Load(path);
                            if (elem.Name != "Group")
                            {
                                throw new ArgumentException("XML file did not have \"Group\" top node.");
                            }
                            ListGroupXmlCreateTranslation<T>.FillPublicXml(
                                group,
                                elem,
                                errorMask,
                                translationMask: XmlFolderTranslationMask.GetCrystal());
                        }
                        List<Task<T>> tasks = new List<Task<T>>();
                        //int counter = 0;
                        foreach (var subDir in dir.EnumerateDirectories(recursive: false, includeSelf: false)
                            .SelectWhere(subDir =>
                            {
                                if (int.TryParse(subDir.Name, out var i))
                                {
                                    return TryGet<(int Index, DirectoryPath Dir)>.Succeed((i, subDir));
                                }
                                else
                                {
                                    return TryGet<(int Index, DirectoryPath Dir)>.Failure;
                                }
                            })
                            .OrderBy(i => i.Index))
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                var creator = LoquiXmlFolderTranslation<T>.CREATE;
                                return await creator(
                                    node: null,
                                    path: subDir.Dir.Path,
                                    errorMask: errorMask,
                                    translationMask: null).ConfigureAwait(false);
                            }));
                        }
                        var items = await Task.WhenAll(tasks).ConfigureAwait(false);
                        group.Records.AddRange(items);
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
        }

        public static async Task WriteToXmlFolder<T>(
            this IListGroupGetter
            <T> list,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder? errorMask,
            int index)
            where T : class, ICellBlockGetter, IXmlItem, IBinaryItem, IXmlFolderItem
        {
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)ListGroup_FieldIndex.Records))
                {
                    try
                    {
                        dir = new DirectoryPath(Path.Combine(dir.Path, name));
                        dir.Create();
                        XElement topNode = new XElement("Group");
                        ListGroupXmlWriteTranslation.WriteToNodeXml(
                            list,
                            topNode,
                            errorMask,
                            translationMask: XmlFolderTranslationMask.GetCrystal());
                        List<Task> tasks = new List<Task>();
                        int counter = 0;
                        foreach (var item in list.Records)
                        {
                            int stampedCounter = counter++;
                            tasks.Add(Task.Run(() =>
                            {
                                item.WriteToXmlFolder(
                                    node: null!,
                                    name: name,
                                    counter: stampedCounter,
                                    dir: dir,
                                    errorMask: errorMask);
                            }));
                        }
                        await Task.WhenAll(tasks).ConfigureAwait(false);
                        topNode.SaveIfChanged(Path.Combine(dir.Path, $"Group.xml"));
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
        }
    }

    namespace Internals
    {
        public partial class ListGroupBinaryCreateTranslation<T>
        {
            static partial void FillBinaryContainedRecordTypeCustom(
                MutagenFrame frame,
                IListGroup<T> item,
                MasterReferences masterReferences)
            {
                frame.Reader.Position += 4;
            }
        }

        public partial class ListGroupBinaryWriteTranslation
        {
            static partial void WriteBinaryContainedRecordTypeCustom<T>(
                MutagenWriter writer,
                IListGroupGetter<T> item,
                MasterReferences masterReferences)
                where T : class, ICellBlockGetter, IXmlItem, IBinaryItem
            {
                Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                    writer,
                    GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
            }
        }

        public partial class ListGroupBinaryOverlay<T>
        {
            private ListGroupAbstract.GroupListOverlay<T>? _Records;
            public IReadOnlyList<T> Records => _Records!;

            partial void CustomCtor(
                IBinaryReadStream stream,
                int finalPos,
                int offset)
            {
                _Records = ListGroupAbstract.GroupListOverlay<T>.Factory(
                    stream,
                    _data,
                    _package,
                    offset: offset,
                    objectType: ObjectType.Group);
            }
        }
    }
}
