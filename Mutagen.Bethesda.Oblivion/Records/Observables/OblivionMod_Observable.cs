using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public class OblivionMod_Observable : IDisposable
    {
        CompositeDisposable compositeDisposable = new CompositeDisposable();

        public IObservable<TES4> TES4 { get; }
        public IObservable<GroupObservable<GameSetting>> GameSettings { get; }

        public OblivionMod_Observable(IObservable<string> streamSource)
        {
            this.TES4 = streamSource
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
            this.GameSettings = grupLocObservable
                .Where((kv) => kv.Key == GameSetting_Registration.GMST_HEADER)
                .Select((kv) => kv.Value)
                .WithLatestFromFixed(streamSource, (loc, path) => (loc, path))
                .Select((v) =>
                {
                    return new GroupObservable<GameSetting>(v.loc, () => GetStream(v.path));
                });
        }

        private MutagenFrame GetStream(string stream)
        {
            return new MutagenFrame(
                new BinaryReadStream(stream));
        }

        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }
}
