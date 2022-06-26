using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins.Aspects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim.Assets;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ScriptEntry
    {
        public enum Flag : byte
        {
            Local = 0,
            Inherited = 1,
            Removed = 2,
            InheritedAndRemoved = 3,
        }
    }

    namespace Internals
    {
        public partial class ScriptEntryCommon
        {
            public static IEnumerable<IAssetLink> GetAdditionalAssetLinks(IScriptEntryGetter obj, ILinkCache linkCache)
            {
                if (string.IsNullOrWhiteSpace(obj.Name)) yield break;

                yield return new AssetLink<SkyrimScriptCompiledAssetType>(SkyrimScriptCompiledAssetType.Instance, obj.Name);
                yield return new AssetLink<SkyrimScriptSourceAssetType>(SkyrimScriptSourceAssetType.Instance, obj.Name);
            }
        }
        
        public partial class ScriptEntryBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name ?? string.Empty;

            public IReadOnlyList<IScriptPropertyGetter> Properties => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public ScriptEntry.Flag Flags => throw new NotImplementedException();
        }
    }
}
