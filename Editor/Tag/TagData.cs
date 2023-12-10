using UnityEngine;

namespace ToolLauncher.Tag
{
    public struct TagData
    {
        public string tagName;
        public Color tagColor;

        public TagData(string s, Color color)
        {
            tagName = s;
            tagColor = color;
        }
    }
}