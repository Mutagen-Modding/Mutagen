using Loqui;
using Mutagen.Bethesda.Binary;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class LoquiBinaryWrapperTranslation<T>
    {
        public delegate T CreateFunc(
            BinaryMemoryReadStream stream,
            MasterReferences masterReferences,
            MetaDataConstants meta);
        public static readonly CreateFunc Create = GetCreateFunc();

        private static CreateFunc GetCreateFunc()
        {
            var regis = LoquiRegistration.GetRegister(typeof(T));
            var className = $"{regis.Namespace}.Internals.{regis.Name}BinaryWrapper";

            var tType = regis.ClassType.Assembly.GetType(className);
            var method = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals($"{regis.Name}Factory"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) =>
                {
                    var param = methodInfo.GetParameters();
                    if (param.Length != 3) return false;
                    if (!param[0].ParameterType.Equals(typeof(BinaryMemoryReadStream))) return false;
                    if (!param[1].ParameterType.Equals(typeof(MasterReferences))) return false;
                    if (!param[2].ParameterType.Equals(typeof(MetaDataConstants))) return false;
                    return true;
                })
                .FirstOrDefault();
            if (method != null)
            {
                return DelegateBuilder.BuildDelegate<CreateFunc>(method);
            }
            else
            {
                return null;
            }
        }
    }
}
