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
    /// <typeparam name="TMod">
    /// Type of Mod to instantiate.  Can be the direct class, or one of its interfaces.
    /// </typeparam>
    public static class ModFactory<TMod>
        where TMod : IModGetter
    {
        public delegate TMod ActivatorDelegate(ModKey modKey, GameRelease release, float? headerVersion = null, bool? forceUseLowerFormIDRanges = false);
        public delegate TMod ImporterDelegate(ModPath modKey, GameRelease release, BinaryReadParameters? param = null);
        public delegate TMod ImportMultiFileGetterDelegate(ModKey targetModKey, IEnumerable<ModPath> splitFiles, IEnumerable<IModMasterStyledGetter> loadOrder, GameRelease release, BinaryReadParameters? param = null);

        /// <summary>
        /// Function to call to retrieve a new Mod of type T
        /// </summary>
        public static readonly ActivatorDelegate Activator;

        /// <summary>
        /// Function to call to import a new Mod of type T
        /// </summary>
        public static readonly ImporterDelegate Importer;

        /// <summary>
        /// Function to call to import multiple split mod files as a unified multi-file overlay
        /// </summary>
        public static readonly ImportMultiFileGetterDelegate ImportMultiFileGetter;

        static ModFactory()
        {
            Warmup.Init();
            bool createActivator = true;
            var type = typeof(TMod);
            if (type.Name.EndsWith("DisposableGetter"))
            {
                var className = type.Name.TrimStringFromEnd("DisposableGetter") + "Getter";
                type = Type.GetType($"{type.Namespace}.{className}, {type.Namespace}");
                createActivator = false;
            }

            if (type == null)
            {
                throw new ArgumentException();
            }

            if (type == typeof(IModGetter) || type == typeof(IMod))
            {
                Activator = (modKey, release, headerVersion, forceUseLowerFormIDRanges) => (TMod)ModFactory.Activator(modKey, release, headerVersion, forceUseLowerFormIDRanges);
                if (type == typeof(IModGetter))
                {
                    Importer = (path, release, param) => (TMod)ModFactory.ImportGetter(path, release, param);
                    ImportMultiFileGetter = (targetModKey, splitFiles, loadOrder, release, param) =>
                        (TMod)ModFactory.ImportMultiFileGetter(targetModKey, splitFiles, loadOrder, release, param);
                }
                else
                {
                    Importer = (path, release, param) => (TMod)ModFactory.ImportSetter(path, release, param);
                    ImportMultiFileGetter = (targetModKey, splitFiles, loadOrder, release, param) =>
                        throw new InvalidOperationException("ImportMultiFileGetter is only supported for getter types (IModGetter), not setter types (IMod)");
                }
            }
            else
            {
                if (!LoquiRegistration.TryGetRegister(type, out var regis))
                {
                    throw new ArgumentException();
                }

                if (createActivator)
                {
                    Activator = ModFactoryReflection.GetActivator<TMod>(regis);
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
                    Importer = ModFactoryReflection.GetImporter<TMod>(regis);
                    ImportMultiFileGetter = (targetModKey, splitFiles, loadOrder, release, param) =>
                        throw new InvalidOperationException("ImportMultiFileGetter is only supported for getter/overlay types, not mutable mod types");
                }
                else
                {
                    Importer = ModFactoryReflection.GetOverlay<TMod>(regis);
                    ImportMultiFileGetter = (targetModKey, splitFiles, loadOrder, release, param) =>
                        (TMod)ModFactory.ImportMultiFileGetter(targetModKey, splitFiles, loadOrder, release, param);
                }
            }
        }
    }

    /// <summary>
    /// A static class encapsulating the job of creating a new Mod in a generic context
    /// </summary>
    public static class ModFactory
    {
        record Delegates(
            ModFactory<IModDisposeGetter>.ImporterDelegate ImportGetter,
            ModFactory<IMod>.ImporterDelegate ImportSetter,
            ModFactory<IMod>.ActivatorDelegate Activator);

        private static Dictionary<GameCategory, Delegates> _dict = new();

        static ModFactory()
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
                    ModFactoryReflection.GetOverlay<IModDisposeGetter>(modRegistration),
                    ModFactoryReflection.GetImporter<IMod>(modRegistration),
                    ModFactoryReflection.GetActivator<IMod>(modRegistration));

            }
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

        /// <summary>
        /// Imports multiple split mod files and returns a multi-file overlay that presents them as a single unified mod.
        /// </summary>
        /// <param name="targetModKey">The ModKey for the unified overlay (typically the base name without _1, _2 suffixes)</param>
        /// <param name="splitFiles">Paths to the split mod files to merge</param>
        /// <param name="loadOrder">Load order to use for master ordering</param>
        /// <param name="release">Game release for the mods</param>
        /// <param name="param">Binary read parameters</param>
        /// <returns>Multi-file overlay presenting all split files as a single mod</returns>
        public static IModDisposeGetter ImportMultiFileGetter(
            ModKey targetModKey,
            IEnumerable<ModPath> splitFiles,
            IEnumerable<IModMasterStyledGetter> loadOrder,
            GameRelease release,
            BinaryReadParameters? param = null)
        {
            // Import all split files as overlays
            var overlays = new List<IModDisposeGetter>();
            foreach (var splitFile in splitFiles)
            {
                var overlay = ImportGetter(splitFile, release, param);
                overlays.Add(overlay);
            }

            // Validate no duplicate FormIDs across split files
            ValidateNoDuplicates(overlays, targetModKey);

            // Merge masters from all overlays according to load order
            var mergedMasters = MergeMasters(overlays, loadOrder);

            // Create multi-file overlay that presents all the split files as one unified mod
            return CreateMultiFileOverlay(targetModKey, release, overlays, mergedMasters);
        }

        private static IReadOnlyList<IModMasterStyledGetter> MergeMasters(
            List<IModDisposeGetter> overlays,
            IEnumerable<IModMasterStyledGetter> loadOrder)
        {
            // Collect all unique masters from all overlays
            var allMasters = new HashSet<ModKey>();
            foreach (var overlay in overlays)
            {
                foreach (var master in overlay.MasterReferences)
                {
                    allMasters.Add(master.Master);
                }
            }

            // Create a dictionary for quick load order lookup
            var loadOrderList = loadOrder.ToList();
            var loadOrderDict = loadOrderList
                .Select((m, i) => new { ModKey = m.ModKey, Index = i })
                .ToDictionary(x => x.ModKey, x => x.Index);

            // Order masters according to the provided load order
            var orderedMasterKeys = allMasters
                .OrderBy(m => loadOrderDict.TryGetValue(m, out var index) ? index : int.MaxValue)
                .ThenBy(m => m.FileName.String) // Fallback to alphabetical for masters not in load order
                .ToList();

            // Convert to IModMasterStyledGetter list
            var result = new List<IModMasterStyledGetter>();
            foreach (var masterKey in orderedMasterKeys)
            {
                // Try to find in original load order first
                var loadOrderEntry = loadOrderList.FirstOrDefault(lo => lo.ModKey == masterKey);
                if (loadOrderEntry != null)
                {
                    result.Add(loadOrderEntry);
                }
                else
                {
                    // Create a keyed master style if not in load order (default to full master)
                    result.Add(new KeyedMasterStyle(masterKey, MasterStyle.Full));
                }
            }

            return result.AsReadOnly();
        }

        private static IModDisposeGetter CreateMultiFileOverlay(
            ModKey modKey,
            GameRelease gameRelease,
            List<IModDisposeGetter> overlays,
            IReadOnlyList<IModMasterStyledGetter> mergedMasters)
        {
            // Determine which multi-file overlay class to instantiate based on game release
            var (typeName, assemblyName) = gameRelease.ToCategory() switch
            {
                GameCategory.Skyrim => ("Mutagen.Bethesda.Skyrim.SkyrimMultiModOverlay", "Mutagen.Bethesda.Skyrim"),
                _ => throw new NotImplementedException(
                    $"Multi-mod overlay is not yet implemented for {gameRelease}. " +
                    "Only Skyrim is currently supported.")
            };

            // Load the overlay type with assembly-qualified name
            var assemblyQualifiedName = $"{typeName}, {assemblyName}";
            var overlayType = Type.GetType(assemblyQualifiedName);
            if (overlayType == null)
            {
                throw new InvalidOperationException($"Could not find multi-file overlay type: {assemblyQualifiedName}");
            }

            // Find the constructor - look for any constructor matching the pattern:
            // (ModKey, GameRelease, IReadOnlyList<IXXXModGetter>, IReadOnlyList<IModMasterStyledGetter>)
            var constructor = overlayType.GetConstructors()
                .FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    if (parameters.Length != 4) return false;
                    if (parameters[0].ParameterType != typeof(ModKey)) return false;
                    if (parameters[1].ParameterType != typeof(GameRelease)) return false;
                    if (!parameters[2].ParameterType.IsGenericType) return false;
                    if (parameters[2].ParameterType.GetGenericTypeDefinition() != typeof(IReadOnlyList<>)) return false;
                    // Check that the list element type is assignable from IModGetter
                    var listElementType = parameters[2].ParameterType.GetGenericArguments()[0];
                    if (!typeof(IModGetter).IsAssignableFrom(listElementType)) return false;
                    if (parameters[3].ParameterType != typeof(IReadOnlyList<IModMasterStyledGetter>)) return false;
                    return true;
                });

            if (constructor == null)
            {
                throw new InvalidOperationException($"Could not find appropriate constructor for {assemblyQualifiedName}");
            }

            // Get the expected list element type from the constructor parameter
            var listParameterType = constructor.GetParameters()[2].ParameterType;
            var listElementType = listParameterType.GetGenericArguments()[0];

            // Create a properly-typed list by casting each overlay to the expected type
            // Use reflection to create a List<TCorrectType>
            var typedListType = typeof(List<>).MakeGenericType(listElementType);
            var typedList = (System.Collections.IList)System.Activator.CreateInstance(typedListType)!;
            foreach (var item in overlays)
            {
                typedList.Add(item);
            }

            // Convert to IReadOnlyList
            var asReadOnlyMethod = typeof(List<>).MakeGenericType(listElementType).GetMethod("AsReadOnly")!;
            var readOnlyList = asReadOnlyMethod.Invoke(typedList, null);

            // Instantiate the overlay
            var overlay = constructor.Invoke(new object[]
            {
                modKey,
                gameRelease,
                readOnlyList!,
                mergedMasters
            });

            if (overlay == null)
            {
                throw new InvalidOperationException($"Failed to create multi-file overlay of type {assemblyQualifiedName}");
            }

            return (IModDisposeGetter)overlay;
        }

        private static void ValidateNoDuplicates(List<IModDisposeGetter> overlays, ModKey modKey)
        {
            var seenFormKeys = new Dictionary<FormKey, string>();

            for (int i = 0; i < overlays.Count; i++)
            {
                var overlay = overlays[i];
                var fileName = $"{modKey.FileName.String.Replace(modKey.Type.ToString(), "")}_{i + 1}.{modKey.Type}";

                foreach (var record in overlay.EnumerateMajorRecords())
                {
                    if (seenFormKeys.TryGetValue(record.FormKey, out var previousFile))
                    {
                        throw new InvalidOperationException(
                            $"Duplicate FormKey {record.FormKey} found in both {previousFile} and {fileName}. " +
                            "This indicates corruption in the split files.");
                    }

                    seenFormKeys[record.FormKey] = fileName;
                }
            }
        }
    }

    internal static class ModFactoryReflection
    {
        internal static ModFactory<TMod>.ActivatorDelegate GetActivator<TMod>(ILoquiRegistration regis)
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

        public static ModFactory<TMod>.ImporterDelegate GetImporter<TMod>(ILoquiRegistration regis)
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

        public static ModFactory<TMod>.ImporterDelegate GetOverlay<TMod>(ILoquiRegistration regis)
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
