using UnityEditor;
using UnityEngine;

namespace ToolLauncher.ListDrawer
{
    #region Styles

    /// <summary>
    /// それぞれの配置を制御する
    /// width,Heightを元にしてRectが計算される 
    /// Rect内での微調整をpaddingで行う       
    /// </summary>
    static class Styles
    {
        public static readonly GUIContent IconContent = EditorGUIUtility.IconContent("d__Help");

        public static readonly GUIStyle ItemBackground1 = new GUIStyle() { name = "quick-search-item-background1", };

        public static readonly GUIStyle ItemBackground2 = new GUIStyle(ItemBackground1) { name = "quick-search-item-background2" };

        public static readonly GUIStyle ToolName = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            wordWrap = false,
            alignment = TextAnchor.UpperLeft,
            fixedHeight = 20,
            padding = new RectOffset(5, 0, 5, 0)
        };

        public static readonly GUIStyle Tooltip = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            wordWrap = false,
            alignment = TextAnchor.UpperLeft,
            fontSize = 8,
            padding = new RectOffset(5, 0, 0, 0)
        };

        public static readonly GUIStyle InfoIcon = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageOnly,
            fixedWidth = 50,
        };

        public static readonly GUIStyle TextIcon = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fixedWidth = 40,
            fontSize = 20,
            normal = new GUIStyleState
            {
                background = Texture2D.grayTexture,
                textColor = Color.white,
            },
        };
    }

    #endregion

    /// <summary>
    /// 長方形でリスト描画するクラス
    /// </summary>
    public class RectangleListDrawer : IListDrawer
    {
        public void Draw(ListDrawerParam param)
        {
            var menu = param.menu;
            var index = param.index;
            var originalRect = EditorGUILayout.GetControlRect(false, (EditorGUIUtility.singleLineHeight + 2f) * 2f);
            var current = Event.current;
            var hovered = originalRect.Contains(current.mousePosition);

            if (Event.current.type == EventType.Repaint)
            {
                // Backgroundの色を交互に
                var bgStyle = index % 2 == 0 ? Styles.ItemBackground1 : Styles.ItemBackground2;
                bgStyle.Draw(originalRect, hovered, false, false, false);
            }

            // 左側のアイコン
            var iconStyle = Styles.TextIcon;
            // 文字数によって
            if (menu.iconText.Length != 0)
                iconStyle.fontSize = Mathf.Min(20, 40 / menu.iconText.Length);
            var imageRect = new Rect(originalRect) { width = Styles.TextIcon.fixedWidth };
            GUI.Label(imageRect, menu.iconText, Styles.TextIcon);

            // 名前
            var nameLabelRect = new Rect(originalRect);
            nameLabelRect.x += imageRect.width;
            nameLabelRect.height = Styles.ToolName.fixedHeight;
            GUI.Label(nameLabelRect, menu.name, Styles.ToolName);

            //説明
            var toolTipLabelRect = new Rect(originalRect);
            toolTipLabelRect.x += imageRect.width;
            toolTipLabelRect.y += nameLabelRect.height;
            toolTipLabelRect.height = originalRect.height - nameLabelRect.height;
            GUI.Label(toolTipLabelRect, menu.tooltip, Styles.Tooltip);

            // helpボタン(URLがない場合は描画しない)
            Rect helpRect = Rect.zero;
            if (!string.IsNullOrEmpty(menu.documentURL))
            {
                helpRect = new Rect(originalRect);
                helpRect.x = helpRect.width - Styles.InfoIcon.fixedWidth;
                helpRect.width = Styles.InfoIcon.fixedWidth;
                GUI.Box(helpRect, Styles.IconContent, Styles.InfoIcon);
            }

            if (hovered && current.type == EventType.MouseDown)
            {
                // Helpボタンの判定を優先
                if (helpRect.Contains(current.mousePosition))
                {
                    Application.OpenURL(menu.documentURL);
                }
                else if (originalRect.Contains(current.mousePosition))
                {
                    var serializedMethodInfo = SerializedMethodInfo.DeserializeFromJson(menu.serializedMethodInfoStr);
                    serializedMethodInfo?.Invoke();
                }

                current.Use();
            }
        }
    }
}
