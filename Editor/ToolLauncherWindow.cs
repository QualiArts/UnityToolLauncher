using System;
using System.Collections.Generic;
using System.Linq;
using ToolLauncher.ListDrawer;
using ToolLauncher.ToolLauncherSettings;
using UnityEditor;
using UnityEngine;

namespace ToolLauncher
{
    public class ToolLauncherWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [SerializeField] private ToolLauncherSetting _setting;
        private IListDrawer _listDrawer;
        private TagManager m_TagManager;
        private ToolLauncherSetting[] _settings = {};

        [MenuItem("Window/ツールランチャー %t")]
        public static void Open()
        {
            GetWindow<ToolLauncherWindow>(false, "ツールランチャー");
        }

        /// <summary>
        /// リスト表示用のインスタンスをセットする
        /// </summary>
        private void SetListDrawer(IListDrawer drawer)
        {
            _listDrawer = drawer;
        }

        private void OnEnable()
        {
            // 今後アイコンだけ表示とか切り替えられるように
            SetListDrawer(new RectangleListDrawer());

            // 最後に開いていた設定を表示する
            var userSettings = UserSettingManager.Load();
            if (userSettings != null)
            {
                _setting = userSettings.LastOpenedLauncherSettings;
            }
        }

        private void OnDisable()
        {
            // ユーザー設定を保存する
            var userSettings = new UserSettings { LastOpenedLauncherSettings = _setting };
            UserSettingManager.Save(userSettings);
        }

        private void OnFocus()
        {
            if (m_TagManager == null)
            {
                m_TagManager = new TagManager();
            }

            _settings = AssetDatabase
                .FindAssets("t:" + nameof(ToolLauncherSetting))
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<ToolLauncherSetting>(x))
                .ToArray();

            if (_setting != null)
            {
                m_TagManager.SetTagActive(_setting.TagName, true);
            }
        }

        private void OnGUI()
        {
            // 設定選択画面
            DrawSettingSelectGUI();

            if (_setting == null) return;

            // 区切り線
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1f));

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            int index = 0;
            
            // タグによる描画
            foreach (var s in _settings)
            {
                // タグで絞り込み
                var tagName = s.TagName;
                if (!m_TagManager.IsTagActive(tagName))
                {
                    continue;
                }

                var tagColor = s.TagColor;
                foreach (var menu in s.GetMenuList())
                {
                    _listDrawer.Draw(new ListDrawerParam(menu, index++, new TagData(tagName, tagColor)));
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private List<ToolLauncherSetting.ToolLauncherMenu> GetMenuList()
        {
            var list = new List<ToolLauncherSetting.ToolLauncherMenu>();             
            foreach (var setting in _settings)
            {
                if (setting == null)
                {
                    continue;
                }

                if (m_TagManager.IsTagActive(setting.TagName))
                {
                    list.AddRange(setting.GetMenuList());
                }
            }

            return list;
        }

        /// <summary>
        /// 設定ファイルの選択画面を描画
        /// </summary>
        private void DrawSettingSelectGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var buttonLabel = _setting != null ? _setting.DisplayName : string.Empty;
                if (_setting != null && !string.IsNullOrEmpty(_setting.TagName) && !m_TagManager.IsOnlyActive(_setting.TagName))
                {
                    buttonLabel += "*";
                }
                
                // 設定ファイルの選択Popup
                // EditorGUIのPopupは表示中に設定ファイルが消えるとハンドリングできないため独自のPopup
                var buttonContent = new GUIContent(buttonLabel);
                var buttonRect = GUILayoutUtility.GetRect(buttonContent, EditorStyles.popup);
                if (GUI.Button(buttonRect, buttonContent, EditorStyles.popup))
                {
                    GenericMenu menu = new GenericMenu();
                    // 設定を持っているScriptableObjectを探す
                    foreach (var s in _settings)
                    {
                        menu.AddItem(new GUIContent(s.DisplayName), 
                            _setting == s,
                            data =>
                            {
                                _setting = (ToolLauncherSetting)data;
                                m_TagManager.SelectTag(_setting.TagName);
                            }, s);
                    }

                    menu.DropDown(buttonRect);
                }

                if (GUILayout.Button("編集", GUILayout.MaxWidth(40f)))
                {
                    Selection.activeObject = _setting;
                }

                // ここから設定ファイルを生成する
                if (GUILayout.Button("新規", GUILayout.MaxWidth(40f)))
                {
                    var path = EditorUtility.SaveFilePanelInProject("保存先", "NewLauncherSettings", "asset", "message");
                    if (string.IsNullOrEmpty(path)) return;
                    if (!path.EndsWith(".asset")) path += ".asset";

                    var setting = CreateInstance<ToolLauncherSetting>();
                    EditorUtility.SetDirty(setting);
                    AssetDatabase.CreateAsset(setting, path);
                    AssetDatabase.SaveAssets();
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<ToolLauncherSetting>(path);
                    ShowNotification(new GUIContent($"新規作成完了"), 3f);
                }
            }
            
            m_TagManager?.OnGUI(_settings);
        }
    }
}
