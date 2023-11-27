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

        private void OnGUI()
        {
            // 設定選択画面
            DrawSettingSelectGUI();

            if (_setting == null) return;

            // 区切り線
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1f));

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            var list = _setting.GetMenuList();
            for (var index = 0; index < list.Count; index++)
            {
                var menu = list[index];
                _listDrawer.Draw(new ListDrawerParam(menu, index));
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 設定ファイルの選択画面を描画
        /// </summary>
        private void DrawSettingSelectGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                // 設定ファイルの選択Popup
                // EditorGUIのPopupは表示中に設定ファイルが消えるとハンドリングできないため独自のPopup
                var buttonContent = new GUIContent(_setting != null ? _setting.DisplayName : string.Empty);
                var buttonRect = GUILayoutUtility.GetRect(buttonContent, EditorStyles.popup);
                if (GUI.Button(buttonRect, buttonContent, EditorStyles.popup))
                {
                    GenericMenu menu = new GenericMenu();
                    // 設定を持っているScriptableObjectを探す
                    foreach (var s in AssetDatabase
                                 .FindAssets("t:" + nameof(ToolLauncherSetting))
                                 .Select(x => AssetDatabase.GUIDToAssetPath(x))
                                 .Select(x => AssetDatabase.LoadAssetAtPath<ToolLauncherSetting>(x)))
                    {
                        menu.AddItem(new GUIContent(s.DisplayName), _setting == s,
                            data => _setting = (ToolLauncherSetting)data, s);
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
        }
    }
}
