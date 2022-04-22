using System;
using System.Linq;

namespace Mutagen.Bethesda.Plugins.RecordTypeMapping;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface,
    Inherited = true)]
public class AssociatedRecordTypesAttribute : Attribute
{
    public RecordType[] Types { get; }

    public AssociatedRecordTypesAttribute(params int[] types)
    {
        Types = types.Select(x => new RecordType(x)).ToArray();
    }
}