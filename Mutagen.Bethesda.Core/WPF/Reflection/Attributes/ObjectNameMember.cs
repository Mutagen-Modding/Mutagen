namespace Mutagen.Bethesda.WPF.Reflection.Attributes;

/// <summary>
/// Specifies a member to be displayed when the object is part of any summary areas, 
/// such as when scoping a child setting and this object is being displayed in the drill down summary
/// </summary>
[AttributeUsage(
    AttributeTargets.Class,
    AllowMultiple = true)]
public class ObjectNameMember : Attribute
{
    public string Name { get; }

    public ObjectNameMember(string name)
    {
        Name = name;
    }
}