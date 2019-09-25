using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class FolderTranslation
    {
        public static bool TryGetItemIndex(string str, out int index)
        {
            string[] split = str.Split('-');
            if (split.Length < 2
                || !int.TryParse(split[0].Trim(), out index))
            {
                index = -1;
                return false;
            }
            return true;
        }

        public static string GetFileString(IMajorRecordCommonGetter rec, int counter)
        {
            return $"{counter} - {rec.EditorID} - {rec.FormKey.ToString()}";
        }
    }
}
