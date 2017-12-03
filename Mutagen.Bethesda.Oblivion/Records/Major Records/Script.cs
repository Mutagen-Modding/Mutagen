using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Script
    {
        public enum ScriptType
        {
            Object = 0,
            Quest = 1,
            MagicEffect = 0x100
        }

        partial void CustomCtor()
        {
            this.CompiledScript_Property.Subscribe(
                (change) =>
                {
                    this.MetadataSummary.CompiledSizeInternal = change.New.Length;
                },
                fireInitial: false);
            this.LocalVariables.Subscribe(
                (change) =>
                {
                    this.MetadataSummary.VariableCountInternal = (uint)this.LocalVariables.Count;
                },
                fireInitial: false);
        }
    }
}
