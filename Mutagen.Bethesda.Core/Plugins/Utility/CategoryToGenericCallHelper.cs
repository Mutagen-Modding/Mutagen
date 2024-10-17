using System.Reflection;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Utility;

public static class ModToGenericCallHelper
{
    public static object? InvokeFromCategory(IModGetter mod, MethodInfo methodInfo, params object?[] parameters)
    {
        var category = mod.GameRelease.ToCategory();
        var typeStr = $"Mutagen.Bethesda.{category}.{category}Mod";

        Warmup.Init();
        var regis = LoquiRegistration.GetRegisterByFullName(typeStr);
        if (regis == null)
        {
            throw new Exception($"No loqui registration found for {typeStr}");
        }

        var genMethod = methodInfo.MakeGenericMethod(new Type[] { regis.SetterType, regis.GetterType });

        return genMethod.Invoke(mod, parameters);
    }
    
    public static async Task InvokeFromCategoryAsync(IModGetter mod, MethodInfo methodInfo, params object?[] parameters)
    {
        var obj = InvokeFromCategory(mod, methodInfo, parameters);
        if (obj is Task t)
        {
            await t;
        }
    }
    
    public static async Task<TRet> InvokeFromCategoryAsync<TRet>(IModGetter mod, MethodInfo methodInfo, params object?[] parameters)
    {
        var obj = InvokeFromCategory(mod, methodInfo, parameters);
        if (obj is Task<TRet> t)
        {
            await t;
        }
        throw new ArgumentException($"Return value was not of type Task<{typeof(TRet)}>");
    }
}