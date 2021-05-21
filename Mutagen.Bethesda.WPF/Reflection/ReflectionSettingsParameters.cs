using DynamicData;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.WPF.Plugins.Order;
using Mutagen.Bethesda.WPF.Reflection.Fields;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reflection;

namespace Mutagen.Bethesda.WPF.Reflection
{
    public record ReflectionSettingsParameters(
        Assembly Assembly,
        IObservable<IChangeSet<ModListingVM>> DetectedLoadOrder,
        IObservable<ILinkCache> LinkCache,
        Type TargetType,
        object? DefaultVal,
        ReflectionSettingsVM MainVM,
        SettingsNodeVM? Parent)
    {
        public static ReflectionSettingsParameters FromType(
            IObservable<IChangeSet<ModListingVM>> detectedLoadOrder,
            IObservable<ILinkCache> linkCache,
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
            IObservable<IChangeSet<ModListingVM>> detectedLoadOrder,
            IObservable<ILinkCache> linkCache,
            TType? defaultVal = null)
            where TType : class
        {
            return FromType(detectedLoadOrder, linkCache, typeof(TType), defaultVal);
        }

        public static ReflectionSettingsParameters CreateFrom<TType>(
            TType defaultVal,
            IObservable<IChangeSet<ModListingVM>> detectedLoadOrder,
            IObservable<ILinkCache> linkCache)
            where TType : class
        {
            return FromType(detectedLoadOrder, linkCache, defaultVal);
        }

        public static ReflectionSettingsParameters FromType<TType>(
            IEnumerable<ModListingVM> detectedLoadOrder,
            ILinkCache linkCache,
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
            IEnumerable<ModListingVM> detectedLoadOrder,
            ILinkCache linkCache)
            where TType : class
        {
            return FromType(detectedLoadOrder, linkCache, defaultVal);
        }
    }
}
