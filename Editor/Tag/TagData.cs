using UnityEngine;

namespace ToolLauncher.Tag
{
    /// <summary>
    /// ランチャー設定に付与するタグ情報
    /// </summary>
    public struct TagData
    {
        /// <summary>
        /// タグの名前
        /// </summary>
        public string tagName;
        
        /// <summary>
        /// タグの色
        /// </summary>
        public Color tagColor;

        public TagData(string tagName, Color color)
        {
            this.tagName = tagName;
            tagColor = color;
        }
    }
}