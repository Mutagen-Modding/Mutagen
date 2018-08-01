using Loqui;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class ObservableModBase
    {
        protected readonly IObservable<string> streamSource;
        protected readonly IObservable<KeyValuePair<RecordType, long>> groupLocationObservable;

        public ObservableModBase(IObservable<string> streamSource)
        {
            this.streamSource = streamSource;
            this.groupLocationObservable = streamSource
                .Select((s) => GetStream(s))
                .SelectMany((frame) =>
                {
                    return RecordLocator.IterateBaseGroupLocations(frame.Reader);
                })
                .Publish()
                .RefCount();
        }

        protected IObservable<GroupObservable<T>> GetGroupObservable<T>(RecordType type)
            where T : MajorRecord, IFormID, ILoquiObject<T>
        {
            return groupLocationObservable
                .Where((kv) => kv.Key == type)
                .Select((kv) => kv.Value)
                .WithLatestFromFixed(streamSource, (loc, path) => (loc, path))
                .Select((v) =>
                {
                    return GroupObservable<T>.FromStream(v.loc, () => GetStream(v.path));
                })
                .Publish()
                .RefCount();
        }

        protected static MutagenFrame GetStream(string stream)
        {
            return new MutagenFrame(
                new BinaryReadStream(stream));
        }

        protected static async Task WriteGroup<T>(
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
