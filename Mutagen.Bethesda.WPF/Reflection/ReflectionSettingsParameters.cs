using DynamicData;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.WPF.Reflection.Fields;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reflection;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.WPF.Reflection;

public record ReflectionSettingsParameters(
    Assembly Assembly,
    IObservable<IChangeSet<IModListingGetter>> DetectedLoadOrder,
    IObservable<ILinkCache?> LinkCache,
    Type TargetType,
    object? DefaultVal,
    ReflectionSettingsVM MainVM,
    SettingsNodeVM? Parent)
{
    public static ReflectionSettingsParameters FromType(
        IObservable<IChangeSet<IModListingGetter>> detectedLoadOrder,
        IObservable<ILinkCache?> linkCache,
        Type type,
        object? defaultVal = null)
    {
        if (defaultVal != null && defaultVal.GetType() != type)
        {
            throw new ArgumentException($"Passed a default object that didn't match the given type. {type} != {defaultVal.GetType()}");
        }
        return new ReflectionSettingsParameters(
            type.Assembly,
            detectedLoadOrder,
            linkCache,
            type,
            Activator.CreateInstance(type),
            MainVM: null!,
            Parent: null);
    }

    public static ReflectionSettingsParameters FromType<TType>(
        IObservable<IChangeSet<IModListingGetter>> detectedLoadOrder,
        IObservable<ILinkCache?> linkCache,
        TType? defaultVal = null)
        where TType : class
    {
        return FromType(detectedLoadOrder, linkCache, typeof(TType), defaultVal);
    }

    public static ReflectionSettingsParameters CreateFrom<TType>(
        TType defaultVal,
        IObservable<IChangeSet<IModListingGetter>> detectedLoadOrder,
        IObservable<ILinkCache?> linkCache)
        where TType : class
    {
        return FromType(detectedLoadOrder, linkCache, defaultVal);
    }

    public static ReflectionSettingsParameters FromType<TType>(
        IEnumerable<IModListingGetter> detectedLoadOrder,
        ILinkCache? linkCache,
        TType? defaultVal = null)
        where TType : class
    {
        return FromType(
            detectedLoadOrder.AsObservableChangeSet(),
            Observable.Return(linkCache),
            defaultVal);
    }

    public static ReflectionSettingsParameters CreateFrom<TType>(
        TType defaultVal,
        IEnumerable<IModListingGetter> detectedLoadOrder,
        ILinkCache? linkCache)
        where TType : class
    {
        return FromType(detectedLoadOrder, linkCache, defaultVal);
    }
}