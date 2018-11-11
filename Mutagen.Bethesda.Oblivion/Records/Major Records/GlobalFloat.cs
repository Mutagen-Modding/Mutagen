using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GlobalFloat
    {
        public const char TRIGGER_CHAR = 'f';

        public override float RawFloat
        {
            get => this.Data;
            set
            {
                if (this.Data != value)
                {
                    this.Data = value;
                    this.RaisePropertyChanged();
                }
                else
                {
                    this.Data_IsSet = true;
                }
            }
        }

        internal static GlobalFloat Factory()
        {
            return new GlobalFloat();
        }

        partial void CustomCtor()
        {
            this.TypeChar = TRIGGER_CHAR;
        }
    }
}
