using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public static class NoggogRxExt
    {
        // https://github.com/dotnet/reactive/issues/444
        public static IObservable<TResult> WithLatestFromFixed<TLeft, TRight, TResult>(
            this IObservable<TLeft> obs,
            IObservable<TRight> other,
            Func<TLeft, TRight, TResult> resultSelector)
        {
            return obs.Publish(
                os => other
                    .Select(a => os
                        .Select(b => resultSelector(b, a)))
                    .Switch());
        }
    }
}
