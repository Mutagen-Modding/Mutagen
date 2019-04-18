using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GlobalShort
    {
        public const char TRIGGER_CHAR = 's';
        
        public override float RawFloat
        {
            get => (float)this.Data;
            set
            {
                var val = (short)value;
                if (this.Data != val)
                {
                    this.Data = val;
                    this.RaisePropertyChanged();
                }
                else
                {
                    this.Data_IsSet = true;
                }
            }
        }

        internal static GlobalShort Factory()
        {
            return new GlobalShort();
        }

        partial void CustomCtor()
        {
            this.TypeChar = TRIGGER_CHAR;
            this.WhenAny(x => x.RawFloat)
                .Skip(1)
                .DistinctUntilChanged()
                .Select(x => (short)Math.Round(x))
                .BindTo(this, x => x.Data);
            this.WhenAny(x => x.Data)
                .Skip(1)
                .DistinctUntilChanged()
                .Select(x => (float)x)
                .BindTo(this, x => x.RawFloat);
        }

        static partial void WriteBinary_Data_Custom(MutagenWriter writer, GlobalShort item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, GlobalShort_Registration.FLTV_HEADER))
            {
                writer.Write((float)item.Data);
            }
        }
    }
}
