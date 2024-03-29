﻿using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;
using Noggog.Testing.AutoFixture.Testing;
using NSubstitute;

namespace Mutagen.Bethesda.Testing.AutoData;

public class MajorRecordBuilder : ISpecimenBuilder
{
    private readonly GameRelease _release;
    private readonly ModConcreteBuilder _modBuilder;
    private readonly bool _configureMembers;
    private bool _isBuilding;

    public MajorRecordBuilder(
        GameRelease release, 
        ModConcreteBuilder modBuilder,
        bool configureMembers)
    {
        _release = release;
        _modBuilder = modBuilder;
        _configureMembers = configureMembers;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (_isBuilding) return new NoSpecimen();
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }
        
        if (request is not Type t) return new NoSpecimen();
        if (t.InheritsFrom(typeof(IMajorRecord)))
        {
            var ret = GetMajorRecord(t, context);
            if (ret != null) return ret;
        }
            
        return new NoSpecimen();
    }

    private IMajorRecord? GetMajorRecord(Type t, ISpecimenContext context)
    {
        FormKey formKey = _modBuilder.LastCreatedConcreteMod?.GetNextFormKey() ?? context.Create<FormKey>();
        IMajorRecordInternal? ret;
        if (t.IsInterface
            && MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(_release.ToCategory(), t, out var regis)
            && regis.Registrations.Count > 0)
        {
            ret = MajorRecordInstantiator.Activator(formKey, _release, regis.Registrations.First().ClassType) as IMajorRecordInternal;
        }
        else if (!t.IsAbstract)
        {
            ret = MajorRecordInstantiator.Activator(formKey, _release, t) as IMajorRecordInternal;
        }
        else
        {
            return null;
        }

        if (ret == null) return null;

        if (_configureMembers)
        {
            FillAllProperties(context, ret, keepArraySizes: true);
        }

        ModifyMajorRecordFields(ret, formKey);
        
        var group = _modBuilder.LastCreatedConcreteMod?.TryGetTopLevelGroup(t);
        group?.AddUntyped(ret);
        return ret;
    }

    private static void ModifyMajorRecordFields(IMajorRecordInternal rec, FormKey formKey)
    {
        rec.IsCompressed = false;
        rec.IsDeleted = false;
        rec.FormKey = formKey;
    }
    
    public static void FillAllProperties(ISpecimenContext context, object item, bool keepArraySizes = false)
    {
        foreach (var prop in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var setter = prop.GetSetMethod();
            if (setter == null) continue;

            var toSet = GetPropertyFor(context, item, prop, keepArraySizes);
            if (toSet != null)
            {
                setter.Invoke(item, new object[] { toSet });
            }
        }
    }

    private static object? GetPropertyFor(
        ISpecimenContext context,
        object item, 
        PropertyInfo prop,
        bool keepArraySizes)
    {
        if (keepArraySizes && prop.PropertyType.IsArray)
        {
            var getter = prop.GetGetMethod();
            if (getter == null) return null;
            var arr = getter.Invoke(item, Array.Empty<object>()) as Array;
            if (arr == null) return null;
            var genArg = prop.PropertyType.GenericTypeArguments[0];
            for (int i = 0; i < arr.Length; i++)
            {
                var indexedItem = GetTypeFor(context, genArg, keepArraySizes);
                if (indexedItem == null) break;
                arr.SetValue(indexedItem, i);
            }

            return null;
        }

        if (keepArraySizes
            && prop.PropertyType.IsGenericType
            && prop.PropertyType.GetGenericTypeDefinition() == typeof(MemorySlice<>))
        {
            var getter = prop.GetGetMethod();
            if (getter == null) return null;
            var arr = getter.Invoke(item, Array.Empty<object>());
            if (arr == null) return null;
            var len = (int)arr.GetType().GetMember("Length")
                .OfType<PropertyInfo>()
                .First().GetValue(arr);
            if (len == 0) return null;

            var genArg = prop.PropertyType.GenericTypeArguments[0];
            var indexedSetter = prop.PropertyType.GetProperties()
                .First(x => x.GetIndexParameters().Length > 0);
            
            for (int i = 0; i < len; i++)
            {
                var indexedItem = GetTypeFor(context, genArg, keepArraySizes);
                if (indexedItem == null) break;
                indexedSetter.SetValue(arr, indexedItem, new object[] { i });
            }
            
            return null;
        }

        return GetTypeFor(context, prop.PropertyType, keepArraySizes);
    }

    private static object? GetTypeFor(
        ISpecimenContext context,
        Type type,
        bool keepArraySizes)
    {
        if (LoquiRegistration.IsLoquiType(type))
        {
            try
            {
                var subItem = Activator.CreateInstance(type);
                if (subItem == null) return null;
                FillAllProperties(context, subItem, keepArraySizes: keepArraySizes);
                if (subItem is IMajorRecordInternal majRec)
                {
                    ModifyMajorRecordFields(majRec, majRec.FormKey);
                }
                return subItem;
            }
            catch (Exception)
            {
                return null;
            }
        }

        return context.Resolve(type);
    }
}