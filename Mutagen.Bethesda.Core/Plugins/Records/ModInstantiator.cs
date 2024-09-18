using Noggog;
using System.Linq.Expressions;
using System.Reflection;
using DynamicData;
using Loqui;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records.Loqui;

namespace Mutagen.Bethesda.Plugins.Records
{
    /// <summary>
    /// A static class encapsulating the job of creating a new Mod in a generic context
    /// </summary>
    public static class ModInstantiator
    {
        record Delegates(
            ModInstantiator<IModDisposeGetter>.ImporterDelegate ImportGetter, 
            ModInstantiator<IMod>.ImporterDelegate ImportSetter, 
            ModInstantiator<IMod>.ActivatorDelegate Activator);
        
        private static Dictionary<GameCategory, Delegates> _dict = new();

        static ModInstantiator()
        {
            foreach (var category in Enums<GameCategory>.Values)
            {
                var t = Type.GetType(
                    $"Mutagen.Bethesda.{category}.{category}Mod_Registration, Mutagen.Bethesda.{category}");
                if (t == null) continue;
                var obj = System.Activator.CreateInstance(t);
                var modRegistration = obj as IModRegistration;
                if (modRegistration == null) continue;
                _dict[modRegistration.GameCategory] = new Delegates(
                    ModInstantiatorReflection.GetOverlay<IModDisposeGetter>(modRegistration),
                    ModInstantiatorReflection.GetImporter<IMod>(modRegistration),
                    ModInstantiatorReflection.GetActivator<IMod>(modRegistration));

            }
        }

        [Obsolete("Use ImportGetter")]
        public static IModDisposeGetter Importer(ModPath path, GameRelease release, BinaryReadParameters? param = null)
        {
            return _dict[release.ToCategory()].ImportGetter(path, release, param);
        }

        public static IModDisposeGetter ImportGetter(ModPath path, GameRelease release, BinaryReadParameters? param = null)
        {
            return _dict[release.ToCategory()].ImportGetter(path, release, param);
        }

        public static IMod ImportSetter(ModPath path, GameRelease release, BinaryReadParameters? param = null)
        {
            return _dict[release.ToCategory()].ImportSetter(path, release, param);
        }

        public static IMod Activator(ModKey modKey, GameRelease release, float? headerVersion = null, bool? forceUseLowerFormIDRanges = false)
        {
            return _dict[release.ToCategory()].Activator(modKey, release, headerVersion: headerVersion, forceUseLowerFormIDRanges: forceUseLowerFormIDRanges);
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
        public delegate TMod ActivatorDelegate(ModKey modKey, GameRelease release, float? headerVersion = null, bool? forceUseLowerFormIDRanges = false);
        public delegate TMod ImporterDelegate(ModPath modKey, GameRelease release, BinaryReadParameters? param = null);

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
            bool createActivator = true;
            var type = typeof(TMod);
            if (type.Name.EndsWith("DisposableGetter"))
            {
                var className = type.Name.TrimEnd("DisposableGetter") + "Getter";
                type = Type.GetType($"{type.Namespace}.{className}, {type.Namespace}");
                createActivator = false;
            }
            
            if (type == null || !LoquiRegistration.TryGetRegister(type, out var regis))
            {
                throw new ArgumentException();
            }

            if (createActivator)
            {
                Activator = ModInstantiatorReflection.GetActivator<TMod>(regis);
            }
            else
            {
                Activator = (Key, Release, Version, Ranges) =>
                {
                    throw new ArgumentException($"Cannot create a new mod of type {type}");
                };
            }
            if (typeof(TMod).InheritsFrom(typeof(IMod)))
            {
                Importer = ModInstantiatorReflection.GetImporter<TMod>(regis);
            }
            else
            {
                Importer = ModInstantiatorReflection.GetOverlay<TMod>(regis);
            }
        }
    }

    internal static class ModInstantiatorReflection
    {
        internal static ModInstantiator<TMod>.ActivatorDelegate GetActivator<TMod>(ILoquiRegistration regis)
            where TMod : IModGetter
        {
            var ctorInfo = regis.ClassType.GetConstructors()
                .Where(c => c.GetParameters().Length >= 3)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(ModKey))
                .First();
            var paramInfo = ctorInfo.GetParameters();
            ParameterExpression modKeyParam = Expression.Parameter(typeof(ModKey), "modKey");
            ParameterExpression headerVersionParam = Expression.Parameter(typeof(float?), "headerVersion");
            ParameterExpression forceUseLowerFormIDRangesParam = Expression.Parameter(typeof(bool?), "forceUseLowerFormIDRanges");
            if (paramInfo.Length == 3)
            {
                NewExpression newExp = Expression.New(ctorInfo, modKeyParam, headerVersionParam, forceUseLowerFormIDRangesParam);
                LambdaExpression lambda = Expression.Lambda(typeof(Func<ModKey, float?, bool?, TMod>), newExp, modKeyParam, headerVersionParam, forceUseLowerFormIDRangesParam);
                var deleg = lambda.Compile();
                return (ModKey modKey, GameRelease release, float? headerVersion = null, bool? forceUseLowerFormIDRanges = false) =>
                {
                    return (TMod)deleg.DynamicInvoke(modKey, headerVersion, forceUseLowerFormIDRanges)!;
                };
            }
            else
            {
                ParameterExpression releaseParam = Expression.Parameter(paramInfo[1].ParameterType, "release");
                NewExpression newExp = Expression.New(ctorInfo, modKeyParam, releaseParam, headerVersionParam, forceUseLowerFormIDRangesParam);
                var funcType = Expression.GetFuncType(typeof(ModKey), paramInfo[1].ParameterType, typeof(float?), typeof(bool?), typeof(TMod));
                LambdaExpression lambda = Expression.Lambda(funcType, newExp, modKeyParam, releaseParam, headerVersionParam, forceUseLowerFormIDRangesParam);
                var deleg = lambda.Compile();
                return (ModKey modKey, GameRelease release, float? headerVersion = null, bool? forceUseLowerFormIDRanges = false) =>
                {
                    return (TMod)deleg.DynamicInvoke(modKey, (int)release, headerVersion, forceUseLowerFormIDRanges)!;
                };
            }
        }

        public static ModInstantiator<TMod>.ImporterDelegate GetImporter<TMod>(ILoquiRegistration regis)
            where TMod : IModGetter
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
            var paramIndex = paramInfo.Select(x => x.Name).IndexOf("param");
            return (ModPath modPath, GameRelease release, BinaryReadParameters? param) =>
            {
                var args = new object?[paramInfo.Length];
                args[0] = modPath;
                if (releaseIndex != -1)
                {
                    args[releaseIndex] = release;
                }

                if (paramIndex != -1)
                {
                    args[paramIndex] = param;
                }

                return (TMod)deleg.DynamicInvoke(args)!;
            };
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
            var paramIndex = paramInfo.Select(x => x.Name).IndexOf("param");
            return (ModPath modPath, GameRelease release, BinaryReadParameters? param) =>
            {
                var args = new object?[paramInfo.Length];
                args[0] = modPath;
                if (releaseIndex != -1)
                {
                    args[releaseIndex] = release;
                }

                args[paramIndex] = param;
                return (TMod)deleg.DynamicInvoke(args)!;
            };
        }
    }
}
