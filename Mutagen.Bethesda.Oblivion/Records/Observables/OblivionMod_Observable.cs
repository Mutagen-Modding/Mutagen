using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public class OblivionMod_Observable_Manual : ObservableModBase
    {
        public IObservable<TES4> TES4 { get; private set; }
        public IObservable<GroupObservable<GameSetting>> GameSettings { get; private set; }

        public OblivionMod_Observable_Manual(IObservable<string> streamSource) 
            : base(streamSource)
        {
        }

        public static OblivionMod_Observable_Manual FromPath(IObservable<string> streamSource)
        {
            OblivionMod_Observable_Manual ret = new OblivionMod_Observable_Manual(streamSource);
            ret.TES4 = streamSource
                .Select((s) => GetStream(s))
                .Select((frame) =>
                {
                    var nextRecordType = HeaderTranslation.GetNextType(
                        reader: frame.Reader,
                        contentLength: out var contentLength);
                    switch (nextRecordType.TypeInt)
                    {
                        case 0x34534554: // TES4
                            return Mutagen.Bethesda.Oblivion.TES4.Create_Binary(
                                frame: frame,
                                errorMask: null,
                                recordTypeConverter: null);
                        default:
                            throw new ArgumentException("Mod did not start with TES4");
                    }
                });
            ret.GameSettings = ret.GetGroupObservable<GameSetting>(GameSetting_Registration.GMST_HEADER);
            return ret;
        }

        public OblivionMod_Observable_Manual Where(Func<KeyValuePair<FormID, IObservable<MajorRecord>>, bool> selector)
        {
            return new OblivionMod_Observable_Manual()
            {
                TES4 = this.TES4,
                GameSettings = this.GameSettings.Select((g) => g.Where(selector))
            };
        }
        
        public OblivionMod_Observable_Manual Do(Action<MajorRecord> doAction)
        {
            return new OblivionMod_Observable_Manual()
            {
                TES4 = this.TES4,
                GameSettings = this.GameSettings.Select((g) => g.Do(doAction))
            };
        }

        public async Task Write_Binary(MutagenWriter writer)
        {
            (await this.TES4.LastAsync()).Write_Binary(writer);
            await WriteGroup(writer, this.GameSettings);
        }

        public async Task Write_Binary(string path)
        {
            using (var writer = new MutagenWriter(path))
            {
                await Write_Binary(writer);
            }
        }
    }
}
