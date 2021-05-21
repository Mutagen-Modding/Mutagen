using System;
using System.Runtime.CompilerServices;

namespace Mutagen.Bethesda.WPF.Reflection.Attributes
{
    /// <summary>
    /// https://stackoverflow.com/questions/9062235/get-properties-in-order-of-declaration-using-reflection
    /// Hopefully will not be needed to specify eventually with something like a Fody plugin?
    /// </summary>
    [AttributeUsage(
      AttributeTargets.Field | AttributeTargets.Property,
      AllowMultiple = false)]
    public class MaintainOrder : Attribute
    {
        public int Order { get; }

        public MaintainOrder([CallerLineNumber] int order = 0)
        {
            Order = order;
        }
    }
}
