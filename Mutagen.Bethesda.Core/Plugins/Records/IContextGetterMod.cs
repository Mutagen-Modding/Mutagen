using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Records;

public interface IContextGetterMod<TMod, TModGetter> : IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    where TModGetter : IModGetter
    where TMod : TModGetter, IMod
{
}

public interface IContextMod<TMod, TModGetter> : IMod, IContextGetterMod<TMod, TModGetter>
    where TModGetter : IModGetter
    where TMod : TModGetter, IContextMod<TMod, TModGetter>
{
}