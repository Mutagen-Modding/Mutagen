using Loqui;
using Loqui.Internal;
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
        where T : ILoquiObject<T>
    {
        public static readonly ListGroup_TranslationMask<TranslationMaskStub> XmlFolderTranslationMask = new ListGroup_TranslationMask<TranslationMaskStub>(true)
        {
            Items = new MaskItem<bool, TranslationMaskStub>(false, default)
        };
    }

    public static class ListGroupExt
    {
        public static async Task Create_Xml_Folder<T>(
            this ListGroup<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : ILoquiObject<T>, IXmlFolderItem, new()
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
                            group.FillPublic_Xml(
                                elem,
                                errorMask,
                                translationMask: ListGroup<T>.XmlFolderTranslationMask.GetCrystal());
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
                            //using (errorMask.PushIndex(counter))
                            //{
                            //try
                            //{
                            tasks.Add(Task.Run(async () =>
                            {
                                var creator = LoquiXmlFolderTranslation<T>.CREATE;
                                return await creator(
                                    node: null,
                                    path: subDir.Dir.Path,
                                    errorMask: errorMask,
                                    translationMask: null);
                            }));
                            //}
                            //catch (Exception ex)
                            //when (errorMask != null)
                            //{
                            //    errorMask.ReportException(ex);
                            //}
                            //}
                            //counter++;
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

        public static async Task Write_Xml_Folder<T>(
            this ListGroup<T> list,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : ILoquiObject<T>, IXmlFolderItem
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
                        ListGroupXmlTranslation<T>.WriteToNode_Xml(
                            list,
                            topNode,
                            errorMask,
                            translationMask: ListGroup<T>.XmlFolderTranslationMask.GetCrystal());
                        List<Task> tasks = new List<Task>();
                        int counter = 0;
                        foreach (var item in list.Items.Items)
                        {
                            //using (errorMask.PushIndex(counter))
                            //{
                            //    try
                            //    {
                            int stampedCounter = counter++;
                            tasks.Add(Task.Run(() =>
                            {
                                item.Write_Xml_Folder(
                                    node: null,
                                    name: name,
                                    counter: stampedCounter,
                                    dir: dir,
                                    errorMask: errorMask);
                            }));
                            //    }
                            //    catch (Exception ex)
                            //    when (errorMask != null)
                            //    {
                            //        errorMask.ReportException(ex);
                            //    }
                            //}
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
            static partial void FillBinary_ContainedRecordType_Custom(
                MutagenFrame frame,
                ListGroup<T> item,
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
            {
                frame.Reader.Position += 4;
            }

            static partial void WriteBinary_ContainedRecordType_Custom(
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
