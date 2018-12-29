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

namespace Mutagen.Bethesda
{
    public partial class Group<T> : IEnumerable<T>
        where T : ILoquiObject<T>, IMajorRecord
    {
        private Lazy<IObservableCache<T, string>> _editorIDCache;
        public IObservableCache<T, string> ByEditorID => _editorIDCache.Value;

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

        static partial void FillBinary_ContainedRecordType_Custom(
            MutagenFrame frame,
            Group<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            frame.Reader.Position += 4;
        }

        static partial void WriteBinary_ContainedRecordType_Custom(
            MutagenWriter writer,
            Group<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
                writer,
                GRUP_RECORD_TYPE.TypeInt,
                errorMask: errorMask);
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
        public static async Task Create_Xml_Folder<T>(
            this Group<T> group,
            DirectoryPath dir,
            ErrorMaskBuilder errorMask,
            int index)
            where T : MajorRecord, ILoquiObject<T>, IFormKey
        {
            try
            {
                errorMask?.PushIndex(index);
                try
                {
                    errorMask?.PushIndex((int)Group_FieldIndex.Items);
                    foreach (var item in dir.EnumerateFiles())
                    {
                        if (!item.Info.Extension.Equals(".xml"))
                        {
                            continue;
                        }

                        var val = LoquiXmlFolderTranslation<T>.CREATE.Value(
                            item.Path,
                            null);
                        group.Items.Set(val);
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
                finally
                {
                    errorMask?.PopIndex();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public static async Task Write_Xml_Folder<T, T_ErrMask>(
            this Group<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : MajorRecord, ILoquiObject<T>, IFormKey
            where T_ErrMask : MajorRecord_ErrorMask, IErrorMask<T_ErrMask>, new()
        {
            try
            {
                errorMask?.PushIndex(index);
                try
                {
                    errorMask?.PushIndex((int)Group_FieldIndex.Items);
                    dir.Create();
                    XElement topNode = new XElement("topnode");
                    int counter = 0;
                    foreach (var item in group.Items.Items)
                    {
                        using (errorMask.PushIndex(counter))
                        {
                            try
                            {
                                item.Write_Xml_Folder(
                                    node: topNode,
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
                    if (topNode.HasElements)
                    {
                        topNode.SaveIfChanged(Path.Combine(dir.Path, $"{name}.xml"));
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
                finally
                {
                    errorMask?.PopIndex();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }
    }
}
