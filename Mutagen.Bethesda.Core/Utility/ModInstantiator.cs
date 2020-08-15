using Loqui;
using Noggog;
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
        public static readonly Func<ModKey, GameRelease, TMod> Activator;

        /// <summary>
        /// Function to call to import a new Mod of type T
        /// </summary>
        public static readonly Func<ModPath, GameRelease, TMod> Importer;

        static ModInstantiator()
        {
            if (!LoquiRegistration.TryGetRegister(typeof(TMod), out var regis))
            {
                throw new ArgumentException();
            }
            Activator = ModInstantiator.GetActivator<TMod>(regis);
            Importer = ModInstantiator.GetImporter<TMod>(regis);
        }
    }

    internal static class ModInstantiator
    {
        public static Func<ModKey, GameRelease, TMod> GetActivator<TMod>(ILoquiRegistration regis)
            where TMod : IModGetter
        {
            var ctorInfo = regis.ClassType.GetConstructors()
                .Where(c => c.GetParameters().Length >= 1)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(ModKey))
                .First();
            var paramInfo = ctorInfo.GetParameters();
            ParameterExpression modKeyParam = Expression.Parameter(typeof(ModKey), "modKey");
            if (paramInfo.Length == 1)
            {
                NewExpression newExp = Expression.New(ctorInfo, modKeyParam);
                LambdaExpression lambda = Expression.Lambda(typeof(Func<ModKey, TMod>), newExp, modKeyParam);
                var deleg = lambda.Compile();
                return (ModKey modKey, GameRelease release) =>
                {
                    return (TMod)deleg.DynamicInvoke(modKey);
                };
            }
            else
            {
                ParameterExpression releaseParam = Expression.Parameter(paramInfo[1].ParameterType, "release");
                NewExpression newExp = Expression.New(ctorInfo, modKeyParam, releaseParam);
                var funcType = Expression.GetFuncType(typeof(ModKey), paramInfo[1].ParameterType, typeof(TMod));
                LambdaExpression lambda = Expression.Lambda(funcType, newExp, modKeyParam, releaseParam);
                var deleg = lambda.Compile();
                return (ModKey modKey, GameRelease release) =>
                {
                    return (TMod)deleg.DynamicInvoke(modKey, (int)release);
                };
            }
        }

        public static Func<ModPath, GameRelease, TMod> GetImporter<TMod>(ILoquiRegistration regis)
            where TMod : IModGetter
        {
            if (regis.ClassType == typeof(TMod)
                || regis.SetterType == typeof(TMod))
            {
                var methodInfo = regis.ClassType.GetMethods()
                    .Where(m => m.Name == "CreateFromBinary")
                    .Where(c => c.GetParameters().Length >= 3)
                    .Where(c => c.GetParameters()[0].ParameterType == typeof(ModPath))
                    .First();
                var paramInfo = methodInfo.GetParameters();
                var paramExprs = paramInfo.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
                MethodCallExpression callExp = Expression.Call(methodInfo, paramExprs);
                var funcType = Expression.GetFuncType(paramInfo.Select(p => p.ParameterType).And(typeof(TMod)).ToArray());
                LambdaExpression lambda = Expression.Lambda(funcType, callExp, paramExprs);
                var deleg = lambda.Compile();
                if (paramInfo[1].Name == "release")
                {
                    return (ModPath modPath, GameRelease release) =>
                    {
                        object[] args = new object[paramInfo.Length];
                        args[0] = modPath;
                        args[1] = release;
                        args[^1] = true;
                        return (TMod)deleg.DynamicInvoke(args);
                    };
                }
                else
                {
                    return (ModPath modPath, GameRelease release) =>
                    {
                        object[] args = new object[paramInfo.Length];
                        args[0] = modPath;
                        args[^1] = true;
                        return (TMod)deleg.DynamicInvoke(args);
                    };
                }
            }
            else if (regis.GetterType == typeof(TMod))
            {
                var methodInfo = regis.ClassType.GetMethods()
                    .Where(m => m.Name == "CreateFromBinaryOverlay")
                    .Where(c => c.GetParameters().Length >= 1)
                    .Where(c => c.GetParameters()[0].ParameterType == typeof(ModPath))
                    .First();
                var paramInfo = methodInfo.GetParameters();
                var paramExprs = paramInfo.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
                MethodCallExpression callExp = Expression.Call(methodInfo, paramExprs);
                var funcType = Expression.GetFuncType(paramInfo.Select(p => p.ParameterType).And(typeof(TMod)).ToArray());
                LambdaExpression lambda = Expression.Lambda(funcType, callExp, paramExprs);
                var deleg = lambda.Compile();
                if (paramInfo.Length > 1 && paramInfo[1].Name == "release")
                {
                    return (ModPath modPath, GameRelease release) =>
                    {
                        object[] args = new object[paramInfo.Length];
                        args[0] = modPath;
                        args[1] = release;
                        return (TMod)deleg.DynamicInvoke(args);
                    };
                }
                else
                {
                    return (ModPath modPath, GameRelease release) =>
                    {
                        object[] args = new object[paramInfo.Length];
                        args[0] = modPath;
                        return (TMod)deleg.DynamicInvoke(args);
                    };
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
