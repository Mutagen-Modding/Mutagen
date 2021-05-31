using System;

namespace Mutagen.Bethesda.WPF.Reflection.Attributes
{
    [AttributeUsage(
      AttributeTargets.Field | AttributeTargets.Property,
      AllowMultiple = false)]
    public class Ignore : Attribute
    {
    }
}
