using System;
using System.Collections.Generic;
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

        [SerializeField] private string _settingName;

        [SerializeField] protected List<ToolLauncherMenu> MenuList = new List<ToolLauncherMenu>();

        public string DisplayName => !string.IsNullOrEmpty(_settingName) ? _settingName : name;

        public IReadOnlyList<ToolLauncherMenu> GetMenuList()
        {
            return MenuList;
        }
    }

    #region CustomEditor

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
            EditorGUI.PropertyField(rect,
                property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.iconText)),
                new GUIContent("略称"));
            rect.y += rect.height + 2f;
            EditorGUI.PropertyField(rect,
                property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.name)), new GUIContent("名前"));
            rect.y += rect.height + 2f;
            EditorGUI.PropertyField(rect,
                property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.tooltip)),
                new GUIContent("説明"));
            rect.y += rect.height + 2f;
            EditorGUI.PropertyField(rect,
                property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.documentURL)),
                new GUIContent("説明URL(省略化)"));
            rect.y += rect.height + 2f;

            // AssemblyQualifiedNameを探すボタン
            var propMethodInfoStr =
                property.FindPropertyRelative(nameof(ToolLauncherSetting.ToolLauncherMenu.serializedMethodInfoStr));
            const float buttonWidth = 80f;
            var horizontalRect1 = new Rect(rect) { width = rect.width - buttonWidth };
            var horizontalRect2 = new Rect(rect) { x = rect.x + horizontalRect1.width, width = buttonWidth };

            // 直接変更不可に
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.PropertyField(horizontalRect1, propMethodInfoStr, new GUIContent("起動ツール"));
            }

            if (GUI.Button(horizontalRect2, "ツール検索"))
            {
                var dropdown = new ToolSelectDropdown(new AdvancedDropdownState(), methodInfo =>
                {
                    if (methodInfo.ReflectedType != null)
                    {
                        propMethodInfoStr.stringValue = SerializedMethodInfo.SerializeToJson(methodInfo);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                });
                dropdown.Show(horizontalRect1);
            }

            // テスト実行ボタン
            rect.y += rect.height + 2f;
            if (GUI.Button(new Rect(rect) { x = horizontalRect2.x, width = horizontalRect2.width }, "テスト実行"))
            {
                if (!string.IsNullOrEmpty(propMethodInfoStr.stringValue))
                {
                    var serializedMethodInfo = SerializedMethodInfo.DeserializeFromJson(propMethodInfoStr.stringValue);
                    serializedMethodInfo?.Invoke();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2f) * 6f;
        }
    }

    #endregion
}
