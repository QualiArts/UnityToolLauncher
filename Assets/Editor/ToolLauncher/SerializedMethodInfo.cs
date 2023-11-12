using System;
using System.Reflection;
using JetBrains.Annotations;
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
        public static SerializedMethodInfo CreateFromJsonString(string json)
        {
            if (json == null) return null;
            return JsonUtility.FromJson<SerializedMethodInfo>(json);
        }

        public SerializedMethodInfo(MethodInfo methodInfo)
        {
            _assemblyQualifiedName = methodInfo.ReflectedType?.AssemblyQualifiedName;
            _methodName = methodInfo.Name;
        }
    }
}
