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
            public static readonly GUIStyle OnTagButton = new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(0, 0, 3,  0),
                padding = new RectOffset(8, 8, 0, 2),
                fixedHeight = 20,
                alignment = TextAnchor.MiddleRight,
            };
        
            public static readonly GUIStyle OffTagButton = new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(0, 0, 3,  0),
                padding = new RectOffset(8, 8, 0, 2),
                fixedHeight = 20,
                alignment = TextAnchor.MiddleRight,
            };
        }

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
                
                var defaultColor = GUI.color;
                var defaultBgColor = GUI.backgroundColor;
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
                    
                    bool isOn = tagEnabled[tag];
                    if (!isOn)
                    {
                        GUI.color = new Color(1, 1, 1, 0.5f);
                    }
                    else
                    {
                        GUI.color = defaultColor;
                    }

                    var style = isOn ? Styles.OnTagButton : Styles.OffTagButton;
                    var icon = GetIconTexture(setting.TagName, setting.TagColor);
                    GUIContent content = new GUIContent(tag);
                    Vector2 contentSize = style.CalcSize(content);
                    contentSize.x += 10;
                    
                    var controlRect = EditorGUILayout.GetControlRect(
                        false, contentSize.y, style, 
                        new GUILayoutOption[] { 
                            GUILayout.Width(contentSize.x), 
                            GUILayout.Height(contentSize.y)
                        });
                    
                    bool isClick = GUI.Button(controlRect, content, style);
                    var iconRect = new Rect(controlRect);
                    const int iconSize = 6;
                    iconRect.x = controlRect.x + iconSize;
                    iconRect.y = controlRect.y + style.fixedHeight / 2 - iconSize / 2;
                    iconRect.width = iconSize;
                    iconRect.height = iconSize;
                    GUI.DrawTexture(iconRect, icon);
                    if (isClick)
                    {
                        tagEnabled[tag] = !isOn;
                    }
                }

                GUI.backgroundColor = defaultBgColor;
                GUI.color = defaultColor;
                GUILayout.FlexibleSpace();
            }
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