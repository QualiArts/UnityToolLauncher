using System;
using UnityEditor;
using UnityEngine;

namespace ToolLauncher.ToolLauncherSettings
{
    /// <summary>
    /// ユーザー毎のランチャーの設定を保存するクラス
    /// </summary>
    public static class UserSettingManager
    {
        private const string Key = "ToolLauncher.ToolLauncherUserSettings";

        public static void Save(UserSettings settings)
        {
            EditorUserSettings.SetConfigValue(Key, JsonUtility.ToJson(settings));
        }

        public static UserSettings Load()
        {
            var json = EditorUserSettings.GetConfigValue(Key);
            try
            {
                return JsonUtility.FromJson<UserSettings>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
