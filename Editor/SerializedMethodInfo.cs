using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace ToolLauncher
{
    [Serializable]
    public class SerializedMethodInfo
    {
        [SerializeField] private string _assemblyQualifiedName;
        [SerializeField] private string _methodName;

        [CanBeNull]
        public MethodInfo Method
        {
            get
            {
                var type = Type.GetType(_assemblyQualifiedName);
                if (type == null) return null;
                var method = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
                    .First(m => m.Name == _methodName);
                return method;
            }
        }

        [CanBeNull]
        public static SerializedMethodInfo DeserializeFromJson(string json)
        {
            if (json == null) return null;
            return JsonUtility.FromJson<SerializedMethodInfo>(json);
        }

        public static string SerializeToJson(MethodInfo methodInfo)
        {
            var serializedMethodInfo = new SerializedMethodInfo(methodInfo);
            var json = EditorJsonUtility.ToJson(serializedMethodInfo);
            return json;
        }

        public void Invoke()
        {
            var parameterInfos = Method?.GetParameters();
            bool hasMenuCommandParam = parameterInfos?.Length == 1;
            var parameters = hasMenuCommandParam
                ? new object[] { new MenuCommand(default, default) }
                : null;

            Method?.Invoke(null, parameters);
        }

        private SerializedMethodInfo(MethodInfo methodInfo)
        {
            _assemblyQualifiedName = methodInfo.ReflectedType?.AssemblyQualifiedName;
            _methodName = methodInfo.Name;
        }
    }
}
