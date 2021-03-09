using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Skyrim
{
    public static class GlobalGroupMixIn
    {
        public static GlobalFloat AddNewFloat(this IGroupCommon<IGlobal> globals)
        {
            var ret = new GlobalFloat(
                globals.SourceMod.GetNextFormKey(),
                globals.SourceMod.GameRelease.ToSkyrimRelease());
            globals.Add(ret);
            return ret;
        }

        public static GlobalFloat AddNewFloat(this IGroupCommon<IGlobal> globals, string editorId)
        {
            var ret = new GlobalFloat(
                globals.SourceMod.GetNextFormKey(editorId),
                globals.SourceMod.GameRelease.ToSkyrimRelease());
            globals.Add(ret);
            return ret;
        }

        public static GlobalInt AddNewInt(this IGroupCommon<IGlobal> globals)
        {
            var ret = new GlobalInt(
                globals.SourceMod.GetNextFormKey(),
                globals.SourceMod.GameRelease.ToSkyrimRelease());
            globals.Add(ret);
            return ret;
        }

        public static GlobalInt AddNewInt(this IGroupCommon<IGlobal> globals, string editorId)
        {
            var ret = new GlobalInt(
                globals.SourceMod.GetNextFormKey(editorId),
                globals.SourceMod.GameRelease.ToSkyrimRelease());
            globals.Add(ret);
            return ret;
        }

        public static GlobalShort AddNewShort(this IGroupCommon<IGlobal> globals)
        {
            var ret = new GlobalShort(
                globals.SourceMod.GetNextFormKey(),
                globals.SourceMod.GameRelease.ToSkyrimRelease());
            globals.Add(ret);
            return ret;
        }

        public static GlobalShort AddNewShort(this IGroupCommon<IGlobal> globals, string editorId)
        {
            var ret = new GlobalShort(
                globals.SourceMod.GetNextFormKey(editorId),
                globals.SourceMod.GameRelease.ToSkyrimRelease());
            globals.Add(ret);
            return ret;
        }
    }
}
