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
            where T : IMajorRecord
        {
            fg.AppendLine($"{name} [{typeof(T).Name}] ({link.FormID})");
        }

        public static string ToString<T>(
            this ILink<T> link)
            where T : IMajorRecord
        {
            return $"{(link.Linked ? link.Item.EditorID : "UNLINKED")} [{typeof(T).Name}] ({link.FormID})";
        }

        public static string ToString<T>(
            this IEDIDLink<T> link)
            where T : IMajorRecord
        {
            return $"{(link.Linked ? link.Item.EditorID : "UNLINKED")} [{typeof(T).Name}] ({link.FormID}) ({link.EDID})";
        }

        public static bool Equals(this ILink lhs, ILink rhs)
        {
            return lhs.FormID.Equals(rhs.FormID);
        }

        public static bool Equals<T, R>(this ILink<T> lhs, object rhs)
            where T : IMajorRecord
        {
            if (!(rhs is ILink rhsLink)) return false;
            return Equals(lhs, rhsLink);
        }

        public static int HashCode(this ILink lhs)
        {
            return lhs.FormID.GetHashCode();
        }

        public static FormID GetFormID<T>(IFormIDLink<T> link)
            where T : IMajorRecord
        {
            FormID? ret = link.Item?.FormID ?? link.UnlinkedForm;
            return ret ?? FormID.NULL;
        }
    }
}
