using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class Utility
    {
        public static void ModifyButThrow<T>(
            NotifyingDictionary<FormID, IMajorRecord> dict,
            ChangeAddRem<T> change)
            where T : IMajorRecord
        {
            if (dict.ContainsKey(change.Item.FormID))
            {
                throw new ArgumentException($"Cannot add a {typeof(T).Name} {change.Item.FormID} that exists elsewhere in the same mod.");
            }
            dict.Modify(change.Item.FormID, change.Item, change.AddRem);
        }

        public static void ModifyButThrow<T>(
            NotifyingDictionary<FormID, IMajorRecord> dict,
            ChangeAddRem<KeyValuePair<FormID, T>> change)
            where T : IMajorRecord
        {
            if (dict.ContainsKey(change.Item.Key))
            {
                throw new ArgumentException($"Cannot add a {typeof(T).Name} {change.Item.Key} that exists elsewhere in the same mod.");
            }
            dict.Modify(change.Item.Key, change.Item.Value, change.AddRem);
        }

        public static void Modify<T>(
            NotifyingDictionary<FormID, IMajorRecord> dict,
            ChangeSet<T> change)
            where T : IMajorRecord
        {
            if (change.Old != null)
            {
                dict.Remove(change.Old.FormID);
            }
            if (change.New != null)
            {
                dict.Modify(change.New.FormID, change.New, Noggog.AddRemove.Add);
            }
        }
    }
}
