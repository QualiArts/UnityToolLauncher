using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ToolLauncher
{
    /// <summary>
    /// タグ選択GUIの管理
    /// </summary>
    public class TagManager
    {
        [NonSerialized] private Dictionary<string, bool> tagEnabled = new Dictionary<string, bool>();
        private Dictionary<string, Texture2D> tagTextures = new Dictionary<string, Texture2D>();

        static class Styles
        {
            /// <summary>
            /// 有効なタグのボタン
            /// </summary>
            public static readonly GUIStyle OnTagButton = new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(0, 0, 3,  0),
                padding = new RectOffset(8, 8, 0, 2),
                fixedHeight = 20,
                alignment = TextAnchor.MiddleRight,
            };
            
            /// <summary>
            /// 無効なタグのボタン
            /// </summary>
            public static readonly GUIStyle OffTagButton = new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(0, 0, 3,  0),
                padding = new RectOffset(8, 8, 0, 2),
                fixedHeight = 20,
                alignment = TextAnchor.MiddleRight,
            };
        }

        /// <summary>
        /// 有効化されているタグの数
        /// </summary>
        public int ActiveTagCount
        {
            get { return tagEnabled.Count(x => x.Value == true); }
        }

        /// <summary>
        /// 指定のタグが有効か
        /// </summary>
        public bool IsTagActive(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return false;
            }
            if (!tagEnabled.ContainsKey(tagName))
            {
                return false;
            }
            return tagEnabled[tagName];
        }
        
        /// <summary>
        /// 指定したタグのみ有効か
        /// </summary>
        public bool IsOnlyActive(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return false;
            }
            if (!tagEnabled.ContainsKey(tagName))
            {
                return false;
            }
            if (!tagEnabled[tagName])
            {
                return false;
            }

            return ActiveTagCount == 1;
        }

        /// <summary>
        /// タグの選択
        /// </summary>
        public void SelectTag(string tagName)
        {
            var keys = tagEnabled.Keys.ToArray();
            foreach (var key in keys)
            {
                tagEnabled[key] = false;
            }
            SetTagActive(tagName, true);
        }
        
        /// <summary>
        /// タグの有効化
        /// </summary>
        public void SetTagActive(string tagName, bool active)
        {
            if (tagEnabled.ContainsKey(tagName))
            {
                tagEnabled[tagName] = active;
            }
            else
            {
                tagEnabled.Add(tagName, active);
            }
        }
        
        public void OnGUI(ToolLauncherSetting[] settings)
        {
            var box = new GUIStyle(GUI.skin.box);
            box.wordWrap = true;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(4);
                EditorGUILayout.LabelField("タグ", GUILayout.Width(26));
                
                foreach (var setting in settings)
                {
                    var tag = setting.TagName;
                    if (string.IsNullOrEmpty(tag))
                    {
                        continue;
                    }
                    if (!tagEnabled.ContainsKey(tag))
                    {
                        tagEnabled.Add(tag, false);
                    }
                    
                    bool isClick = DrawTagButton(tag, setting);
                    if (isClick)
                    {
                        tagEnabled[tag] = !tagEnabled[tag];
                    }
                }
                GUILayout.FlexibleSpace();
            }
        }

        /// <summary>
        /// タグボタンの描画
        /// </summary>
        /// <param name="tag">タグ名</param>
        /// <param name="setting">設定ファイル</param>
        /// <returns>クリックされたらtrue</returns>
        private bool DrawTagButton(string tag, ToolLauncherSetting setting)
        {
            const int iconSpace = 10;
            const int iconSize = 6;

            var defaultColor = GUI.color;
            bool isOn = tagEnabled[tag];
            if (!isOn)
            {
                GUI.color = new Color(1, 1, 1, 0.5f);
            }

            // ボタン
            var buttonStyle = isOn ? Styles.OnTagButton : Styles.OffTagButton;
            GUIContent content = new GUIContent(tag);
            Vector2 contentSize = buttonStyle.CalcSize(content);
            contentSize.x += iconSpace; // アイコン用に幅を広げる
            var controlRect = EditorGUILayout.GetControlRect(false, contentSize.y, buttonStyle, GUILayout.Width(contentSize.x)); 
            bool isClick = GUI.Button(controlRect, content, buttonStyle);
            
            // ボタンの中のアイコン
            var icon = GetIconTexture(setting.TagName, setting.TagColor);
            var iconRect = new Rect(controlRect);
            iconRect.x = controlRect.x + iconSize;
            iconRect.y = controlRect.y + buttonStyle.fixedHeight / 2 - iconSize / 2;
            iconRect.width = iconSize;
            iconRect.height = iconSize;
            GUI.DrawTexture(iconRect, icon);

            // 色を元に戻す
            GUI.color = defaultColor; 

            return isClick;
        }


        Texture2D GetIconTexture(string tag, Color c)
        {
            if (!tagTextures.ContainsKey(tag))
            {
                tagTextures[tag] = new Texture2D(1, 1);
            }
            
            var t = tagTextures[tag];
            for (int x = 0; x < t.width; x++)
            {
                for (int y = 0; y < t.height; y++)
                {
                    t.SetPixel(x, y, c);
                }
            }
            t.Apply();
            return t;
        }
    }
}