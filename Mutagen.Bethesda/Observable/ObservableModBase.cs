using Loqui;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class ObservableModBase
    {
        protected readonly IObservable<Func<MutagenFrame>> streamSource;
        protected readonly IObservable<KeyValuePair<RecordType, long>> groupLocationObservable;

        protected ObservableModBase()
        {
        }

        public ObservableModBase(IObservable<string> streamSource)
        {
            this.streamSource = streamSource
                .Select<string, Func<MutagenFrame>>((s) =>
                {
                    return () => new MutagenFrame(
                        new BinaryReadStream(s));
                });
            this.groupLocationObservable = this.streamSource
                .Select((sg) => sg())
                .SelectMany((frame) =>
                {
                    using (frame.Reader)
                    {
                        return RecordLocator.IterateBaseGroupLocations(frame.Reader).ToArray();
                    }
                })
                .Replay()
                .RefCount();
        }

        public ObservableModBase(IObservable<byte[]> streamSource)
        {
            this.streamSource = streamSource
                .Select<byte[], Func<MutagenFrame>>((s) =>
                {
                    return () => new MutagenFrame(
                        new BinaryMemoryReadStream(s));
                });
            this.groupLocationObservable = this.streamSource
                .Select((sg) => sg())
                .SelectMany((frame) =>
                {
                    using (frame.Reader)
                    {
                        return RecordLocator.IterateBaseGroupLocations(frame.Reader).ToArray();
                    }
                })
                .Replay()
                .RefCount();
        }

        protected IObservable<T> GetObservableRecord<T>(RecordType type)
            where T : ILoquiObjectGetter
        {
            return this.streamSource
                .Select((sg) => sg())
                .Select((frame) =>
                {
                    using (frame.Reader)
                    {
                        var nextRecordType = HeaderTranslation.GetNextType(
                            reader: frame.Reader,
                            contentLength: out var contentLength);
                        if (nextRecordType == type)
                        {
                            LoquiBinaryTranslation<T>.Instance.Parse(
                                frame: frame,
                                errorMask: null,
                                item: out var ret);
                            return ret;
                        }
                        else
                        {
                            throw new ArgumentException("Mod did not start with TES4");
                        }
                    }
                });
        }

        protected IObservable<GroupObservable<T>> GetGroupObservable<T>(RecordType type)
            where T : MajorRecord, IFormID, ILoquiObject<T>
        {
            return groupLocationObservable
                .Where((kv) => kv.Key == type)
                .Select((kv) => kv.Value)
                .WithLatestFromFixed(streamSource, (loc, sg) => (loc, sg))
                .Select((v) =>
                {
                    return GroupObservable<T>.FromStream(v.loc, v.sg);
                });
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
