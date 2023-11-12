using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ToolLauncher
{
    /// <summary>
    /// ランチャーの設定情報
    /// </summary>
    [Serializable]
    public class ToolLauncherSetting : ScriptableObject
    {
        /// <summary>
        /// それぞれのツールを表示するために必要な情報
        /// </summary>
        [Serializable]
        public class ToolLauncherMenu
        {
            public string name;
            public string tooltip;
            public string documentURL;
            public string iconText;
            public string serializedMethodInfoStr;
        }

        [SerializeField]
        private string _settingName;

        [SerializeField]
        protected List<ToolLauncherMenu> MenuList = new List<ToolLauncherMenu>();

        public string DisplayName => !string.IsNullOrEmpty(_settingName) ? _settingName : name;

        public IReadOnlyList<ToolLauncherMenu> GetMenuList()
        {
            return MenuList;
        }
    }

    #region CustomEditor

    // 日本語表示するためのEditor拡張
    [CustomEditor(typeof(ToolLauncherSetting))]
    class ToolLauncherSettingDrawer : Editor
    {
        private SerializedProperty _propSettingName;
        private SerializedProperty _propMenuList;

        private void OnEnable()
        {
            _propSettingName = serializedObject.FindProperty("_settingName");
            _propMenuList = serializedObject.FindProperty("MenuList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_propSettingName, new GUIContent("表示名"));
            EditorGUILayout.PropertyField(_propMenuList, new GUIContent("ツール一覧"));
            if (GUILayout.Button("保存"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssetIfDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion

    #region PropertyDrawer

    [CustomPropertyDrawer(typeof(ToolLauncherSetting.ToolLauncherMenu))]
    class ToolLauncherMenuDrawer : PropertyDrawer
    {
        // ToolLauncherSettingの設定画面でAssemblyを探すメニュー出す
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.iconText)), new GUIContent("略称"));
            rect.y += rect.height + 2f;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.name)), new GUIContent("名前"));
            rect.y += rect.height + 2f;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.tooltip)), new GUIContent("説明"));
            rect.y += rect.height + 2f;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.documentURL)), new GUIContent("説明URL(省略化)"));
            rect.y += rect.height + 2f;

            // AssemblyQualifiedNameを探すボタンを描画する
            var propMethodInfoStr = property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.serializedMethodInfoStr));
            const float buttonWidth = 80f;
            var horizontalRect1 = new Rect(rect) { width = rect.width - buttonWidth };
            var horizontalRect2 = new Rect(rect) { x = rect.x + horizontalRect1.width, width = buttonWidth };

            // 直接変更不可に
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.PropertyField(horizontalRect1, propMethodInfoStr, new GUIContent("起動ツール"));
            }

            var controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
            if (GUI.Button(horizontalRect2, "ツール検索"))
            {
                var dropdown = new ToolSelectDropdown(new AdvancedDropdownState(), methodInfo =>
                {
                    if (methodInfo.ReflectedType != null)
                    {
                        propMethodInfoStr.stringValue = methodInfo.ReflectedType.AssemblyQualifiedName;
                        var serializedMethodInfo = new SerializedMethodInfo(methodInfo);
                        var json = EditorJsonUtility.ToJson(serializedMethodInfo);
                        propMethodInfoStr.stringValue = json;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                });
                dropdown.Show(horizontalRect1);
            }

            // DropDownMenuで選択されたTypeを取得する
            // var current = Event.current;
            // if (current.GetTypeForControl(controlID) == EventType.ExecuteCommand && current.commandName == "DropDownSelect")
            // {
            //     var selectedItem = DropDownMenu.GetSelectedItem(controlID);
            //     if (!selectedItem.IsNullOrEmpty())
            //     {
            //         propAssemblyQualifiedName.stringValue = selectedItem;
            //     }
            // }

            // 取得したTypeからWindowを開くボタン
            rect.y += rect.height + 2f;
            if (GUI.Button(new Rect(rect) { x = horizontalRect2.x, width = horizontalRect2.width }, "テスト実行"))
            {
                if (!string.IsNullOrEmpty(propMethodInfoStr.stringValue))
                {
                    var serializedMethodInfo = SerializedMethodInfo.CreateFromJsonString(propMethodInfoStr.stringValue);
                    serializedMethodInfo?.Method?.Invoke(null, null);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2f) * 6f;
        }

        private List<(string, MethodInfo)> GetMenuItemMethods()
        {
            // MenuItemのAttributeがついたメソッドを探して呼ぶ
            var result = new List<(string, MethodInfo)>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var methodInfo in assemblies
                         .SelectMany(x => x.GetTypes())
                         .SelectMany(x =>
                             x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                var menuItem = methodInfo.GetCustomAttribute<MenuItem>();
                if (menuItem == null) continue;
                
                result.Add((menuItem.menuItem, methodInfo));
            }

            return result;
        }
    }

    #endregion
}
