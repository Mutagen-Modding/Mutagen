using System.Reflection;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Utility;

public static class ModToGenericCallHelper
{
    public static object? InvokeFromCategory<TSource>(TSource sourceObj, GameCategory category, MethodInfo methodInfo, params object?[] parameters)
    {
        var typeStr = $"Mutagen.Bethesda.{category}.{category}Mod";

        Warmup.Init();
        var regis = LoquiRegistration.GetRegisterByFullName(typeStr);
        if (regis == null)
        {
            throw new Exception($"No loqui registration found for {typeStr}");
        }

        var genMethod = methodInfo.MakeGenericMethod(new Type[] { regis.SetterType, regis.GetterType });

        return genMethod.Invoke(sourceObj, parameters);
    }
    
    public static async Task InvokeFromCategoryAsync<TSource>(TSource sourceObj, GameCategory category, MethodInfo methodInfo, params object?[] parameters)
    {
        var obj = InvokeFromCategory(sourceObj, category, methodInfo, parameters);
        if (obj is Task t)
        {
            await t;
        }
    }
    
    public static async Task<TRet> InvokeFromCategoryAsync<TSource, TRet>(TSource sourceObj, GameCategory category, MethodInfo methodInfo, params object?[] parameters)
    {
        var obj = InvokeFromCategory(sourceObj, category, methodInfo, parameters);
        if (obj is Task<TRet> t)
        {
            await t;
        }
        throw new ArgumentException($"Return value was not of type Task<{typeof(TRet)}>");
    }
}