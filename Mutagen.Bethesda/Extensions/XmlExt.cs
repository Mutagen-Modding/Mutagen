using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public static class XmlExt
    {
        /*
         * Attempt to make saving masses of XML faster by checking the disk if they changed before writing
         */
        public static void SaveIfChanged(this XElement elem, string path)
        {
            if (File.Exists(path))
            {
                MemoryStream data = new MemoryStream();
                elem.Save(data);
                data.Position = 0;

                File.SetLastAccessTime(path, DateTime.Now);

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    if (fs.ContentsEqual(data)) return;
                }

                data.Position = 0;
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
                {
                    data.CopyTo(fs);
                }
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.None))
                {
                    elem.Save(fs);
                }

                File.SetLastAccessTime(path, DateTime.Now);
            }
        }
    }
}
