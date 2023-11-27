using System;
using UnityEditor;
using UnityEngine;

namespace ToolLauncher.ToolLauncherSettings
{
    /// <summary>
    /// ユーザー毎のランチャー設定をシリアライズ化するクラス
    /// </summary>
    [Serializable]
    public class UserSettings
    {
        /// <summary>
        /// 最後に開いていた設定のGUID
        /// </summary>
        [SerializeField]
        private string _lastOpenedLauncherSettingsGUID;

        /// <summary>
        /// 最後に開いていた設定
        /// </summary>
        public ToolLauncherSetting LastOpenedLauncherSettings
        {
            get => AssetDatabase.LoadAssetAtPath<ToolLauncherSetting>(AssetDatabase.GUIDToAssetPath(_lastOpenedLauncherSettingsGUID));
            set => _lastOpenedLauncherSettingsGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
        }
    }
}
