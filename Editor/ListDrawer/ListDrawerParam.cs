using JetBrains.Annotations;
using ToolLauncher.Tag;

namespace ToolLauncher.ListDrawer
{
    /// <summary>
    /// ランチャーのリスト描画に必要な情報
    /// </summary>
    public class ListDrawerParam
    {
        /// <summary>
        /// Menuの内容
        /// </summary>
        [NotNull]
        public readonly ToolLauncherSetting.ToolLauncherMenu menu;

        /// <summary>
        /// 表示順のIndex
        /// </summary>
        public readonly int index;

        /// <summary>
        /// タグ情報
        /// </summary>
        public readonly TagData tagData;

        public ListDrawerParam([NotNull] ToolLauncherSetting.ToolLauncherMenu menu, int index, TagData tagData = default)
        {
            this.menu = menu;
            this.index = index;
            this.tagData = tagData;
        }
    }
}
