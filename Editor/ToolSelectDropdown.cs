using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace ToolLauncher
{
    public class ToolSelectDropdown : AdvancedDropdown
    {
        private class DropDownItem : AdvancedDropdownItem
        {
            public readonly MethodInfo methodInfo;

            public DropDownItem(string name, MethodInfo methodInfo) : base(name)
            {
                this.methodInfo = methodInfo;
            }
        }

        private static List<(string, MethodInfo)> _menuItemMethodsCache;
        private Action<MethodInfo> _onSelected;

        public ToolSelectDropdown(AdvancedDropdownState state, Action<MethodInfo> onSelected) : base(state)
        {
            _onSelected = onSelected;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Menu Items");

            foreach (var menuItemMethod in GetMenuItemMethods())
            {
                var pathSplit = menuItemMethod.Item1.Split('/');
                var methodInfo = menuItemMethod.Item2;
                AddItem(root, methodInfo, pathSplit);
            }

            return root;
        }


        private List<(string, MethodInfo)> GetMenuItemMethods()
        {
            if (_menuItemMethodsCache != null) return _menuItemMethodsCache;

            // MenuItemのAttributeがついたメソッドを全て探す
            var result = new List<(string, MethodInfo)>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var methodInfo in assemblies
                         .SelectMany(x => x.GetTypes())
                         .SelectMany(x =>
                             x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                var menuItems = methodInfo.GetCustomAttributes<MenuItem>();
                foreach (var menuItem in menuItems)
                {
                    if (menuItem == null) continue;
                    if (menuItem.menuItem.StartsWith("CONTEXT")) continue;

                    result.Add((menuItem.menuItem, methodInfo));
                }
            }

            _menuItemMethodsCache = result;
            return result;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is DropDownItem dropdown)
            {
                _onSelected?.Invoke(dropdown.methodInfo);
            }
        }

        private static void AddItem(AdvancedDropdownItem parent, MethodInfo methodInfo, string[] pathParts,
            int index = 0)
        {
            if (index >= pathParts.Length)
            {
                return;
            }

            var existingChild = parent.children.FirstOrDefault(x => x.name == pathParts[index]);
            if (existingChild == null)
            {
                var isLast = index == pathParts.Length - 1;
                var newChildName = isLast ? $"{pathParts[index]} ({string.Join('/', pathParts)})" : pathParts[index];
                var newChild = isLast
                    ? new DropDownItem(newChildName, methodInfo)
                    : new AdvancedDropdownItem(newChildName);

                parent.AddChild(newChild);
                AddItem(newChild, methodInfo, pathParts, index + 1);
            }
            else
            {
                AddItem(existingChild, methodInfo, pathParts, index + 1);
            }
        }
    }
}
