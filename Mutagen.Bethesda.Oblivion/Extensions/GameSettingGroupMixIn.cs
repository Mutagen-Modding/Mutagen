using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public static class GameSettingGroupMixIn
    {
        public static GameSettingFloat AddNewFloat(this IGroupCommon<GameSetting> gameSettings)
        {
            var ret = new GameSettingFloat(
                gameSettings.SourceMod.GetNextFormKey());
            gameSettings.Add(ret);
            return ret;
        }

        public static GameSettingFloat AddNewFloat(this IGroupCommon<GameSetting> gameSettings, string editorId)
        {
            var ret = new GameSettingFloat(
                gameSettings.SourceMod.GetNextFormKey(editorId));
            gameSettings.Add(ret);
            return ret;
        }

        public static GameSettingInt AddNewInt(this IGroupCommon<GameSetting> gameSettings)
        {
            var ret = new GameSettingInt(
                gameSettings.SourceMod.GetNextFormKey());
            gameSettings.Add(ret);
            return ret;
        }

        public static GameSettingInt AddNewInt(this IGroupCommon<GameSetting> gameSettings, string editorId)
        {
            var ret = new GameSettingInt(
                gameSettings.SourceMod.GetNextFormKey(editorId));
            gameSettings.Add(ret);
            return ret;
        }

        public static GameSettingString AddNewShort(this IGroupCommon<GameSetting> gameSettings)
        {
            var ret = new GameSettingString(
                gameSettings.SourceMod.GetNextFormKey());
            gameSettings.Add(ret);
            return ret;
        }

        public static GameSettingString AddNewShort(this IGroupCommon<GameSetting> gameSettings, string editorId)
        {
            var ret = new GameSettingString(
                gameSettings.SourceMod.GetNextFormKey(editorId));
            gameSettings.Add(ret);
            return ret;
        }
    }
}
