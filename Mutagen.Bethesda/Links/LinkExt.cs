using Loqui;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class LinkExt
    {
        public static void ToString<T>(
            this ILink<T> link,
            FileGeneration fg,
            string name)
            where T : MajorRecord
        {
            fg.AppendLine($"{name} [{typeof(T).Name}] ({link.FormKey})");
        }

        public static string ToString<T>(
            this ILink<T> link)
            where T : MajorRecord
        {
            return $"{(link.Linked ? link.Item.EditorID : "UNLINKED")} [{typeof(T).Name}] ({link.FormKey})";
        }

        public static string ToString<T>(
            this IEDIDLink<T> link)
            where T : MajorRecord
        {
            return $"{(link.Linked ? link.Item.EditorID : "UNLINKED")} [{typeof(T).Name}] ({link.FormKey}) ({link.EDID})";
        }

        public static bool Equals(this ILink lhs, ILink rhs)
        {
            return lhs.FormKey.Equals(rhs.FormKey);
        }

        public static bool Equals<T, R>(this ILink<T> lhs, object rhs)
            where T : MajorRecord
        {
            if (!(rhs is ILink rhsLink)) return false;
            return Equals(lhs, rhsLink);
        }

        public static int HashCode(this ILink lhs)
        {
            return lhs.FormKey.GetHashCode();
        }

        public static FormKey GetFormKey<T>(IFormIDLink<T> link)
            where T : MajorRecord
        {
            FormKey? ret = link.Item?.FormKey ?? link.UnlinkedForm;
            return ret ?? FormKey.NULL;
        }
    }
}
