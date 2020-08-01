using Loqui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Mutagen.Bethesda.Internals
{    
    /// <summary>
    /// A static class encapsulating the job of creating a new Mod in a generic context
    /// </summary>
    /// <typeparam name="TMod">
    /// Type of Mod to instantiate.  Can be the direct class, or one of its interfaces.
    /// </typeparam>
    public static class ModInstantiator<TMod>
        where TMod : IModGetter
    {
        /// <summary>
        /// Function to call to retrieve a new Mod of type T
        /// </summary>
        public static readonly Func<ModKey, TMod> Activator;

        /// <summary>
        /// Function to call to import a new Mod of type T
        /// </summary>
        public static readonly Func<ModPath, TMod> Importer;

        static ModInstantiator()
        {
            if (!LoquiRegistration.TryGetRegister(typeof(TMod), out var regis))
            {
                throw new ArgumentException();
            }
            Activator = GetActivator(regis);
            Importer = GetImporter(regis);
        }

        private static Func<ModKey, TMod> GetActivator(ILoquiRegistration regis)
        {
            var ctorInfo = regis.ClassType.GetConstructors()
                .Where(c => c.GetParameters().Length == 1)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(ModKey))
                .First();
            var paramInfo = ctorInfo.GetParameters();
            ParameterExpression param = Expression.Parameter(typeof(ModKey), "modKey");
            NewExpression newExp = Expression.New(ctorInfo, param);
            LambdaExpression lambda = Expression.Lambda(typeof(Func<ModKey, TMod>), newExp, param);
            return (Func<ModKey, TMod>)lambda.Compile();
        }

        private static Func<ModPath, TMod> GetImporter(ILoquiRegistration regis)
        {
            if (regis.ClassType == typeof(TMod)
                || regis.SetterType == typeof(TMod))
            {
                var methodInfo = regis.ClassType.GetMethods()
                    .Where(m => m.Name == "CreateFromBinary")
                    .Where(c => c.GetParameters().Length == 3)
                    .Where(c => c.GetParameters()[0].ParameterType == typeof(ModPath))
                    .First();
                var paramInfo = methodInfo.GetParameters();
                ParameterExpression param = Expression.Parameter(typeof(ModPath), "modKey");
                ParameterExpression param2 = Expression.Parameter(paramInfo[1].ParameterType, "importMask");
                ParameterExpression param3 = Expression.Parameter(typeof(bool), "parallel");
                MethodCallExpression callExp = Expression.Call(methodInfo, param, param2, param3);
                var funcType = Expression.GetFuncType(typeof(ModPath), paramInfo[1].ParameterType, typeof(bool), typeof(TMod));
                LambdaExpression lambda = Expression.Lambda(funcType, callExp, param, param2, param3);
                var deleg = lambda.Compile();
                return (ModPath modPath) =>
                {
                    return (TMod)deleg.DynamicInvoke(modPath, null, true);
                };
            }
            else if (regis.GetterType == typeof(TMod))
            {
                var methodInfo = regis.ClassType.GetMethod(
                    "CreateFromBinaryOverlay",
                    new Type[]
                    {
                        typeof(ModPath)
                    });
                var paramInfo = methodInfo.GetParameters();
                ParameterExpression param = Expression.Parameter(typeof(ModPath), "modKey");
                MethodCallExpression callExp = Expression.Call(methodInfo, param);
                LambdaExpression lambda = Expression.Lambda(typeof(Func<ModPath, TMod>), callExp, param);
                return (Func<ModPath, TMod>) lambda.Compile();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
