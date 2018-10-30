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
            NotifyingDictionary<FormKey, IMajorRecord> dict,
            ChangeAddRem<T> change)
            where T : IMajorRecord
        {
            if (change.AddRem == Noggog.AddRemove.Add
                && dict.ContainsKey(change.Item.FormKey))
            {
                throw new ArgumentException($"Cannot add a {typeof(T).Name} {change.Item.FormKey} that exists elsewhere in the same mod.");
            }
            dict.Modify(change.Item.FormKey, change.Item, change.AddRem);
        }

        public static void ModifyButThrow<T>(
            NotifyingDictionary<FormKey, IMajorRecord> dict,
            ChangeAddRem<KeyValuePair<FormKey, T>> change)
            where T : IMajorRecord
        {
            if (change.AddRem == Noggog.AddRemove.Add
                && dict.ContainsKey(change.Item.Key))
            {
                throw new ArgumentException($"Cannot add a {typeof(T).Name} {change.Item.Key} that exists elsewhere in the same mod.");
            }
            dict.Modify(change.Item.Key, change.Item.Value, change.AddRem);
        }

        public static void Modify<T>(
            NotifyingDictionary<FormKey, IMajorRecord> dict,
            ChangeSet<T> change)
            where T : IMajorRecord
        {
            if (change.Old != null)
            {
                dict.Remove(change.Old.FormKey);
            }
            if (change.New != null)
            {
                dict.Modify(change.New.FormKey, change.New, Noggog.AddRemove.Add);
            }
        }
    }
}
