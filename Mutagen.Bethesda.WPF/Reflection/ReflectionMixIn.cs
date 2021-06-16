using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Mutagen.Bethesda.WPF.Reflection
{
    public static class ReflectionMixIn
    {
        public static T GetCustomAttributeValueByName<T>(this MemberInfo info, string attrName, string valName, T fallback)
        {
            if (!TryGetCustomAttributeByName(info, attrName, out var attr)) return fallback;
            var propInfo = attr.GetType().GetProperty(valName);
            if (propInfo == null) return fallback;
            return (T)propInfo.GetValue(attr)!;
        }

        public static bool TryGetCustomAttributeByName(this MemberInfo info, string name, [MaybeNullWhen(false)] out Attribute attr)
        {
            attr = Attribute.GetCustomAttributes(info).FirstOrDefault(a => IsNamed(a, name));
            return attr != null;
        }

        public static IEnumerable<Attribute> GetCustomAttributesByName(this MemberInfo info, string name)
        {
            return Attribute.GetCustomAttributes(info).Where(a => IsNamed(a, name));
        }

        public static bool IsNamed(Attribute a, string name)
        {
            var type = a.GetType();
            if (type.Name == name) return true;
            if (type.BaseType == null) return false;
            return IsNamed(type.BaseType, name);
        }

        public static bool IsNamed(Type type, string name)
        {
            if (type.Name == name) return true;
            if (type.BaseType == null) return false;
            return IsNamed(type.BaseType, name);
        }
    }
}
