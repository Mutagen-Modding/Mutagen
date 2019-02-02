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
            ListGroup<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                writer,
                GRUP_RECORD_TYPE.TypeInt,
                errorMask: errorMask);
        }
    }

    public static class ListGroupExt
    {
        public static async Task Create_Xml_Folder<T>(
            this ListGroup<T> group,
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
                        var path = Path.Combine(dir.Path, $"{name}.xml");
                        if (!File.Exists(path))
                        {
                            group.Clear();
                            return;
                        }
                        foreach (var item in dir.EnumerateFiles())
                        {
                            if (!item.Info.Extension.Equals("xml"))
                            {
                                continue;
                            }

                            var val = LoquiXmlFolderTranslation<T>.CREATE.Value(
                                item.Path,
                                null);
                            group.Items.Add(val);
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
                        int counter = 0;
                        foreach (var item in list.Items.Items)
                        {
                            using (errorMask.PushIndex(counter))
                            {
                                try
                                {
                                    item.Write_Xml_Folder(
                                        node: null,
                                        name: name,
                                        counter: counter,
                                        dir: dir,
                                        errorMask: errorMask);
                                }
                                catch (Exception ex)
                                when (errorMask != null)
                                {
                                    errorMask.ReportException(ex);
                                }
                            }
                            counter++;
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
    }
}
