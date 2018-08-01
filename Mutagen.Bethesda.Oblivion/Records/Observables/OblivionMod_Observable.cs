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
    public class OblivionMod_Observable_Manual
    {
        public IObservable<TES4> TES4 { get; private set; }
        public IObservable<GroupObservable<GameSetting>> GameSettings { get; private set; }

        public static OblivionMod_Observable_Manual FromPath(IObservable<string> streamSource)
        {
            OblivionMod_Observable_Manual ret = new OblivionMod_Observable_Manual();
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
            var grupLocObservable = streamSource
                .Select((s) => GetStream(s))
                .SelectMany((frame) =>
                {
                    return RecordLocator.IterateBaseGroupLocations(frame.Reader);
                })
                .Publish()
                .RefCount();
            ret.GameSettings = grupLocObservable
                .Where((kv) => kv.Key == GameSetting_Registration.GMST_HEADER)
                .Select((kv) => kv.Value)
                .WithLatestFromFixed(streamSource, (loc, path) => (loc, path))
                .Select((v) =>
                {
                    return GroupObservable<GameSetting>.FromStream(v.loc, () => GetStream(v.path));
                })
                .Publish()
                .RefCount();
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

        private static MutagenFrame GetStream(string stream)
        {
            return new MutagenFrame(
                new BinaryReadStream(stream));
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

        private async Task WriteGroup<T>(
            MutagenWriter writer,
            IObservable<GroupObservable<T>> obs)
            where T : MajorRecord, IFormID, ILoquiObject<T>
        {
            var grup = (await obs.LastOrDefaultAsync());
            if (grup == null) return;
            await grup.Write(writer);
        }
    }
}
