using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Testing.AutoData;

public class GenderedItemBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type
            && IsTargetType(type))
        {
            return GetCreateMethod()
                .MakeGenericMethod(type.GenericTypeArguments[0])
                .Invoke(this, new object[] {context, request})!;
        }

        return new NoSpecimen();
    }

    public static bool IsTargetType(Type t)
    {
        return t.GenericTypeArguments.Length == 1
               && t.Name.Contains("GenderedItem");
    }

    public static MethodInfo GetCreateMethod()
    {
        return typeof(GenderedItemBuilder)
            .GetMethod("Create", BindingFlags.Static | BindingFlags.Public)!;
    }

    public static GenderedItem<T> Create<T>(
        ISpecimenContext context,
        object request)
    {
        return new GenderedItem<T>(context.Create<T>(), context.Create<T>());
    }
}