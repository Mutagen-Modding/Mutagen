using System;

namespace Mutagen.Bethesda.WPF.Reflection.Attributes;

[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false)]
public class SettingName : Attribute
{
    public string Name { get; }

    public SettingName(string name)
    {
        Name = name;
    }
}