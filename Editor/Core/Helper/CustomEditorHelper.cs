#nullable enable

using System;
using System.Reflection;

namespace Rano.Editor
{
    public static class CustomEditorHelper
    {
         public static Type GetCustomEditorType_FromObject(UnityEngine.Object objectRef)
         {
             // UnityEditor.CustomEditorAttributes 인스턴스를 생성한다.
             var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
             var instance = assembly.CreateInstance("UnityEditor.CustomEditorAttributes");
  
             // UnityEditor.CustomEditorAttributes 타입을 얻는다.
             var type = instance.GetType();
             
             // Static 메서드인 FindCustomEditorType 을 얻는다.
             BindingFlags bf = BindingFlags.Static | BindingFlags.NonPublic;
             MethodInfo methodInfo = type.GetMethod("FindCustomEditorType", bf);
             
             // FindCustomEditorType 메서드를 사용해서 특정 개체의 CustomEditor 클래스를 얻는다.
             bool multiEdit = false;
             return (Type)methodInfo.Invoke(
                 instance,
                 new object[] { objectRef, multiEdit }
             );
         }
    }
}