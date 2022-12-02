#nullable enable

using System;
using System.Reflection;

namespace Rano.Editor
{
    public static class CustomEditorHelper
    {
         public static Type? GetCustomEditorType_FromObject(UnityEngine.Object objectRef)
         {
             // UnityEditor.CustomEditorAttributes 인스턴스를 생성한다.
             Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
             
             object? instance = assembly.CreateInstance("UnityEditor.CustomEditorAttributes");
             if (instance == null) return null;
  
             // UnityEditor.CustomEditorAttributes 타입을 얻는다.
             Type type = instance.GetType();
             
             // Static 메서드인 FindCustomEditorType 을 얻는다.
             BindingFlags bf = BindingFlags.Static | BindingFlags.NonPublic;
             MethodInfo? findCustomEditorTypeMethod = type.GetMethod("FindCustomEditorType", bf);
             if (findCustomEditorTypeMethod == null) return null;
             
             // FindCustomEditorType 메서드를 사용해서 특정 개체의 CustomEditor 클래스를 얻는다.
             bool multiEdit = false;
             return (Type)findCustomEditorTypeMethod.Invoke(
                 instance,
                 new object[] { objectRef, multiEdit }
             );
         }
    }
}