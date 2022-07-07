using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System.IO.Abstractions;
using System.Linq.Expressions;
using DynamicData;
using Loqui;
using Mutagen.Bethesda.Plugins.Records.Loqui;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Records
{
    /// <summary>
    /// A static class encapsulating the job of creating a new Mod in a generic context
    /// </summary>
    public static class ModInstantiator
    {
        private static Dictionary<GameCategory, ModInstantiator<IModGetter>.ImporterDelegate> _dict = new();

        static ModInstantiator()
        {
            foreach (var category in EnumExt<GameCategory>.Values)
            {
                var t = Type.GetType(
                    $"Mutagen.Bethesda.{category}.{category}Mod_Registration, Mutagen.Bethesda.{category}");
                if (t == null) continue;
                var obj = Activator.CreateInstance(t);
                var modRegistration = obj as IModRegistration;
                if (modRegistration == null) continue;
                _dict[modRegistration.GameCategory] = ModInstantiatorReflection.GetOverlay<IModGetter>(modRegistration);

            }
        }

        public static IModGetter Importer(ModPath path, GameRelease release, IFileSystem? fileSystem = null, StringsReadParameters? stringsParam = null)
        {
            return _dict[release.ToCategory()](path, release, fileSystem, stringsParam);
        }
    }
    
    /// <summary>
    /// A static class encapsulating the job of creating a new Mod in a generic context
    /// </summary>
    /// <typeparam name="TMod">
    /// Type of Mod to instantiate.  Can be the direct class, or one of its interfaces.
    /// </typeparam>
    public static class ModInstantiator<TMod>
        where TMod : IModGetter
    {
        public delegate TMod ActivatorDelegate(ModKey modKey, GameRelease release);
        public delegate TMod ImporterDelegate(ModPath modKey, GameRelease release, IFileSystem? fileSystem = null, StringsReadParameters? stringsParam = null);

        /// <summary>
        /// Function to call to retrieve a new Mod of type T
        /// </summary>
        public static readonly ActivatorDelegate Activator;

        /// <summary>
        /// Function to call to import a new Mod of type T
        /// </summary>
        public static readonly ImporterDelegate Importer;
        //
        // /// <summary>
        // /// Function to call to import a new Mod of type T
        // /// </summary>
        // public static readonly Func<ModPath, GameRelease, IFileSystem?, TMod> FileSystemImporter;

        static ModInstantiator()
        {
            if (!LoquiRegistration.TryGetRegister(typeof(TMod), out var regis))
            {
                throw new ArgumentException();
            }
            Activator = ModInstantiatorReflection.GetActivator<TMod>(regis);
            Importer = ModInstantiatorReflection.GetImporter<TMod>(regis);
        }
    }

    internal static class ModInstantiatorReflection
    {
        internal static ModInstantiator<TMod>.ActivatorDelegate GetActivator<TMod>(ILoquiRegistration regis)
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
                return (ModKey modKey, GameRelease release) => { return (TMod)deleg.DynamicInvoke(modKey)!; };
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
                    return (TMod)deleg.DynamicInvoke(modKey, (int)release)!;
                };
            }
        }

        public static ModInstantiator<TMod>.ImporterDelegate GetImporter<TMod>(ILoquiRegistration regis)
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
                var funcType =
                    Expression.GetFuncType(paramInfo.Select(p => p.ParameterType).And(typeof(TMod)).ToArray());
                LambdaExpression lambda = Expression.Lambda(funcType, callExp, paramExprs);
                var deleg = lambda.Compile();
                var releaseIndex = paramInfo.Select(x => x.Name).IndexOf("release");
                var fileSystemIndex = paramInfo.Select(x => x.Name).IndexOf("fileSystem");
                var stringsParamIndex = paramInfo.Select(x => x.Name).IndexOf("stringsParam");
                var parallelIndex = paramInfo.Select(x => x.Name).IndexOf("parallel");
                return (ModPath modPath, GameRelease release, IFileSystem? fileSystem,
                    StringsReadParameters? stringsParam) =>
                {
                    var args = new object?[paramInfo.Length];
                    args[0] = modPath;
                    if (releaseIndex != -1)
                    {
                        args[releaseIndex] = release;
                    }

                    if (stringsParamIndex != -1)
                    {
                        args[stringsParamIndex] = stringsParam;
                    }

                    args[parallelIndex] = true;
                    args[fileSystemIndex] = fileSystem;
                    return (TMod)deleg.DynamicInvoke(args)!;
                };
            }
            else if (regis.GetterType == typeof(TMod))
            {
                var overlayGet = GetOverlay<TMod>(regis);
                return (ModPath modPath, GameRelease release, IFileSystem? fileSystem,
                    StringsReadParameters? stringsParam) =>
                {
                    return overlayGet(modPath, release, fileSystem, stringsParam);
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static ModInstantiator<TMod>.ImporterDelegate GetOverlay<TMod>(ILoquiRegistration regis)
            where TMod : IModGetter
        {
            var methodInfo = regis.ClassType.GetMethods()
                .Where(m => m.Name == "CreateFromBinaryOverlay")
                .Where(c => c.GetParameters().Length >= 1)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(ModPath))
                .First();
            var paramInfo = methodInfo.GetParameters();
            var paramExprs = paramInfo.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
            MethodCallExpression callExp = Expression.Call(methodInfo, paramExprs);
            var funcType =
                Expression.GetFuncType(paramInfo.Select(p => p.ParameterType).And(regis.GetterType).ToArray());
            LambdaExpression lambda = Expression.Lambda(funcType, callExp, paramExprs);
            var deleg = lambda.Compile();
            var releaseIndex = paramInfo.Select(x => x.Name).IndexOf("release");
            var fileSystemIndex = paramInfo.Select(x => x.Name).IndexOf("fileSystem");
            var stringsParamIndex = paramInfo.Select(x => x.Name).IndexOf("stringsParam");
            return (ModPath modPath, GameRelease release, IFileSystem? fileSystem,
                StringsReadParameters? stringsParam) =>
            {
                var args = new object?[paramInfo.Length];
                args[0] = modPath;
                if (releaseIndex != -1)
                {
                    args[releaseIndex] = release;
                }

                if (stringsParamIndex != -1)
                {
                    args[stringsParamIndex] = stringsParam;
                }

                args[fileSystemIndex] = fileSystem;
                return (TMod)deleg.DynamicInvoke(args)!;
            };
        }
    }
}
