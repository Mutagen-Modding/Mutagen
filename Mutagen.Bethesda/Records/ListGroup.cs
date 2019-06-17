using Loqui;
using Loqui.Internal;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Folder;
using Mutagen.Bethesda.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public partial class ListGroup<T>
        where T : ILoquiObject<T>, IXmlItem, IBinaryItem
    {
    }

    public static class ListGroupExt
    {
        public static readonly ListGroup_TranslationMask<TranslationMaskStub> XmlFolderTranslationMask = new ListGroup_TranslationMask<TranslationMaskStub>(true)
        {
            Items = new MaskItem<bool, TranslationMaskStub>(false, default)
        };

        public static async Task CreateFromXmlFolder<T>(
            this ListGroup<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : ILoquiObject<T>, IXmlItem, IBinaryItem, IXmlFolderItem, new()
        {
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.Items))
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
                            ListGroupXmlTranslation<T>.FillPublicXml(
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
                                    translationMask: null);
                            }));
                        }
                        var items = await Task.WhenAll(tasks);
                        group.Items.AddRange(items);
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
            this ListGroup<T> list,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : ILoquiObject<T>, IXmlItem, IBinaryItem, IXmlFolderItem
        {
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.Items))
                {
                    try
                    {
                        dir = new DirectoryPath(Path.Combine(dir.Path, name));
                        dir.Create();
                        XElement topNode = new XElement("Group");
                        ListGroupXmlTranslation<T>.WriteToNodeXml(
                            list,
                            topNode,
                            errorMask,
                            translationMask: XmlFolderTranslationMask.GetCrystal());
                        List<Task> tasks = new List<Task>();
                        int counter = 0;
                        foreach (var item in list.Items.Items)
                        {
                            int stampedCounter = counter++;
                            tasks.Add(Task.Run(() =>
                            {
                                item.WriteToXmlFolder(
                                    node: null,
                                    name: name,
                                    counter: stampedCounter,
                                    dir: dir,
                                    errorMask: errorMask);
                            }));
                        }
                        await Task.WhenAll(tasks);
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
        public partial class ListGroupBinaryTranslation<T>
        {
            static partial void FillBinaryContainedRecordTypeCustom(
                MutagenFrame frame,
                ListGroup<T> item,
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
            {
                frame.Reader.Position += 4;
            }

            static partial void WriteBinaryContainedRecordTypeCustom(
                MutagenWriter writer,
                IListGroupGetter<T> item,
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
            {
                Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                    writer,
                    ListGroup<T>.GRUP_RECORD_TYPE.TypeInt);
            }
        }
    }
}
