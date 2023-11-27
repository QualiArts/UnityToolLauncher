using System;
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
                return type.GetMethod(_methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
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

        private SerializedMethodInfo(MethodInfo methodInfo)
        {
            _assemblyQualifiedName = methodInfo.ReflectedType?.AssemblyQualifiedName;
            _methodName = methodInfo.Name;
        }
    }
}
