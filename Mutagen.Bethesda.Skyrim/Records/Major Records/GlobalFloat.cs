using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class GlobalFloat
    {
        public const char TRIGGER_CHAR = 'f';
        public override char TypeChar => TRIGGER_CHAR;

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
    }
}
