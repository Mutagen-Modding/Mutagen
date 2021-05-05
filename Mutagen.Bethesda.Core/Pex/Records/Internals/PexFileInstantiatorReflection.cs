using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Loqui;
using Noggog;

namespace Mutagen.Bethesda.Pex.Records.Internals
{
    public static class PexFileInstantiatorReflection
    {
        public static Func<Stream, IPexFileCommon> GetImporter(ILoquiRegistration regis)
        {
            var methodInfo = regis.ClassType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(c =>
                {
                    var param = c.GetParameters();
                    if (param.Length != 1) return false;
                    if (param[0].ParameterType != typeof(Stream)) return false;
                    return true;
                })
                .First();
            var paramInfo = methodInfo.GetParameters();
            var paramExprs = paramInfo.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
            MethodCallExpression callExp = Expression.Call(methodInfo, paramExprs);
            var funcType = Expression.GetFuncType(paramInfo.Select(p => p.ParameterType).And(typeof(IPexFileCommon)).ToArray());
            LambdaExpression lambda = Expression.Lambda(funcType, callExp, paramExprs);
            return (Func<Stream, IPexFileCommon>)lambda.Compile();
        }
    }
}
