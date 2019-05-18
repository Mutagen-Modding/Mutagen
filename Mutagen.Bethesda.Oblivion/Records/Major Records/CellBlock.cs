using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CellBlock : IXmlFolderItem
    {
        public static CellBlock_CopyMask duplicateMask = new CellBlock_CopyMask(true)
        {
            Items = new Loqui.MaskItem<Loqui.CopyOption, CellSubBlock_CopyMask>(Loqui.CopyOption.Skip, null)
        };

        public async Task Write_Xml_Folder(
            DirectoryPath? dir,
            string name,
            XElement node,
            int counter,
            ErrorMaskBuilder errorMask)
        {
            var subDir = Path.Combine(dir.Value.Path, $"{this.BlockNumber}");
            Directory.CreateDirectory(subDir);
            this.Write_Xml(
                Path.Combine(subDir, "Group.xml"),
                errorMask: errorMask,
                translationMask: GroupExt.XmlFolderTranslationCrystal);
            int blockCounter = 0;
            List<Task> tasks = new List<Task>();
            foreach (var item in this.Items)
            {
                int stampedCounter = blockCounter++;
                tasks.Add(Task.Run(() =>
                {
                    item.Write_Xml_Folder(
                        path: Path.Combine(subDir, $"{stampedCounter.ToString()}.xml"),
                        errorMask: errorMask);
                }));
            }
            await Task.WhenAll(tasks);
        }

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecordTracker = null)
        {
            var ret = new CellBlock();
            ret.CopyFieldsFrom(this, duplicateMask);
            ret.Items.SetTo(this.Items.Select(i => (CellSubBlock)i.Duplicate(getNextFormKey, duplicatedRecordTracker)));
            return ret;
        }

        public static async Task<CellBlock> Create_XmlFolder(
            XElement node,
            string path,
            ErrorMaskBuilder errorMask,
            TranslationCrystal translationMask)
        {
            CellBlock ret = new CellBlock();
            var groupPath = Path.Combine(path, $"Group.xml");
            if (File.Exists(groupPath))
            {
                XElement elem = XElement.Load(groupPath);
                ret.FillPublic_Xml(
                    elem,
                    errorMask,
                    translationMask: GroupExt.XmlFolderTranslationCrystal);
            }
            var dir = new DirectoryPath(path);
            List<Task<CellSubBlock>> tasks = new List<Task<CellSubBlock>>();
            foreach (var f in dir.EnumerateFiles(recursive: false)
                .SelectWhere(subDir =>
                {
                    if (int.TryParse(subDir.NameWithoutExtension, out var i))
                    {
                        return TryGet<(int Index, FilePath File)>.Succeed((i, subDir));
                    }
                    else
                    {
                        return TryGet<(int Index, FilePath File)>.Failure;
                    }
                })
                .OrderBy(i => i.Index))
            {
                tasks.Add(Task.Run(() => CellSubBlock.Create_Xml_Folder(f.File, f.Index)));
            }
            var subBlocks = await Task.WhenAll(tasks);
            ret.Items.AddRange(subBlocks);
            return ret;
        }
    }
}

