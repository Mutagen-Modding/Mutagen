using Loqui;
using Loqui.Internal;
using Loqui.Xml;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Folder
{
    public class LoquiXmlFolderTranslation<T>
        where T : ILoquiObjectGetter
    {
        public delegate T CREATE_FUNC(
            string path,
            ErrorMaskBuilder errorMask);
        public static readonly Lazy<CREATE_FUNC> CREATE = new Lazy<CREATE_FUNC>(GetCreateFunc);

        public static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var options = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("Create_XmlFolder"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.ReturnType.Equals(tType))
                .Where((methodInfo) => methodInfo.GetParameters().Length == 2)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(string)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(ErrorMaskBuilder)))
                .ToArray();
            var method = options
                .FirstOrDefault();
            if (method != null)
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
            else
            {
                return (path, errorMask) =>
                {
                    if (LoquiXmlTranslation<T>.Instance.Parse(
                        XElement.Load(path),
                        out var item,
                        errorMask: errorMask,
                        translationMask: null))
                    {
                        return item;
                    }
                    throw new NotImplementedException();
                };
            }
        }
    }
}
