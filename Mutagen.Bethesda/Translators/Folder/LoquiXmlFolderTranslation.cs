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
        public delegate Task<T> CREATE_FUNC(
            XElement node,
            string path,
            ErrorMaskBuilder errorMask,
            TranslationCrystal translationMask);
        private static readonly Lazy<CREATE_FUNC> create = new Lazy<CREATE_FUNC>(GetCreateFunc);
        public static CREATE_FUNC CREATE
        {
            get
            {
                lock (create)
                {
                    return create.Value;
                }
            }
        }

        private static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var options = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("CreateFromXmlFolder"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.ReturnType.Equals(typeof(Task<T>)))
                .Where((methodInfo) => methodInfo.GetParameters().Length == 4)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(XElement)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(string)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(ErrorMaskBuilder)))
                .Where((methodInfo) => methodInfo.GetParameters()[3].ParameterType.Equals(typeof(TranslationCrystal)))
                .ToArray();
            var method = options
                .FirstOrDefault();
            if (method != null)
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
            else
            {
                return async (node, path, errorMask, translMask) =>
                {
                    if (LoquiXmlTranslation<T>.Instance.Parse(
                        node,
                        out var item,
                        errorMask: errorMask,
                        translationMask: translMask))
                    {
                        return item;
                    }
                    throw new NotImplementedException();
                };
            }
        }
    }
}
